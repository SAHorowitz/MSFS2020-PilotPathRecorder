using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
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
        const string EXPORT_KMLFILE_CAPTION = "Export KML File";

        bool bLoggingEnabled = false;
        FlightDataConnector flightDataConnector;
        ScKmlAdapter scKmlAdapter;
        LiveCamRegistry liveCamRegistry;
        LiveCamServer liveCamServer;
        FS2020_SQLLiteDB FlightPathDB;
        int nCurrentFlightID;
        DateTime dtLastDataRecord;
        FlightPlan flightPlan;
        FlightLoggingOrchestrator flightLogOrchestrator;


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

            liveCamRegistry = new LiveCamRegistryFactory().NewRegistry();
            LoadLiveCams();

            scKmlAdapter = new ScKmlAdapter(CreateKmlParameterValues());
            liveCamServer = new LiveCamServer(scKmlAdapter, liveCamRegistry);

            flightDataConnector = CreateFlightDataConnector();
            flightLogOrchestrator = CreateFlightLogOrchestrator();
        }

        private KmlCameraParameterValues CreateKmlParameterValues()
        {
            KmlCameraParameterValues kmlCameraParameterValues = new KmlCameraParameterValues();
            kmlCameraParameterValues.listenerUrl = LiveCamServer.LiveCamUrl();
            kmlCameraParameterValues.liveCamUriPath = LiveCamServer.LIVECAM_URLPATH_SEGMENTS;
            kmlCameraParameterValues.getMultitrackUpdates = getKmlCameraUpdatesFromDb;
            return kmlCameraParameterValues;
        }

        private KmlCameraParameterValues[] getKmlCameraUpdatesFromDb(int flightId, long seqSince)
        {
            Console.WriteLine($"fetching camera updates for flight #{flightId} from timestamp({seqSince})");

            List<FlightPathData> flightPaths = FlightPathDB.GetFlightPathSinceTimestamp(flightId, seqSince);

            KmlCameraParameterValues[] kmlCameraUpdates = new KmlCameraParameterValues[flightPaths.Count];
            int cameraIndex = 0;
            foreach (var fp in flightPaths)
            {
                KmlCameraParameterValues newCameraParameterValues = scKmlAdapter.KmlCameraValues.ShallowCopy();

                newCameraParameterValues.seq = fp.timestamp;
                newCameraParameterValues.altitude = (fp.altitude / 3.28084) + 0.5;
                newCameraParameterValues.longitude = fp.longitude;
                newCameraParameterValues.latitude = fp.latitude;
                newCameraParameterValues.tilt = fp.plane_pitch;
                newCameraParameterValues.roll = fp.plane_bank;
                newCameraParameterValues.heading = fp.plane_heading_true;

                kmlCameraUpdates[cameraIndex++] = newCameraParameterValues;
            }

            return kmlCameraUpdates;
        }

        private FlightDataConnector CreateFlightDataConnector()
        {
            Action<FlightDataStructure> flightDataHandler = flightData => HandleFlightData(flightData);
            Action<EnvironmentDataStructure> environmentDataHandler = environmentData => HandleEnvironmentData(environmentData.title);
            FlightDataConnector flightDataConnector = (
                new FlightDataConnectorBuilder()
                .withConnectorFactory(
                    simConnectRB.Text,
                    () => new SimConnectFlightDataConnector(
                        this,
                        flightDataHandler,
                        environmentDataHandler
                    )
                )
                .withConnectorFactory(
                    replayRB.Text,
                    () => new GeneratedFlightDataConnector(
                        this,
                        flightDataHandler,
                        environmentDataHandler,
                        new ReplayFlightDataGenerator(FlightPathDB, GetFlightReplayGeneratorContext())
                    )
                )
                .withConnectorFactory(
                    randomWalkRB.Text,
                    () => new GeneratedFlightDataConnector(
                        this,
                        flightDataHandler,
                        environmentDataHandler,
                        new FlightDataGenerator(  
                            "RandomWalk",
                            new FlightPathData  // TODO - let user set these per invocation
                            {
                                timestamp = DateTime.Now.Ticks,
                                longitude = -121.6601805,
                                latitude = 38.0282797,
                                altitude = 3500,
                                plane_heading_true = 200,
                                ground_velocity = 120
                            },
                            new RandomWalkFlightAdvancer(
                                new RandomWalkFlightAdvancerParameters() // TODO - let user set these per invocation
                                {
                                    SamplesPerSecond = 2,      // at 2 samples per second; 
                                    HeadingChangeScale = 1.5,  // 3 degrees per second
                                    VelocityChangeScale = 0.5, // 1 knot per second
                                    AltitudeChangeScale = 10,  // 20' per second => 1200 fpm
                                    RollingCount = 5           // rolling average of 5 samples
                                }
                            )
                            .advance
                        )
                    )
                )
                .build()
            );
            return flightDataConnector;
        }

        private (int flightNo, string flightName, int segmentDurationSecs) GetFlightReplayGeneratorContext()
        {
            const string FLIGHT_TO_REPLAY_CAPTION = "Flight To Replay";

            ListViewItem listViewItem;
            if (!getSelectedFlight(out listViewItem, FLIGHT_TO_REPLAY_CAPTION))
            {
                throw new Exception($"no flight is selected");
            }

            int segmentDurationSecs;
            try
            {
                segmentDurationSecs = Convert.ToInt32(ThresholdLogWriteFreqTB.Text);
            } catch(Exception e)
            {
                UserDialogUtils.displayMessage(FLIGHT_TO_REPLAY_CAPTION, "Please set a threshold frequency");
                throw new Exception($"no threshold frequency: {e.Message}");
            }

            if (segmentDurationSecs <= 0)
            {
                // TODO fix this arbitrary correction
                Console.WriteLine($"fallback to 1 from from segmentDurationSecs = {segmentDurationSecs}");
                segmentDurationSecs = 1;
            }

            return (
                (int) listViewItem.Tag,
                $"{listViewItem.SubItems[1].Text} from {listViewItem.SubItems[0].Text}", 
                segmentDurationSecs
            );
        }

        private FlightLoggingOrchestrator CreateFlightLogOrchestrator()
        {
            FlightLoggingOrchestrator orchestrator = new FlightLoggingOrchestrator(
                new MultiButtonStateModel<ToggleState>(StartLoggingBtn, true, ToggleState.Out, "Start", "Stop"),
                new MultiButtonStateModel<ToggleState>(PauseLoggingBtn, false, ToggleState.Out, "Pause", "Resume"),
                s => { },
                s => {
                    bLoggingEnabled = true;
                    loggingStatusLB.Text = (
                        s == FlightLoggingOrchestrator.TriggerSource.StartStop
                      ? $"Logging activated at {DateTime.Now}."
                      : $"Logging resumed at {DateTime.Now}."
                    );
                },
                s => {
                    bLoggingEnabled = false;
                    loggingStatusLB.Text = (
                        s == FlightLoggingOrchestrator.TriggerSource.StartStop
                      ? $"Logging stopped at {DateTime.Now}."
                      : $"Logging paused at {DateTime.Now}."
                    );
                },
                s => StopLoggingAction()
            );

            orchestrator.IsAutomatic = AutomaticLoggingCB.Checked;

            return orchestrator;
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
            if (FolderSelectDialog1.Show(Handle))
            {
                KMLFilePathTBRO.Text = FolderSelectDialog1.FileName;
            }
        }

        private void MainPage_Shown(object sender, EventArgs e)
        {
            string sAppLatestVersion;

            sAppLatestVersion = ReadLatestAppVersionFromWeb();
            if (sAppLatestVersion.Equals(Program.sAppVersion) == false)
                if (MessageBox.Show("There is a newer version of the application available. Do you wish to download it now?", "New Version Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Process.Start($"https://github.com/{sourceRepo}");
            AttemptSimConnection(simConnectRB.Text);
            StartLoggingBtn.Enabled = true;
            nCurrentFlightID = 0;
        }

        private void AttemptSimConnection(string operationalMode)
        {
            flightDataConnector.SetMode(operationalMode);
            flightDataConnector.Connect();
            flightLogOrchestrator.Reset();
            UpdateConnectionDialogStatus();
        }

        protected override void DefWndProc(ref Message m)
        {
            if (!flightDataConnector.HandleWindowMessage(ref m))
            {
                base.DefWndProc(ref m);
            }
        }

        private void StartFlightLoggingToggleBtn_Click(object sender, EventArgs e)
        {
            if (flightLogOrchestrator.StartButton.State == ToggleState.Out)
            {
                flightLogOrchestrator.Start();
            } else
            {
                flightLogOrchestrator.Stop();
            }
            // set the last time a record was written to mintime
            dtLastDataRecord = DateTime.MinValue;
        }

        private void PauseFlightLoggingToggleBtn_Click(object sender, EventArgs e)
        {
            if (flightLogOrchestrator.PauseButton.State == ToggleState.Out)
            {
                flightLogOrchestrator.Pause();
            }
            else
            {
                flightLogOrchestrator.Resume();
            }
        }

        private void StopLoggingAction()
        {
            if (nCurrentFlightID == 0)
            {
                loggingStatusLB.Text = "";
                return;
            }

            FlightPathDB.WriteFlightPlan(nCurrentFlightID, flightPlan.flight_waypoints);
            loggingStatusLB.Text = $"Flight #{nCurrentFlightID} logged.";

            LoadFlightList();
            if (Program.bLogErrorsWritten == true)
                ErrorTBRO.Text = "Errors detected.  Please see " + Program.ErrorLogFile() + " for more details";
            else
                ErrorTBRO.Text = "";

            nCurrentFlightID = 0;
        }

        // function is called from the retrieval of information from the simconnect and in this case stores it in the database based 
        // on prefrences
        public void HandleFlightData(FlightDataStructure simPlaneData)
        {

            // logging mode according to ground velocity
            int userLoggingThresholdGroundVelocity = Parser.Convert(
                LoggingThresholdGroundVelTB.Text,
                s => Convert.ToInt32(s),
                () => FS2020_SQLLiteDB.DEFAULT_AUTOMATIC_LOGGING_THRESHOLD
            );

            if (simPlaneData.ground_velocity >= userLoggingThresholdGroundVelocity)
            {
                flightLogOrchestrator.ThresholdReached();
            }
            else
            {
                flightLogOrchestrator.ThresholdMissed();
            }

            // the value of this flag is managed through 'flightLogOrchestrator'
            if (bLoggingEnabled)     
            {
                // if we don't have flight header information then ask for it and don't write out this data point
                if (nCurrentFlightID == 0)
                {
                    flightDataConnector.GetSimEnvInfo();
                }
                else
                {
                    System.TimeSpan tsDiffRecords = DateTime.Now - dtLastDataRecord;

                    // if altitude above ground is greater or equal threshold and it has been threshold amount of time or
                    //    altitude is below threshold
                    int userThresholdMininumAltitude = Parser.Convert(
                        ThresholdMinAltTB.Text,
                        s => Convert.ToInt32(ThresholdMinAltTB.Text),
                        () => FS2020_SQLLiteDB.DEFAULT_THRESHOLD_MIN_ALT
                    );
                    double userThresholdLogWriteFrequency = Parser.Convert(
                        ThresholdLogWriteFreqTB.Text,
                        s => Convert.ToDouble(ThresholdLogWriteFreqTB.Text),
                        () => FS2020_SQLLiteDB.DEFAULT_ABOVE_THRESHOLD_WRITE_FREQ
                    );
                    if (((simPlaneData.altitude_above_ground >= userThresholdMininumAltitude) &&
                         (tsDiffRecords.TotalSeconds >= userThresholdLogWriteFrequency)) ||
                        (simPlaneData.altitude_above_ground < userThresholdMininumAltitude))
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

            //Console.WriteLine($"scUpdate latLon({simPlaneData.latitude}, {simPlaneData.longitude})");
            scKmlAdapter.Update(simPlaneData, nCurrentFlightID, dtLastDataRecord.Ticks);
        }

        // function is called from the retrieval of information from the simconnect and hold the aircraft name info
        public void HandleEnvironmentData(string aircraft)
        {
            if (bLoggingEnabled == true)
            {
                nCurrentFlightID = FlightPathDB.WriteFlight(aircraft);
                Console.WriteLine($"set nCurrentFlightID({nCurrentFlightID})");
            }
        }

        private bool getSelectedFlight(out ListViewItem listViewItem, string caption)
        {
            if (FlightPickerLV.SelectedItems.Count != 1)
            {
                UserDialogUtils.displayMessage(caption, "Please choose a flight.", MessageBoxIcon.Question);
                listViewItem = default(ListViewItem);
                return false;
            }
            listViewItem = FlightPickerLV.SelectedItems[0];
            return true;
        }

        /// <returns>name of the Kml file created from the log</returns>
        private bool createKmlFileFromLog(out string kmlFileName)
        {
            int nCount;
            int nFlightID;
            long lprevTimestamp;
            ListViewItem selectedFlight;

            kmlFileName = default(string);
            //UpdateLoggingEnabledState(false, false);  // TODO investigate why this was here - why would it matter?

            if (!getSelectedFlight(out selectedFlight, EXPORT_KMLFILE_CAPTION))
            {
                return false;
            }
            nFlightID = (int)selectedFlight.Tag;


            if (KMLFilePathTBRO.Text.Length == 0)
            {
                UserDialogUtils.displayMessage(EXPORT_KMLFILE_CAPTION, "Please choose a folder location.");
                return false;
            }

            // This is the root element of the file
            var kml = new Kml();
            Folder mainFolder = new Folder();
            mainFolder.Name = GetSavedFlightName(FlightPickerLV.SelectedItems[0]);
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
            string sfilename = string.Concat(KMLFilePathTBRO.Text, "\\", GetSavedFlightFileName(FlightPickerLV.SelectedItems[0]));

            System.IO.File.Delete(sfilename);
            KmlFile kmlfile = KmlFile.Create(kml, true);
            using (var stream = System.IO.File.OpenWrite(sfilename))
            {
                kmlfile.Save(stream);
            }
            kmlFileName = sfilename;
            return true;
        }

        private static string GetSavedFlightName(ListViewItem lvi)
        {
            return $"{lvi.SubItems[1].Text} {lvi.SubItems[0].Text}";
        }

        private static string GetSavedFlightFileName(ListViewItem lvi)
        {
            string rawFileName = $"{lvi.SubItems[1].Text}_{lvi.SubItems[0].Text}.kml";
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            return new string(rawFileName.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
        }

        // function that writes out KML file based on the flight chosen by the user
        private void CreateKMLButton_Click(object sender, EventArgs e)
        {
            string exportedKmlFilename;
            if (createKmlFileFromLog(out exportedKmlFilename))
            {
                UserDialogUtils.displayMessage(
                    EXPORT_KMLFILE_CAPTION, 
                    String.Format($"Flight successfully exported to {exportedKmlFilename}")
                );
            }
        }

        // function that writes out and opens the KML file based on the flight chosen by the user
        private void LaunchKmlFileBtn_Handler(object sender, EventArgs e)
        {
            string exportedKmlFilename;
            if (createKmlFileFromLog(out exportedKmlFilename))
            {
                LaunchFile(exportedKmlFilename, 3000);
            }
        }

        private void LoadLiveCams() {
            LiveCameraHostPortCB.Items.Clear();
            List<string> liveCamAliases = liveCamRegistry.GetIds(1000);
            if (liveCamAliases.Count == 0)
            {
                return;
            }
            List<string> liveCamUrls = new List<string>();
            foreach (var alias in liveCamAliases) {
                liveCamUrls.Add(LiveCamServer.LiveCamLensSpecToUrl(alias, ""));
            }
            string[] liveCamUrlsArray = liveCamUrls.ToArray();
            LiveCameraHostPortCB.Text = liveCamUrlsArray[0];
            LiveCameraHostPortCB.Items.AddRange(liveCamUrlsArray);
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

            ListView.SelectedListViewItemCollection selectedItems = FlightPickerLV.SelectedItems;
            if (selectedItems.Count < 1)
            {
                UserDialogUtils.displayError("Delete Flight(s)", "Please choose the flight(s) to delete.");
                return;
            }

            IEnumerable<ListViewItem> flightsToBeDeleted = selectedItems.Cast<ListViewItem>();
            if (
                UserDialogUtils.obtainConfirmation(
                    "Delete Flight(s)",
                    $"Flight(s) to be deleted:\n\n" +
                    $"- {string.Join("\n- ", flightsToBeDeleted.Select(f => GetSavedFlightName(f)).ToArray())}\n\n" +
                    "NOTE: Deleting a flight from the database cannot be undone.\n\n" +
                    "Are you sure you want to proceed?",
                    MessageBoxIcon.Exclamation
                )
            ) {
                foreach (ListViewItem flight in flightsToBeDeleted)
                {
                    int flightID = (int) flight.Tag;
                    string flightName = GetSavedFlightName(flight);
                    FlightPathDB.DeleteFlight(flightID);
                    Console.WriteLine($"deleted flightNo({flightID}); {flightName}");
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
            flightDataConnector.CloseConnection();
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
            flightLogOrchestrator.IsAutomatic = AutomaticLoggingCB.Checked;
        }

        private void LiveCameraCB_CheckedChanged(object sender, EventArgs eventArgs)
        {
            liveCamServer.Stop();

            if (!LiveCameraCB.Checked)
            {
                ErrorTBRO.Text = "NOTE: Live Camera Listener Deactivated";
                return;
            }

            // start a new live camera listener using supplied URI
            Uri hostUri = null;
            string problemMessage = null;
            string liveCamUrl = LiveCameraHostPortCB.Text;
            try
            {
                hostUri = new Uri(liveCamUrl);
                // ensure live cam is registered
                liveCamRegistry.LoadByAlias(LiveCamServer.LiveCamUrlToLensSpec(liveCamUrl).alias);
            }
            catch (UriFormatException ufe)
            {
                problemMessage = malformedUriErrorMessage(liveCamUrl, ufe);
            }

            if (problemMessage == null)
            {
                try
                {
                    liveCamServer.Start(hostUri);
                    scKmlAdapter.KmlCameraValues.listenerUrl = hostUri.GetLeftPart(UriPartial.Authority);
                }
                catch (SystemException ene)
                {
                    problemMessage = $"Could not listen on: {hostUri}.\n\nDetails: {ene.Message}";
                }
            }

            if (problemMessage != null)
            {
                UserDialogUtils.displayError("Could not start Live Camera", $"{problemMessage}");
                LiveCameraCB.CheckedChanged -= LiveCameraCB_CheckedChanged;
                LiveCameraCB.Checked = false;
                LiveCameraCB.CheckedChanged += LiveCameraCB_CheckedChanged;
                return;
            }

            ErrorTBRO.Text = "NOTE: Live Camera Listener Activated"; 
        }

        private void LiveCameraKml_Click(object sender, EventArgs e)
        {
            string liveCamUrl = LiveCameraHostPortCB.Text;
            string alias, lensName;

            try
            {
                (alias, lensName) = LiveCamServer.LiveCamUrlToLensSpec(liveCamUrl);
            }
            catch (UriFormatException ufe)
            {
                UserDialogUtils.displayError("Could not Invoke LiveCam Editor", malformedUriErrorMessage(liveCamUrl, ufe));
                return;
            }

            KmlLiveCam liveCam = liveCamRegistry.LoadByAlias(alias);
            using (
                KmlLiveCamEditorForm kmlEditorForm = new KmlLiveCamEditorForm(
                    alias, 
                    lensName, 
                    liveCam,
                    lcfn => loadLiveCamFromFile(lcfn),
                    (lcfn, lc) => saveLiveCamToFile(lcfn, lc),
                    a => !liveCamRegistry.TryGetById(a, out _)
                )
            )
            {

                if (kmlEditorForm.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                KmlLiveCam updatedKmlLiveCam = kmlEditorForm.KmlLiveCam;
                if (!updatedKmlLiveCam.Equals(liveCam) || alias != kmlEditorForm.Alias)
                {
                    liveCamRegistry.Save(kmlEditorForm.Alias, updatedKmlLiveCam);
                    UserDialogUtils.displayMessage(
                        "Live Camera Update", 
                        $"Live Camera '{kmlEditorForm.Alias}' Definition was Saved"
                    );
                    if (alias != kmlEditorForm.Alias)
                    {
                        LoadLiveCams();
                    }
                }

            }

        }

        private void saveLiveCamToFile(string liveCamFilename, KmlLiveCam lc)
        {
            if (
                !FilesystemSerializer.TrySerializeToFile(
                    liveCamFilename, 
                    new JsonSerializer<LiveCamEntity>(), 
                    new LiveCamEntity(lc)
                )
            )
            {
                throw new Exception($"Could not save KmlLiveCam to '{liveCamFilename}'");
            }
        }

        private KmlLiveCam loadLiveCamFromFile(string liveCamFilename)
        {
            LiveCamEntity liveCamEntity;
            if (
                FilesystemSerializer.TryDeserializeFromFile(
                    liveCamFilename, 
                    new JsonSerializer<LiveCamEntity>(), 
                    out liveCamEntity
                )
            ) {
                return new KmlLiveCam(liveCamEntity);
            }
            throw new Exception($"Could not load KmlLiveCam from '{liveCamFilename}'");
        }

        private void geLinkBT_Click(object sender, EventArgs e)
        {
            string alias, lensName;
            string liveCamUrl = LiveCameraHostPortCB.Text;
            try
            {
                (alias, lensName) = LiveCamServer.LiveCamUrlToLensSpec(liveCamUrl);
            } catch(UriFormatException ufe)
            {
                UserDialogUtils.displayError("Could not Install Link", malformedUriErrorMessage(liveCamUrl, ufe));
                return;
            }

            KmlLiveCam liveCam = liveCamRegistry.LoadByAlias(alias);
            string liveCamAliasQualifier = (
                Path.GetInvalidFileNameChars()
                .Aggregate(alias, (current, c) => current.Replace(c, '_'))
            );
            string linkFileName = Path.GetTempPath() + $"FS2020PlanePath-kmllink-{liveCamAliasQualifier}.kml";
            IStringTemplateRenderer<KmlCameraParameterValues> stringTemplateRenderer = liveCam.GetLens(lensName);
            KmlCameraParameterValues linkKmlCameraParameterValues = scKmlAdapter.KmlCameraValues.ShallowCopy();
            linkKmlCameraParameterValues.alias = alias;
            linkKmlCameraParameterValues.lens = lensName;
            try
            {
                File.WriteAllText(
                    linkFileName,
                    stringTemplateRenderer.Render(linkKmlCameraParameterValues)
                );
            }
            catch (Exception ex)
            {
                UserDialogUtils.displayError("Could not save Network Link file", ex.Message);
                return;
            }

            LaunchFile(linkFileName, 3000);

        }

        private void LaunchFile(string launchFilename, int timeoutMillis)
        {
            Console.WriteLine($"launching({launchFilename})");
            try
            {
                using (Process installLinkProcess = Process.Start(launchFilename))
                {
                    if (!installLinkProcess.WaitForExit(timeoutMillis))
                    {
                        // NOTE: this seems to happen always when handling application (e.g., Google Earth) is not already running
                        Console.WriteLine($"NOTE: timeout waiting for '{launchFilename}' to launch");
                    }
                    else
                    {
                        int exitCode = installLinkProcess.ExitCode;
                        if (exitCode != 0)
                        {
                            Console.WriteLine($"exitCode({exitCode})");
                            UserDialogUtils.displayError(
                                "Launch Error",
                                $"launching '{launchFilename}' produced exit code '{exitCode}'"
                            );
                        }
                    }
                }
            }
            catch (Exception ilpe)
            {
                UserDialogUtils.displayError(
                    "Launch Error",
                    $"launching '{launchFilename}' produced exception '{ilpe.Message}'"
                );
            }
        }

        private void ValidateNetworkLink(object sender, CancelEventArgs e)
        {
            string liveCamUrl = LiveCameraHostPortCB.Text;
            try
            {
                new Uri(liveCamUrl);
            } catch(UriFormatException ufe)
            {
                UserDialogUtils.displayError(
                    "Invalid Network Link", 
                    malformedUriErrorMessage(liveCamUrl, ufe)
                );
            }            
        }

        private void Handle_LiveCameraKmlResetBT_Click(object sender, EventArgs e)
        {
            KmlLiveCam liveCam;
            string liveCamUrl = LiveCameraHostPortCB.Text;
            string alias = LiveCamServer.LiveCamUrlToLensSpec(liveCamUrl).alias;
            if (!liveCamRegistry.TryGetById(alias, out liveCam))
            {
                return;
            }
            
            if (!liveCamRegistry.IsDefaultDefinition(liveCam, alias))
            {
                if (
                    !UserDialogUtils.obtainConfirmation(
                        "Confirm Reset", 
                        $"Discard changes for:\n{liveCamUrl}"
                    )
                )
                {
                    return;
                }
            }

            liveCamRegistry.Delete(alias);
            LoadLiveCams();
        }

        private string malformedUriErrorMessage(string url, UriFormatException ufe)
        {
            return $"Malformed URI: {url}.\n\nDetails: {ufe.Message}\n\nTry e.g.: 'http://localhost:8000/kmlcam/cockpit'";
        }

        private void RetrySimConnectionBtn_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Text == DISCONNECT_BUTTON_TEXT)
            {
                flightDataConnector.CloseConnection();
                UpdateConnectionDialogStatus();
                return;
            }
            AttemptSimConnection(
                connectionTypeGB
                .Controls
                .OfType<RadioButton>()
                .FirstOrDefault(r => r.Checked)
                .Text
            );
        }

        private void HandleConnectionTypeChangeEvent(object sender, EventArgs e)
        {

            RadioButton changedButton = (RadioButton) sender;

            string changedMode = changedButton.Text;
            if (changedButton.Checked)
            {
                // set the new mode
                flightDataConnector.SetMode(changedMode);
                UpdateConnectionDialogStatus();
                return;
            }

            // unset the old mode
            if (flightDataConnector.IsSimConnected())
            {
                Debug.Assert(flightDataConnector.Mode == changedMode);
                if (
                    !UserDialogUtils.obtainConfirmation(
                        "Confirm Connection Switch",
                        $"OK to Close {flightDataConnector.Mode}?"
                    )
                )
                {
                    // user didn't approve the change, so change the
                    // button back to reflect the active connection type
                    changedButton.Checked = true;
                    return;
                }
                flightDataConnector.CloseConnection();
                UpdateConnectionDialogStatus();
            }

        }

        private void UpdateConnectionDialogStatus()
        {
            bool isConnected = flightDataConnector.IsSimConnected();
            string connectionState = isConnected ? "Connected" : "Not Connected";
            if (flightDataConnector.Diagnostics.Length > 0)
            {
                connectionState = connectionState + $" ({string.Join(",", flightDataConnector.Diagnostics)})";
            }

            SimConnectStatusLabel.Text = $"{flightDataConnector.Mode}: {connectionState}";
            RetrySimConnectionBtn.Text = isConnected ? DISCONNECT_BUTTON_TEXT : CONNECT_BUTTON_TEXT;

            toolTip1.SetToolTip(this.SimConnectStatusLabel, SimConnectStatusLabel.Text);
        }

        private const string CONNECT_BUTTON_TEXT = "Connect";
        private const string DISCONNECT_BUTTON_TEXT = "Disconnect";

    }

}