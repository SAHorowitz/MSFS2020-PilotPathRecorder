using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace FS2020PlanePath
{
    public partial class MainPage : Form
    {
        bool bLoggingEnabled = false;
        MSFS2020_SimConnectIntergration simConnectIntegration = new MSFS2020_SimConnectIntergration();
        FS2020_SQLLiteDB FlightPathDB;
        int nCurrentFlightID;
        DateTime dtLastDataRecord;
        FlightPlan flightPlan;
        bool bStartedLoggingDueToSpeed;
        bool bStoppedLoggingDueToSpeed;
        Timer CheckSimConnectionTimer = new Timer();

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
            KMLFileNameTBRO.Text = GetFilenameFromTemplate(0);
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

            if (string.Compare(FlightPathDB.GetTableOption("AutomaticLoggingStopEngineOff"), "true") == 0)
                LoggingThresholdEngineOffCB.Checked = true;
            else
                LoggingThresholdEngineOffCB.Checked = false;

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
            string sKMLFileNameToolTip;

            simConnectIntegration.FForm = this;
            System.Windows.Forms.ToolTip KMLFileNameToolTip = new System.Windows.Forms.ToolTip();
            System.Windows.Forms.ToolTip AutomaticLoggingEngineOffToolTip = new System.Windows.Forms.ToolTip();

            sKMLFileNameToolTip =  "Template format: key fields need to be surrounded by { }\r\nValid key field values are:\r\n";
            sKMLFileNameToolTip += "Plane.Name = name of the plane\r\n";
            sKMLFileNameToolTip += "Loc.Start.Ident = closest airport identification at start of flight\r\n";
            sKMLFileNameToolTip += "Loc.End.Ident = closest airport identification at end of flight\r\n";
            sKMLFileNameToolTip += "Loc.Start.Name = closest airport name at start of flight\r\n";
            sKMLFileNameToolTip += "Loc.End.Name = closest airport name at end of flight\r\n";
            sKMLFileNameToolTip += "Loc.Start.City = closest airport city at start of flight\r\n";
            sKMLFileNameToolTip += "Loc.End.City = closest airport city at end of flight\r\n";
            sKMLFileNameToolTip += "Loc.Start.State = closest airport state at start of flight\r\n";
            sKMLFileNameToolTip += "Loc.End.State = closest airport state at end of flight\r\n";
            sKMLFileNameToolTip += "Flight.Start.Timestamp:timeformat = computer date and time when flight started\r\n";
            sKMLFileNameToolTip += "Flight.Start.Timestamp:timeformat = computer date and time when flight ended\r\n";
            sKMLFileNameToolTip += "timeformat details can be found by searching 'Standard date and time format strings' on the web\r\n\r\n";
            sKMLFileNameToolTip += "Non key fields (plain text or symbols) can also be placed throughout the KML File Name as well";

            KMLFileNameToolTip.SetToolTip(KMLFileNameTBRO, sKMLFileNameToolTip);
            KMLFileNameToolTip.SetToolTip(KMLFileNameGB, sKMLFileNameToolTip);
            KMLFileNameToolTip.SetToolTip(KMLFileNameTemplateTB, sKMLFileNameToolTip);
            AutomaticLoggingEngineOffToolTip.SetToolTip(LoggingThresholdEngineOffCB, "Leave Unchecked To Stop Automatic Logging By Ground Velocity Only");

            KMLFileNameTemplateTB.Text = FlightPathDB.GetTableOption("KMLFileNameTemplate");

            // Load Up Airport Info if needed
            string pattern = "AirportListv*.csv";
            var dirInfo = new DirectoryInfo(".\\"); 
            try
            {
                var file = (from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f).First();
                if (Int32.Parse(FlightPathDB.GetAirportListVersion(file.Name)) > Int32.Parse(FlightPathDB.GetTableOption("AirportListVersion")))
                    FlightPathDB.LoadUpAirportInfo(file.Name);

                // See if there is a newer version availble on the web
                sAppLatestVersion = ReadLatestAppVersionFromWeb();
                if (sAppLatestVersion.Equals(Program.sAppVersion) == false)
                    if (MessageBox.Show("There is a newer version of the application available. Do you wish to download it now?", "New Version Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        System.Diagnostics.Process.Start("https://github.com/SAHorowitz/MSFS2020-PilotPathRecorder");
                AttemptSimConnection();
                nCurrentFlightID = 0;
                bStartedLoggingDueToSpeed = false;
                bStoppedLoggingDueToSpeed = true;

                // Setup SimConnection Timer
                CheckSimConnectionTimer.Interval = 5000; // 5 seconds
                CheckSimConnectionTimer.Tick += new EventHandler(CheckSimConnection_Tick);
                CheckSimConnectionTimer.Start();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging("Error finding or parsing AirportListvx.csv where x is a number.  Please ensure file is in directory and try again", ex);
                MessageBox.Show("Errors detected.  Please see " + Program.ErrorLogFile() + " for more details. Program will now end.", "Fatal Error");
                System.Windows.Forms.Application.Exit();
            }
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

        // Specify what you want to happen when the Elapsed event is raised.
        private void CheckSimConnection_Tick(object sender, EventArgs e)
        {
            if (simConnectIntegration.IsSimConnected() == false)
                AttemptSimConnection();
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
                        // if ground speed is < LoggingThresholdGroundVelTB and user wanted automatic logging and logging was on due to speed then check if it should be turned off
                        if (bStartedLoggingDueToSpeed == true)
                            // if user wanted to only turn off if all engines off and all engines are off OR user wanted it based on speed only
                            if (((LoggingThresholdEngineOffCB.Checked == true) && (simPlaneData.engine1rpm == 0) && (simPlaneData.engine2rpm == 0) && 
                                (simPlaneData.engine3rpm == 0) && (simPlaneData.engine4rpm == 0)) ||
                                (LoggingThresholdEngineOffCB.Checked == false))
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

        // user desires to export to KML
        private void CreateKMLButton_Click(object sender, EventArgs e)
        {
            int nCount = 0;
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

            // Export passing the FlightID
            foreach (int nSelectedItem in FlightPickerLV.SelectedIndices)
            {
                if (ExportKML((int)FlightPickerLV.Items[nSelectedItem].Tag) == false)
                    break;
                nCount++;
            }

            DialogResult dialogResult = MessageBox.Show(String.Format("{0} flight(s) successfully exported.  Do you wish to open the exported KML folder?", nCount.ToString()), "Export KML File", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = KMLFilePathTBRO.Text; 
                startInfo.FileName = "explorer.exe";

                Process.Start(startInfo);
            }
        }

        // function that writes out KML file based on the flight chosen by the user         
        private bool ExportKML(int nFlightID)
        {
            int nCount;
            string sfilename;
            long lprevTimestamp;
            FlightListData FlightData;
            bool bFileSavedOK;

            // Get Flight Info
            FlightData = FlightPathDB.GetFlight(nFlightID);

            // should never happen but just in case
            if (FlightData == null)
            {
                MessageBox.Show("Error getting data for that flight.", "Export KML File Failure");
                bFileSavedOK = false;
                return bFileSavedOK;
            }

            // This is the root element of the file
            var kml = new Kml();
            Folder mainFolder = new Folder();
            mainFolder.Name = GetFilenameFromTemplate(nFlightID); 

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

                    // verify that waypoint name is a valid string
                    Regex r = new Regex(@"[^\w\.@-]");
                    if (r.IsMatch(waypointData.gps_wp_name))
                        waypointData.gps_wp_name = Program.sInvalidWaypointName;

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
            sfilename = GetFilenameFromTemplate(nFlightID);
            if (String.Compare(sfilename, "Filename Error") != 0)
            {
                sfilename = sfilename + ".kml"; 
                var validfilename = new string(sfilename.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
                sfilename = string.Concat(KMLFilePathTBRO.Text, "\\");
                sfilename += validfilename;

                System.IO.File.Delete(sfilename);
                KmlFile kmlfile = KmlFile.Create(kml, true);
                using (var stream = System.IO.File.OpenWrite(sfilename))
                {
                    kmlfile.Save(stream);
                }
                bFileSavedOK = true;
            }
            else
            {
                MessageBox.Show("Filename Template Invalid.", "Export KML File Failure");
                bFileSavedOK = false;
            }

            return bFileSavedOK;
        }

        // this function will parse the KMLFileNameTemplate and create filename.  Either example or real one
        private string GetFilenameFromTemplate(int nFlightID)
        {
            string sFilename = "";
            string sFileNameTemplate;
            string sPlaneName;
            string sStartIdent;
            string sEndIdent;
            string sStartName;
            string sEndName;
            string sStartCity;
            string sEndCity;
            string sStartState;
            string sEndState;
            DateTime dtStartFlight;
            DateTime dtEndFlight;
            int nStrPos;

            // if FlightID is 0 then it must be example as FlightID minimum value is 1
            if (nFlightID == 0)
            {
                sPlaneName = "Cessna Skyhawk G1000 Asobo";
                dtStartFlight = DateTime.Now.AddHours(-1);
                dtEndFlight = DateTime.Now;
                sStartIdent = "KATL";
                sEndIdent = "KHXD";
                sStartName = "Hartsfield-Jackson Atlanta Int Airport";
                sEndName = "Hilton Head";
                sStartCity = "Atlanta";
                sEndCity = "Hilton Head";
                sStartState = "Georgia";
                sEndState = "South Carolina";
            }
            else
            {
                FlightListData FlightData;
                AirportData apFirstPointData;
                AirportData apLastPointData;

                // Get Flight Info
                FlightData = FlightPathDB.GetFlight(nFlightID);
                FlightPathData FirstFlightPathData = FlightPathDB.GetFlightPathEntry(true, nFlightID);
                FlightPathData LastFlightPathData = FlightPathDB.GetFlightPathEntry(false, nFlightID);

                sPlaneName = FlightData.aircraft;
                dtStartFlight = new DateTime(FlightData.start_flight_timestamp);
                dtEndFlight = new DateTime(LastFlightPathData.timestamp);

                apFirstPointData = FlightPathDB.GetClosestAirportToLatLong(false, FirstFlightPathData.latitude, FirstFlightPathData.longitude);
                apLastPointData = FlightPathDB.GetClosestAirportToLatLong(false, LastFlightPathData.latitude, LastFlightPathData.longitude);
                
                sStartIdent = apFirstPointData.ident;
                sEndIdent = apLastPointData.ident;
                sStartName = apFirstPointData.name;
                sEndName = apLastPointData.name;
                sStartCity = apFirstPointData.city;
                sEndCity = apLastPointData.city;
                sStartState = apFirstPointData.state;
                sEndState = apLastPointData.state;
            }

            sFileNameTemplate = KMLFileNameTemplateTB.Text;

            nStrPos = 0;
            while ((nStrPos < sFileNameTemplate.Length) && (String.Compare(sFilename, "Filename Error") != 0))
            {
                if (sFileNameTemplate[nStrPos] == '{')
                {
                    int nEndBracketPos = 0;
                    nEndBracketPos = sFileNameTemplate.IndexOf('}', nStrPos);

                    if (nEndBracketPos < 0)
                        sFilename = "Filename Error";
                    else
                    {
                        string sTemplateElement;

                        sTemplateElement = sFileNameTemplate.Substring(nStrPos + 1, nEndBracketPos - (nStrPos + 1));

                        if (String.Compare(sTemplateElement, "Plane.Name", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sPlaneName;
                        else if (String.Compare(sTemplateElement, "Loc.Start.Ident", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sStartIdent;
                        else if (String.Compare(sTemplateElement, "Loc.End.Ident", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sEndIdent;
                        else if (String.Compare(sTemplateElement, "Loc.Start.Name", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sStartName;
                        else if (String.Compare(sTemplateElement, "Loc.End.Name", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sEndName;
                        else if (String.Compare(sTemplateElement, "Loc.Start.City", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sStartCity;
                        else if (String.Compare(sTemplateElement, "Loc.End.City", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sEndCity;
                        else if (String.Compare(sTemplateElement, "Loc.Start.State", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sStartState;
                        else if (String.Compare(sTemplateElement, "Loc.End.State", StringComparison.OrdinalIgnoreCase) == 0)
                            sFilename = sFilename + sEndState;
                        else
                        {
                            int nColonPos = 0;

                            nColonPos = sFileNameTemplate.IndexOf(':', nStrPos);
                            if (nColonPos >= 0)
                            {
                                sTemplateElement = sFileNameTemplate.Substring(nStrPos + 1, nColonPos - (nStrPos + 1));

                                if (String.Compare(sTemplateElement, "Flight.Start.Timestamp", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    sTemplateElement = sTemplateElement = sFileNameTemplate.Substring(nColonPos + 1, nEndBracketPos - (nColonPos + 1));
                                    sFilename = sFilename + dtStartFlight.ToString(sTemplateElement);
                                }
                                else if (String.Compare(sTemplateElement, "Flight.End.Timestamp", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    sTemplateElement = sTemplateElement = sFileNameTemplate.Substring(nColonPos + 1, nEndBracketPos - (nColonPos + 1));
                                    sFilename = sFilename + dtEndFlight.ToString(sTemplateElement);
                                }
                            }
                        }
                        nStrPos = nEndBracketPos + 1;
                    }
                }
                else // must be literal character
                {
                    sFilename = sFilename + sFileNameTemplate[nStrPos];
                    nStrPos++;
                }
            }
            return sFilename;
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

            FlightPathData FirstFlightPathData = FlightPathDB.GetFlightPathEntry(true, nCurrentFlightID);
            FlightPathData LastFlightPathData = FlightPathDB.GetFlightPathEntry(false, nCurrentFlightID);

            AirportData apFirstPointData = FlightPathDB.GetClosestAirportToLatLong(false, FirstFlightPathData.latitude, FirstFlightPathData.longitude);
            AirportData apLastPointData = FlightPathDB.GetClosestAirportToLatLong(false, LastFlightPathData.latitude, LastFlightPathData.longitude);

            // if there are no items in the flightplan then use start airport as first waypoint if within 2 miles
            if ((flightPlan.flight_waypoints.Count == 0) && (apFirstPointData.distance <= 2))
                    flightPlan.flight_waypoints.Add(new FlightWaypointData(FirstFlightPathData.latitude, FirstFlightPathData.longitude, FirstFlightPathData.altitude, apFirstPointData.ident));

            // if last item in the list item in the flightplan is not equal to closest and airport and it is within 1KM then write it as final waypoint
            if (((flightPlan.flight_waypoints.Count == 0) ||
                 (String.Compare(flightPlan.flight_waypoints.Last().gps_wp_name, apLastPointData.ident, StringComparison.OrdinalIgnoreCase) != 0)) && 
                (apLastPointData.distance <= 2))
                    flightPlan.flight_waypoints.Add(new FlightWaypointData(LastFlightPathData.latitude, LastFlightPathData.longitude, LastFlightPathData.altitude, apLastPointData.ident));

            FlightPathDB.WriteFlightPlan(nCurrentFlightID, flightPlan.flight_waypoints);
            flightPlan.flight_waypoints.Clear();

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
            if (MessageBox.Show("Deleting flights from the database cannot be undone.  Are you sure you want to delete?", "Delete a Flight", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (int nSelectedItem in FlightPickerLV.SelectedIndices)
                {
                    nFlightID = (int) FlightPickerLV.Items[nSelectedItem].Tag;
                    FlightPathDB.DeleteFlight(nFlightID);
                }
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
            FlightPathDB.WriteTableOption("KMLFileNameTemplate", KMLFileNameTemplateTB.Text);
            if (LoggingThresholdEngineOffCB.Checked == true)
                FlightPathDB.WriteTableOption("AutomaticLoggingStopEngineOff", "true");
            else
                FlightPathDB.WriteTableOption("AutomaticLoggingStopEngineOff", "false");
            CheckSimConnectionTimer.Stop();
            simConnectIntegration.CloseConnection();
        }

        private string ReadLatestAppVersionFromWeb()
        {
            string sRetVal = "";

            WebClient client = new WebClient();
            try
            {
                Stream stream = client.OpenRead("https://raw.githubusercontent.com/SAHorowitz/MSFS2020-PilotPathRecorder/master/docs/latest_version.txt");
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

        private void KMLFileNameTemplateTB_TextChanged(object sender, EventArgs e)
        {
            KMLFileNameTBRO.Text = GetFilenameFromTemplate(0);
        }
    }
}