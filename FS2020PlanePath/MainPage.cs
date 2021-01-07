using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.FlightSimulator.SimConnect;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Diagnostics;

namespace FS2020PlanePath
{
    public partial class MainPage : Form
    {

        //const string sourceRepo = "SAHorowitz/MSFS2020-PilotPathRecorder";
        const string sourceRepo = "noodnik2/MSFS2020-PilotPathRecorder";

        bool bLoggingEnabled = false;
        MSFS2020_SimConnectIntergration simConnectIntegration = new MSFS2020_SimConnectIntergration();
        InternalWebServer activeInternalWebserver;
        ScKmlAdapter scKmlAdapter = new ScKmlAdapter();
        LiveCamRegistry liveCamRegistry = new LiveCamRegistry();
        FS2020_SQLLiteDB FlightPathDB;
        int nCurrentFlightID;
        DateTime dtLastDataRecord;
        FlightPlan flightPlan;
        bool bStartedLoggingDueToSpeed;
        bool bStoppedLoggingDueToSpeed;

        public MainPage()
        {
            InitializeComponent();
            this.Text += Program.sAppVersion;
            FlightPathDB = new FS2020_SQLLiteDB();
            FlightPathDB.CreateTables();
            flightPlan = new FlightPlan();
            ThresholdLogWriteFreqTB.Text = FlightPathDB.GetTableOption("AboveThresholdWriteFreq");
            ThresholdMinAltTB.Text = FlightPathDB.GetTableOption("ThresholdMinAltitude");
            KMLFilePathTBRO.Text = FlightPathDB.GetTableOption("KMLFilePath");
            if (string.Compare(FlightPathDB.GetTableOption("GoolgeEarthChoice"), "Application") == 0)
                GoogleEarthAppRB.Checked = true;
            else
                GoogleEarthWebRB.Checked = true;

            if (string.Compare(FlightPathDB.GetTableOption("SpeedUpVideoPlayback"), "true") == 0)
                SpeedUpVideoPlaybackCB.Checked = true;
            else
                SpeedUpVideoPlaybackCB.Checked = false;

            if (string.Compare(FlightPathDB.GetTableOption("AutomaticLogging"), "true") == 0)
                AutomaticLoggingCB.Checked = true;
            else
                AutomaticLoggingCB.Checked = false;

            LoggingThresholdGroundVelTB.Text = FlightPathDB.GetTableOption("AutomaticLoggingThreshold");
            LoggingThresholdGroundVelTB.Enabled = AutomaticLoggingCB.Checked;

            LoadFlightList();
        }

        private void LogWriteFreqTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            // only want numbers
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LogFolderBrowser_Click(object sender, EventArgs e)
        {
            FolderSelectDialog FolderSelectDialog1 = new FolderSelectDialog();

            // Show the FolderBrowserDialog.
            if (FolderSelectDialog1.Show(Handle))
                KMLFilePathTBRO.Text = FolderSelectDialog1.FileName;
        }

        private void MainPage_Shown(object sender, EventArgs e)
        {
            string sAppLatestVersion;

            simConnectIntegration.FForm = this;
            sAppLatestVersion = ReadLatestAppVersionFromWeb();
            if (sAppLatestVersion.Equals(Program.sAppVersion) == false)
                if (MessageBox.Show("There is a newer version of the application available. Do you wish to download it now?", "New Version Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Process.Start($"https://github.com/{sourceRepo}");
            AttemptSimConnection();
            nCurrentFlightID = 0;
            bStartedLoggingDueToSpeed = false;
            bStoppedLoggingDueToSpeed = true;
        }

        private void AttemptSimConnection()
        {
            if (simConnectIntegration.Connect() == true)
            {
                simConnectIntegration.Initialize();
                SimConnectStatusLabel.Text = "SimConnect Connected";
                StartLoggingBtn.Enabled = true;
                RetrySimConnectionBtn.Enabled = false;
            }
            else
            {
                SimConnectStatusLabel.Text = "Unable to connect to FS2020";
                RetrySimConnectionBtn.Enabled = true;
                StartLoggingBtn.Enabled = false;
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == MSFS2020_SimConnectIntergration.WM_USER_SIMCONNECT)
            {
                if (simConnectIntegration.SimConnect != null)
                {
                    try
                    {
                        simConnectIntegration.SimConnect.ReceiveMessage();
                    }
                    catch (Exception ex)
                    {
                        SimConnectStatusLabel.Text = "Connection lost to SimConnect";
                        StopLoggingBtn.PerformClick();
                    }
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        private string internalWebserverResponse(string requestPath)
        {
            //Console.WriteLine($"received path({requestPath})");
            KmlLiveCam liveCam;
            if (!liveCamRegistry.TryGetByAlias(requestPath, out liveCam))
            {
                return $"<error text='LiveCam({requestPath}) not found; request ignored' />";
            }
            liveCam.Camera.Values = scKmlAdapter.KmlCameraValues;
            return liveCam.Camera.Render();
        }

        private void StartLoggingBtn_Click(object sender, EventArgs e)
        {
            // set the last time a record was written to mintime
            dtLastDataRecord = DateTime.MinValue;

            // sim is not connected try one time
            if (simConnectIntegration.IsSimConnected() == false)
                AttemptSimConnection();

            // if sim is still not connected then abort logging
            if (simConnectIntegration.IsSimConnected() == false)
                return;

            if (simConnectIntegration.IsSimInitialized() == true)
                bLoggingEnabled = true;
            else
                SimConnectStatusLabel.Text = "Unable to connect to FS2020";

            // set buttons accordingly based on logging situation
            if (bLoggingEnabled == true)
            {
                // visible textboxes and labels around log file name and disable start logging button and enable stop logging button
                StartLoggingBtn.Enabled = false;
                PauseLoggingBtn.Enabled = true;
                StopLoggingBtn.Enabled = true;
                ContinueLogginBtn.Enabled = false;
            }
            else
            {
                // visible textboxes and labels around log file name and disable start logging button and enable stop logging button
                StartLoggingBtn.Enabled = true;
                PauseLoggingBtn.Enabled = false;
                StopLoggingBtn.Enabled = false;
                ContinueLogginBtn.Enabled = false;
            }
        }

        // function is called from the retrieval of information from the simconnect and in this case stores it in the database based 
        // on prefrences
        public void UseData(MSFS2020_SimConnectIntergration.SimPlaneDataStructure simPlaneData)
        {
            if (AutomaticLoggingCB.Checked == true)
            {
                // if user wanted automatic logging and ground speed is > LoggingThresholdGroundVelTB and they hadn't stoppeed logging before manaually then turn logging on and write aircraft to start flight
                if (simPlaneData.ground_velocity >= Convert.ToInt32(LoggingThresholdGroundVelTB.Text))
                {
                    if ((bLoggingEnabled == false) && (bStoppedLoggingDueToSpeed == true))
                    {
                        StartLoggingBtn.PerformClick();
                        bStartedLoggingDueToSpeed = true;
                    }
                }
                else
                {
                    if (bLoggingEnabled == true)
                    {
                        // if ground speed is < LoggingThresholdGroundVelTB and user wanted automatic logging and logging was on due to speed then turn it off
                        if (bStartedLoggingDueToSpeed == true)
                            StopLoggingAction();
                    }
                    bStoppedLoggingDueToSpeed = true;
                }
            }

            if (bLoggingEnabled == true)
            {
                // if we don't have flight header information then ask for it and don't write out this data point
                if (nCurrentFlightID == 0)
                {
                    simConnectIntegration.GetSimEnvInfo();
                }
                else
                {
                    System.TimeSpan tsDiffRecords = DateTime.Now - dtLastDataRecord;

                    // if altitude above ground is greater or equal threshold and it has been threshold amount of time or
                    //    altitude is below threshold
                    if (((simPlaneData.altitude_above_ground >= Convert.ToInt32(ThresholdMinAltTB.Text)) &&
                         (tsDiffRecords.TotalSeconds >= Convert.ToDouble(ThresholdLogWriteFreqTB.Text))) ||
                        (simPlaneData.altitude_above_ground < Convert.ToInt32(ThresholdMinAltTB.Text)))
                    {
                        int FlightSampleID;

                        FlightSampleID = FlightPathDB.WriteFlightPoint(nCurrentFlightID, simPlaneData.latitude, simPlaneData.longitude, simPlaneData.altitude);
                        FlightPathDB.WriteFlightPointDetails(FlightSampleID, simPlaneData.altitude_above_ground, simPlaneData.engine1rpm, simPlaneData.engine2rpm, simPlaneData.engine3rpm,
                                                             simPlaneData.engine4rpm, simPlaneData.lightsmask, simPlaneData.ground_velocity, simPlaneData.plane_pitch, simPlaneData.plane_bank, simPlaneData.plane_heading_true,
                                                             simPlaneData.plane_heading_magnetic, simPlaneData.plane_airspeed_indicated, simPlaneData.airspeed_true, simPlaneData.vertical_speed,
                                                             simPlaneData.heading_indicator, simPlaneData.flaps_handle_position, simPlaneData.spoilers_handle_position,
                                                             simPlaneData.gear_handle_position, simPlaneData.ambient_wind_velocity, simPlaneData.ambient_wind_direction, simPlaneData.ambient_temperature,
                                                             simPlaneData.stall_warning, simPlaneData.overspeed_warning, simPlaneData.is_gear_retractable, simPlaneData.spoiler_available, simPlaneData.sim_on_ground);
                        dtLastDataRecord = DateTime.Now;
                    }
                    flightPlan.AddFlightPlanWaypoint(new FlightWaypointData(simPlaneData.gps_wp_prev_latitude, simPlaneData.gps_wp_prev_longitude, simPlaneData.gps_wp_prev_altitude, simPlaneData.gps_wp_prev_id),
                                                     new FlightWaypointData(simPlaneData.gps_wp_next_latitude, simPlaneData.gps_wp_next_longitude, simPlaneData.gps_wp_next_altitude, simPlaneData.gps_wp_next_id),
                                                     simPlaneData.gps_flight_plan_wp_index, simPlaneData.gps_flight_plan_wp_count);

                    scKmlAdapter.Update(simPlaneData);
                }
            }
        }

        // function is called from the retrieval of information from the simconnect and hold the aircraft name info
        public void UseSimEnvData(string aircraft)
        {
            if (bLoggingEnabled == true)
            {
                nCurrentFlightID = FlightPathDB.WriteFlight(aircraft);
            }
        }

        // function that writes out KML file based on the flight chosen by the user
        private void CreateKMLButton_Click(object sender, EventArgs e)
        {
            int nCount;
            int nFlightID;
            string sfilename;
            long lprevTimestamp;

            bLoggingEnabled = false;

            if (FlightPickerLV.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please choose a flight before exporting.", "Export KML File");
                return;
            }

            if (KMLFilePathTBRO.Text.Length == 0)
            {
                MessageBox.Show("Please choose a folder location before exporting.", "Export KML File");
                return;
            }

            nFlightID = (int)FlightPickerLV.SelectedItems[0].Tag;

            // This is the root element of the file
            var kml = new Kml();
            Folder mainFolder = new Folder();
            mainFolder.Name = String.Format("{0} {1}", FlightPickerLV.SelectedItems[0].SubItems[1].Text, FlightPickerLV.SelectedItems[0].SubItems[0].Text);
            mainFolder.Description = new Description
            {
                Text = "Overall Data for the flight"
            };

            kml.Feature = mainFolder;

            // start of Flight Path Line
            var placemarkLine = new Placemark();
            mainFolder.AddFeature(placemarkLine);
            placemarkLine.Name = "Flight Path Line";
            placemarkLine.Description = new Description
            {
                Text = "Line of the flight"
            };
            var linestring = new LineString();
            var coordinatecollection = new CoordinateCollection();

            linestring.Coordinates = coordinatecollection;
            linestring.AltitudeMode = AltitudeMode.Absolute;

            SharpKml.Dom.LineStyle lineStyle = new SharpKml.Dom.LineStyle();
            lineStyle.Color = Color32.Parse("ff0000ff");
            lineStyle.Width = 5;
            Style flightStyle = new Style();
            flightStyle.Id = "FlightStyle";
            flightStyle.Line = lineStyle;
            linestring.Extrude = false;
            mainFolder.AddStyle(flightStyle);

            SharpKml.Dom.Style waypointStyle = new SharpKml.Dom.Style();
            waypointStyle.Id = "WaypointStyle";
            waypointStyle.Icon = new SharpKml.Dom.IconStyle();
            waypointStyle.Icon.Icon = new SharpKml.Dom.IconStyle.IconLink(new System.Uri("https://maps.google.com/mapfiles/kml/paddle/grn-square.png"));
            mainFolder.AddStyle(waypointStyle);

            SharpKml.Dom.Style pushpinblueStyle = new SharpKml.Dom.Style();
            pushpinblueStyle.Id = "PushPinBlueStyle";
            pushpinblueStyle.Icon = new SharpKml.Dom.IconStyle();
            pushpinblueStyle.Icon.Icon = new SharpKml.Dom.IconStyle.IconLink(new System.Uri("https://maps.google.com/mapfiles/kml/pushpin/blue-pushpin.png"));
            mainFolder.AddStyle(pushpinblueStyle);

            SharpKml.Dom.Style pushpingreenStyle = new SharpKml.Dom.Style();
            pushpingreenStyle.Id = "PushPinGreenStyle";
            pushpingreenStyle.Icon = new SharpKml.Dom.IconStyle();
            pushpingreenStyle.Icon.Icon = new SharpKml.Dom.IconStyle.IconLink(new System.Uri("https://maps.google.com/mapfiles/kml/pushpin/grn-pushpin.png"));
            mainFolder.AddStyle(pushpingreenStyle);

            placemarkLine.StyleUrl = new Uri("#FlightStyle", UriKind.Relative);

            List<FlightPathData> FlightPath = new List<FlightPathData>();
            FlightPath = FlightPathDB.GetFlightPathData(nFlightID);
            foreach (FlightPathData fpd in FlightPath)
                coordinatecollection.Add(new Vector(fpd.latitude, fpd.longitude, fpd.altitude * 0.3048));
            placemarkLine.Geometry = linestring;

            // start of Flight Plan Waypoints
            List<FlightWaypointData> FlightWaypoints = new List<FlightWaypointData>();
            FlightWaypoints = FlightPathDB.GetFlightWaypoints(nFlightID);
            if (FlightWaypoints.Count > 0)
            {
                Folder FlightPlanFolder = new Folder();

                FlightPlanFolder.Name = "Flight Plan";
                FlightPlanFolder.Description = new Description
                {
                    Text = "Waypoints along the flight plan"
                };
                mainFolder.AddFeature(FlightPlanFolder);
                foreach (FlightWaypointData waypointData in FlightWaypoints)
                {
                    var placemarkPoint = new Placemark();

                    placemarkPoint.Name = waypointData.gps_wp_name;
                    placemarkPoint.StyleUrl = new System.Uri("#WaypointStyle", UriKind.Relative);
                    placemarkPoint.Geometry = new SharpKml.Dom.Point
                    {
                        Coordinate = new Vector(waypointData.gps_wp_latitude, waypointData.gps_wp_longitude, (double)waypointData.gps_wp_altitude * 0.3048),
                        AltitudeMode = AltitudeMode.Absolute
                    };
                    placemarkPoint.Description = new Description
                    {
                        Text = String.Concat(String.Format("Coordinates ({0:0.0000}, {1:0.0000}, {2} feet)", waypointData.gps_wp_longitude, waypointData.gps_wp_latitude, waypointData.gps_wp_altitude))
                    };
                    FlightPlanFolder.AddFeature(placemarkPoint);
                }
            }

            // start of Flight Data Points
            Folder DataPointsfolder = new Folder();
            DataPointsfolder.Name = "Flight Path Data Points";
            DataPointsfolder.Visibility = false;
            DataPointsfolder.Description = new Description
            {
                Text = "Data Points along the flight path"
            };
            mainFolder.AddFeature(DataPointsfolder);

            nCount = 0;
            foreach (FlightPathData fpd in FlightPath)
            {
                var placemarkPoint = new Placemark();
                string descriptioncard;
                bool bAnyLightsOn = false;

                nCount++;
                // if Google Earth App then you need to turn off visibility on each data point also
                if (GoogleEarthAppRB.Checked == true)
                    placemarkPoint.Visibility = false;
                placemarkPoint.Name = String.Concat("Flight Data Point ", nCount.ToString());
                placemarkPoint.Id = nCount.ToString();
                descriptioncard = String.Concat("<br>Timestamp = ", new DateTime(fpd.timestamp).ToString());

                descriptioncard += String.Concat(String.Format("<br><br>Coordinates ({0:0.0000}, {1:0.0000}, {2} feet)", fpd.latitude, fpd.longitude, fpd.altitude));
                descriptioncard += String.Format("<br>Temperature: {0:0.00}C / {1:0.00}F", fpd.ambient_temperature, fpd.ambient_temperature * 9 / 5 + 32);
                descriptioncard += String.Format("<br>Wind: {0:0.00} knts from {1:0.00} degrees", fpd.ambient_wind_velocity, fpd.ambient_wind_direction);
                descriptioncard += String.Format("<br>Altitude Above Ground: {0} feet", fpd.altitudeaboveground);
                if (fpd.sim_on_ground == 1)
                    descriptioncard += String.Format("<br>Plane Is On The Ground");

                descriptioncard += String.Format("<br><br>Heading Indicator: {0:0.00} degrees", fpd.heading_indicator);
                descriptioncard += String.Format("<br>True Heading: {0:0.00} degrees", fpd.plane_heading_true);
                descriptioncard += String.Format("<br>Magnetic Heading {0:0.00} degrees", fpd.plane_heading_magnetic);

                descriptioncard += string.Format("<br><br>Airspeed Indicated: {0:0.00 knts}", fpd.plane_airspeed_indicated);
                descriptioncard += string.Format("<br>Airspeed True: {0:0.00} knts", fpd.airspeed_true);
                descriptioncard += string.Format("<br>Ground Velocity: {0:0.00} knts", fpd.ground_velocity);
                descriptioncard += string.Format("<br>Engine 1: {0} RPM", fpd.Eng1Rpm);
                if (fpd.Eng2Rpm > 0)
                    descriptioncard += string.Format("<br>Engine 2: {0} RPM", fpd.Eng2Rpm);
                if (fpd.Eng3Rpm > 0)
                    descriptioncard += string.Format("<br>Engine 3: {0} RPM", fpd.Eng3Rpm);
                if (fpd.Eng4Rpm > 0)
                    descriptioncard += string.Format("<br>Engine 4: {0} RPM", fpd.Eng4Rpm);

                descriptioncard += string.Format("<br><br>Pitch: {0:0.00} degrees {1}", Math.Abs(fpd.plane_pitch), fpd.plane_pitch < 0 ? "Up" : "Down");
                descriptioncard += string.Format("<br>Bank: {0:0.00} degrees {1}", Math.Abs(fpd.plane_bank), fpd.plane_bank < 0 ? "Right" : "Left");
                descriptioncard += string.Format("<br>Vertical Speed: {0:0} feet per minute", fpd.vertical_speed);
                descriptioncard += string.Concat("<br>Flaps Position: ", fpd.flaps_handle_position);

                descriptioncard += string.Concat("<br><br>Lights On: ");

                // go thru mask and set lights that are on
                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Nav) == (int)FlightPathData.LightStates.Nav)
                {
                    descriptioncard += string.Concat("Nav, ");
                    bAnyLightsOn = true;
                }
                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Beacon) == (int)FlightPathData.LightStates.Beacon)
                {
                    descriptioncard += string.Concat("Beacon, ");
                    bAnyLightsOn = true;
                }
                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Landing) == (int)FlightPathData.LightStates.Landing)
                {
                    descriptioncard += string.Concat("Landing, ");
                    bAnyLightsOn = true;
                }
                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Taxi) == (int)FlightPathData.LightStates.Taxi)
                {
                    descriptioncard += string.Concat("Taxi, ");
                    bAnyLightsOn = true;
                }
                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Strobe) == (int)FlightPathData.LightStates.Strobe)
                {
                    descriptioncard += string.Concat("Strobe, ");
                    bAnyLightsOn = true;
                }
                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Panel) == (int)FlightPathData.LightStates.Panel)
                {
                    descriptioncard += string.Concat("Panel, ");
                    bAnyLightsOn = true;
                }
                // commented out the following lights because most planes don't use them and it messes up the GA aircraft
                /*                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Recognition) == (int)FlightPathData.LightStates.Recognition)
                                {
                                    descriptioncard += string.Concat("Recognition, ");
                                    bAnyLightsOn = true;
                                }
                                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Wing) == (int)FlightPathData.LightStates.Wing)
                                {
                                    descriptioncard += string.Concat("Wing, ");
                                    bAnyLightsOn = true;
                                }
                                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Logo) == (int)FlightPathData.LightStates.Logo)
                                {
                                    descriptioncard += string.Concat("Logo, ");
                                    bAnyLightsOn = true;
                                }*/
                if ((fpd.LightsMask & (int)FlightPathData.LightStates.Cabin) == (int)FlightPathData.LightStates.Cabin)
                {
                    descriptioncard += string.Concat("Cabin, ");
                    bAnyLightsOn = true;
                }

                // if there are no masks then put none else remove last two characters which are the comma and the space from above
                if (bAnyLightsOn == false)
                    descriptioncard += string.Concat("None");
                else
                    descriptioncard = descriptioncard.Remove(descriptioncard.Length - 2, 2);

                if (fpd.is_gear_retractable == 1)
                    descriptioncard += String.Concat("<br>Gear Position: ", fpd.gear_handle_position == 1 ? "Down" : "Up");
                if (fpd.spoiler_available == 1)
                    descriptioncard += String.Concat("<br>Spoiler Position: ", fpd.spoilers_handle_position == 1 ? "On" : "Off");
                if (fpd.stall_warning == 1)
                    descriptioncard += "<br>Stall Warning";
                if (fpd.overspeed_warning == 1)
                    descriptioncard += "<br>Overspeed Warning";

                placemarkPoint.Description = new Description
                {
                    Text = descriptioncard
                };

                // turned off showing time with data points as it caused issues of not showing in Google Earth 
                // if user turned them off and then back on
                /*                placemarkPoint.Time = new SharpKml.Dom.Timestamp
                                {
                                    When = new DateTime(fpd.timestamp)
                                };*/
                if (fpd.sim_on_ground == 1)
                    placemarkPoint.StyleUrl = new System.Uri("#PushPinGreenStyle", UriKind.Relative);
                else
                    placemarkPoint.StyleUrl = new System.Uri("#PushPinBlueStyle", UriKind.Relative);

                placemarkPoint.Geometry = new SharpKml.Dom.Point
                {
                    Coordinate = new Vector(fpd.latitude, fpd.longitude, (double)fpd.altitude * 0.3048),
                    AltitudeMode = AltitudeMode.Absolute
                };
                DataPointsfolder.AddFeature(placemarkPoint);
            }

            // add 1st person feature
            SharpKml.Dom.GX.Tour tour = new SharpKml.Dom.GX.Tour();
            tour.Name = "First Person View of Flight";
            kml.AddNamespacePrefix("gx", "http://www.google.com/kml/ext/2.2");
            SharpKml.Dom.GX.Playlist playlist = new SharpKml.Dom.GX.Playlist();
            tour.Playlist = playlist;
            mainFolder.AddFeature(tour);
            lprevTimestamp = 0;
            nCount = 0;
            foreach (FlightPathData fpd in FlightPath)
            {
                nCount++;
                SharpKml.Dom.GX.FlyTo flyto = new SharpKml.Dom.GX.FlyTo();

                // assume duration will be based on difference between timestamps
                // if first time thru loop and don't have old time or user wants to speed up video playback above threshold
                // then set duration to 1 if it is greater than 1 else leave as-is
                flyto.Duration = (new DateTime(fpd.timestamp) - new DateTime(lprevTimestamp)).TotalMilliseconds / 1000;
                if ((lprevTimestamp == 0) ||
                    (SpeedUpVideoPlaybackCB.Checked == true))
                {
                    if (flyto.Duration > 1)
                        flyto.Duration = 1;
                }

                lprevTimestamp = fpd.timestamp;
                flyto.Mode = SharpKml.Dom.GX.FlyToMode.Smooth;
                SharpKml.Dom.Camera cam = new SharpKml.Dom.Camera();
                cam.AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute;
                cam.Latitude = fpd.latitude;
                cam.Longitude = fpd.longitude;
                cam.Altitude = fpd.altitude * 0.3048;
                cam.Heading = fpd.plane_heading_true;
                if (GoogleEarthAppRB.Checked == true)
                    cam.Roll = fpd.plane_bank;
                else
                    cam.Roll = fpd.plane_bank * -1;
                cam.Tilt = (90 - fpd.plane_pitch);

                SharpKml.Dom.Timestamp tstamp = new SharpKml.Dom.Timestamp();
                tstamp.When = new DateTime(fpd.timestamp);

                cam.GXTimePrimitive = tstamp;

                flyto.View = cam;
                playlist.AddTourPrimitive(flyto);

                // change it so balloons show during first person view (for potential future use)
                /*
                var placemarkPoint = new Placemark();
                placemarkPoint.TargetId = nCount.ToString();
                placemarkPoint.GXBalloonVisibility = true;
                SharpKml.Dom.Update update = new SharpKml.Dom.Update();
                update.AddUpdate(new ChangeCollection() { placemarkPoint });
                SharpKml.Dom.GX.AnimatedUpdate animatedUpdate = new SharpKml.Dom.GX.AnimatedUpdate();
                animatedUpdate.Update = update;
                playlist.AddTourPrimitive(animatedUpdate);*/
            }

            // write out KML file
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            sfilename = String.Format("{0}_{1}.kml", FlightPickerLV.SelectedItems[0].SubItems[1].Text, FlightPickerLV.SelectedItems[0].SubItems[0].Text);
            var validfilename = new string(sfilename.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
            sfilename = string.Concat(KMLFilePathTBRO.Text, "\\");
            sfilename += validfilename;

            System.IO.File.Delete(sfilename);
            KmlFile kmlfile = KmlFile.Create(kml, true);
            using (var stream = System.IO.File.OpenWrite(sfilename))
            {
                kmlfile.Save(stream);
            }
            MessageBox.Show(String.Format("Flight successfully exported to {0}", sfilename), "Export KML File");
        }

        // stop logging disconnects simconnect, sets buttons correctly and reloads flight list 
        // since a new flight was made
        private void StopLoggingBtn_Click(object sender, EventArgs e)
        {
            bStoppedLoggingDueToSpeed = false;
            StopLoggingAction();
        }

        private void StopLoggingAction()
        {
            bLoggingEnabled = false;
            FlightPathDB.WriteFlightPlan(nCurrentFlightID, flightPlan.flight_waypoints);

            StartLoggingBtn.Enabled = true;
            PauseLoggingBtn.Enabled = false;
            StopLoggingBtn.Enabled = false;
            ContinueLogginBtn.Enabled = false;
            LoadFlightList();
            if (Program.bLogErrorsWritten == true)
                ErrorTBRO.Text = "Errors detected.  Please see " + Program.ErrorLogFile() + " for more details";
            else
                ErrorTBRO.Text = "";

            nCurrentFlightID = 0;
            bStartedLoggingDueToSpeed = false;
        }

        // pause and continue logging are as simple as button visibility and setting logging flag
        private void PauseLoggingBtn_Click(object sender, EventArgs e)
        {
            bLoggingEnabled = false;

            StartLoggingBtn.Enabled = false;
            PauseLoggingBtn.Enabled = false;
            StopLoggingBtn.Enabled = true;
            ContinueLogginBtn.Enabled = true;
        }

        private void ContinueLogginBtn_Click(object sender, EventArgs e)
        {
            bLoggingEnabled = true;

            StartLoggingBtn.Enabled = false;
            PauseLoggingBtn.Enabled = true;
            StopLoggingBtn.Enabled = true;
            ContinueLogginBtn.Enabled = false;
        }

        private void RetrySimConnectionBtn_Click(object sender, EventArgs e)
        {
            RetrySimConnectionBtn.Enabled = false;
            AttemptSimConnection();
        }

        private void LoadFlightList()
        {
            List<FlightListData> FlightList = new List<FlightListData>();

            FlightPickerLV.Items.Clear();
            FlightPickerLV.Columns.Clear();

            FlightList = FlightPathDB.GetFlightList();

            // reverse order guarantees newest on top since they come from the database in order of creation (oldest first)
            foreach (FlightListData flist in Enumerable.Reverse(FlightList))
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = new DateTime(flist.start_flight_timestamp).ToString();
                lvi.SubItems.Add(flist.aircraft);
                lvi.Tag = flist.FlightID;
                FlightPickerLV.Items.Add(lvi);
            }

            FlightPickerLV.Columns.Add("Flight Start Date/Time", -1, HorizontalAlignment.Left);
            FlightPickerLV.Columns.Add("Aircraft", -1, HorizontalAlignment.Left);
            FlightPickerLV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void DeleteFlight_Click(object sender, EventArgs e)
        {
            int nFlightID;

            if (FlightPickerLV.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please choose a flight before deleting.", "Delete a Flight");
                return;
            }
            if (MessageBox.Show("Deleting a flight from the database cannot be undone.  Are you sure you want to delete?", "Delete a Flight", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                nFlightID = (int)FlightPickerLV.SelectedItems[0].Tag;
                FlightPathDB.DeleteFlight(nFlightID);
                LoadFlightList();
            }
        }

        private void MainPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            FlightPathDB.WriteTableOption("AboveThresholdWriteFreq", ThresholdLogWriteFreqTB.Text);
            FlightPathDB.WriteTableOption("ThresholdMinAltitude", ThresholdMinAltTB.Text);
            FlightPathDB.WriteTableOption("KMLFilePath", KMLFilePathTBRO.Text);
            if (GoogleEarthAppRB.Checked == true)
                FlightPathDB.WriteTableOption("GoolgeEarthChoice", "Application");
            else
                FlightPathDB.WriteTableOption("GoolgeEarthChoice", "Web");
            if (SpeedUpVideoPlaybackCB.Checked == true)
                FlightPathDB.WriteTableOption("SpeedUpVideoPlayback", "true");
            else
                FlightPathDB.WriteTableOption("SpeedUpVideoPlayback", "false");
            if (AutomaticLoggingCB.Checked == true)
                FlightPathDB.WriteTableOption("AutomaticLogging", "true");
            else
                FlightPathDB.WriteTableOption("AutomaticLogging", "false");
            FlightPathDB.WriteTableOption("AutomaticLoggingThreshold", LoggingThresholdGroundVelTB.Text);
            simConnectIntegration.CloseConnection();
        }

        private string ReadLatestAppVersionFromWeb()
        {
            string sRetVal = "";

            WebClient client = new WebClient();
            try
            {
                Stream stream = client.OpenRead($"https://raw.githubusercontent.com/{sourceRepo}/master/docs/latest_version.txt");
                StreamReader reader = new StreamReader(stream);
                sRetVal = reader.ReadToEnd();

                var sb = new StringBuilder(sRetVal.Length);
                foreach (char i in sRetVal)
                {
                    if (i == '\n')
                    {
                        sb.Append(Environment.NewLine);
                    }
                    else if (i != '\r' && i != '\t')
                        sb.Append(i);
                }
                sRetVal = sb.ToString();
                if (sRetVal.Contains("general") == true)
                {
                    var vals = sRetVal.Split(
                                                new[] { Environment.NewLine },
                                                StringSplitOptions.None
                                            )
                                .SkipWhile(line => !line.StartsWith("[general]"))
                                .Skip(1)
                                .Take(1)
                                .Select(line => new
                                {
                                    Key = line.Substring(0, line.IndexOf('=')),
                                    Value = line.Substring(line.IndexOf('=') + 1).Replace("\"", "").Replace(" ", "")
                                });
                    sRetVal = vals.FirstOrDefault().Value;
                }
            }
            catch (Exception e)
            {
                sRetVal = Program.sAppVersion;
            }

            return sRetVal;
        }

        private void AutomaticLoggingCB_Click(object sender, EventArgs e)
        {
            LoggingThresholdGroundVelTB.Enabled = AutomaticLoggingCB.Checked;
        }

        private void ErrorTBRO_TextChanged(object sender, EventArgs e)
        {

        }

        private void LiveCameraCB_CheckedChanged(object sender, EventArgs eventArgs)
        {
            // disable and free any currently running listener
            if (activeInternalWebserver != null)
            {
                activeInternalWebserver.Disable();
                activeInternalWebserver = null;
            }

            if (!LiveCameraCB.Checked)
            {
                // re-enable the user to change the live camera's URI
                LiveCameraHostPortTB.Enabled = true;
                MessageBox.Show(
                    "No longer listening.", 
                    "Live Camera Disabled",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // start a new live camera listener using supplied URI
            Uri hostUri = null;
            string problemMessage = null;
            string liveCamUrl = LiveCameraHostPortTB.Text;
            try
            {
                hostUri = new Uri(liveCamUrl);
                // ensure live cam is registered
                liveCamRegistry.LoadByUrl(liveCamUrl);
            }
            catch (UriFormatException ufe)
            {
                problemMessage = malformedUriErrorMessage(liveCamUrl, ufe);
            }

            if (problemMessage == null)
            {
                InternalWebServer internalWebserver = null;
                try
                {
                    internalWebserver = new InternalWebServer(
                        hostUri, 
                        request => internalWebserverResponse(request)
                    );
                    internalWebserver.Enable();
                    activeInternalWebserver = internalWebserver;
                }
                catch (SystemException ene)
                {
                    if (internalWebserver != null)
                    {
                        internalWebserver.Disable();
                    }
                    problemMessage = $"Could not listen on: {hostUri}.\n\nDetails: {ene.Message}";
                }
            }

            if (problemMessage != null)
            {
                displayError("Could not start Live Camera", $"{problemMessage}");
                LiveCameraCB.CheckedChanged -= LiveCameraCB_CheckedChanged;
                LiveCameraCB.Checked = false;
                LiveCameraCB.CheckedChanged += LiveCameraCB_CheckedChanged;
                return;
            }

            // prevent modification of the live camera's URI while it's active
            LiveCameraHostPortTB.Enabled = false;
            MessageBox.Show(
                "Listening at URL:\n" + hostUri, 
                "Live Camera Enabled",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

        }

        private void LiveCameraKml_Click(object sender, EventArgs e)
        {
            Func<string, string> kmlValidator = (
                kmlString =>
                {
                    try
                    {
                        new Parser().ParseString(kmlString, true);
                        return null;
                    }
                    catch (Exception pe)
                    {
                        return pe.Message;
                    }
                }
            );

            KmlLiveCam liveCam;
            string liveCamUrl = LiveCameraHostPortTB.Text;
            try
            {
                liveCam = liveCamRegistry.LoadByUrl(liveCamUrl);
            }
            catch (UriFormatException ufe)
            {
                displayError("Could not Invoke Editor", malformedUriErrorMessage(liveCamUrl, ufe));
                return;
            }

            string originalCameraKmlTemplate = liveCam.Camera.Template;
            string originalLinkKmlTemplate = liveCam.Link.Template;
            using (
                TextEditorForm kmlEditorForm = new TextEditorForm(
                    $"Live Camera KML Editor: {liveCam.Link.Values.alias}",
                    originalCameraKmlTemplate,
                    originalLinkKmlTemplate,
                    kmlValidator
                )
            ) {

                if (kmlEditorForm.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                List<string> changeList = new List<string>();
                    
                string updatedCameraKmlTemplate = kmlEditorForm.EditorText;
                if (originalCameraKmlTemplate != updatedCameraKmlTemplate)
                {
                    Console.WriteLine($"updatedCameraKmlTemplate({updatedCameraKmlTemplate})");
                    liveCam.Camera.Template = updatedCameraKmlTemplate;
                    changeList.Add("Camera");
                }

                string updatedLinkKmlTemplate = kmlEditorForm.LinkText;
                if (originalLinkKmlTemplate != updatedLinkKmlTemplate)
                {
                    Console.WriteLine($"updatedLinkKmlTemplate({updatedLinkKmlTemplate})");
                    liveCam.Link.Template = updatedLinkKmlTemplate;
                    changeList.Add("Link");
                }

                if (changeList.Count > 0)
                {
                    MessageBox.Show(
                        $"Live Camera Definition ({string.Join(", ", changeList)} KML) was Changed",
                        "Live Camera Update",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

            }

        }

        private void geLinkBT_Click(object sender, EventArgs e)
        {
            KmlLiveCam liveCam;
            string liveCamUrl = LiveCameraHostPortTB.Text;
            try
            {
                liveCam = liveCamRegistry.LoadByUrl(liveCamUrl);
            } catch(UriFormatException ufe)
            {
                displayError("Could not Install Link", malformedUriErrorMessage(liveCamUrl, ufe));
                return;
            }

            string liveCamAliasQualifier = (
                Path.GetInvalidFileNameChars()
                .Aggregate(liveCam.Link.Values.alias, (current, c) => current.Replace(c, '_'))
            );
            string linkFileName = Path.GetTempPath() + $"FS2020PlanePath-kmllink-{liveCamAliasQualifier}.kml";
            try
            {
                File.WriteAllText(linkFileName, liveCam.Link.Render());
            }
            catch (Exception ex)
            {
                displayError("Could not save Network Link file", ex.Message);
                return;
            }
            Console.WriteLine($"Installing Link via({linkFileName})");
            try
            {
                using (Process installLinkProcess = Process.Start(linkFileName))
                {
                    if (!installLinkProcess.WaitForExit(3000))
                    {
                        //displayError("Timeout Waiting for Link Installation", "Is Google Earth Properly Installed?");
                        // NOTE: this happens when handling application is not already running (i.e., is started here)
                        Console.WriteLine($"NOTE: timeout waiting for application started to handle({linkFileName})");
                    } else
                    {
                        int exitCode = installLinkProcess.ExitCode;
                        if (exitCode != 0)
                        {
                            Console.WriteLine($"exitCode({exitCode})");
                            displayError("Unexpected Result while Installing Network Link", $"Exit Code {exitCode}");
                        }
                    }
                }
            } catch(Exception ilpe)
            {
                displayError("Error Installing Network Link", ilpe.Message);
            }
        }

        private string malformedUriErrorMessage(string url, UriFormatException ufe)
        {
            return $"Malformed URI: {url}.\n\nDetails: {ufe.Message}\n\nTry e.g.: 'http://localhost:8000/kmlcam'";
        }

        private void displayError(string caption, string details)
        {
            MessageBox.Show($"Details:\n{details}", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }

}