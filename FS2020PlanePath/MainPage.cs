using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.FlightSimulator.SimConnect;
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
        long lCurrentFlightID;
        DateTime dtLastDataRecord;

        public MainPage()
        {
            InitializeComponent();
            FlightPathDB = new FS2020_SQLLiteDB();
            FlightPathDB.CreateTables();
            ThresholdLogWriteFreqTB.Text = FlightPathDB.GetTableOption("AboveThresholdWriteFreq");
            ThresholdMinAltTB.Text = FlightPathDB.GetTableOption("ThresholdMinAltitude");
            KMLFilePathTBRO.Text = FlightPathDB.GetTableOption("KMLFilePath");
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
            simConnectIntegration.FForm = this;
            AttemptSimConnection();
        }

        private void AttemptSimConnection()
        {
            if (simConnectIntegration.Connect() == true)
            {
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
                    catch
                    {
                        SimConnectStatusLabel.Text = "Connection lost to SimConnect";
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

            // if sim is connected then initialize the data flow or throw up an error
            if (simConnectIntegration.Initialize() == true)
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
        public void UseData(double latitude, double longitude, Int32 altitude, Int32 altitude_above_ground, Int32 engine1rpm,
                            Int32 engine2rpm, Int32 engine3rpm, Int32 engine4rpm, Int32 lightsmask, double ground_velocity,
                            double plane_pitch, double plane_bank, double plane_heading_true, double plane_heading_magnetic,
                            double plane_airspeed_indicated, double airspeed_true, double vertical_speed, double heading_indicator,
                            Int32 flaps_handle_position, Int32 spoilers_handle_position, bool gear_handle_position, 
                            double ambient_wind_velocity, double ambient_wind_direction, double ambient_temperature, bool stall_warning,
                            bool overspeed_warning, bool is_gear_retractable, bool spoiler_available)
        {
            if (bLoggingEnabled == true)
            {
                System.TimeSpan tsDiffRecords = DateTime.Now - dtLastDataRecord;

                // if altitude above ground is greater or equal threshold and it has been threshold amount of time or
                //    altitude is below threshold
                if (((altitude_above_ground >= Convert.ToInt32(ThresholdMinAltTB.Text)) &&
                     (tsDiffRecords.TotalSeconds >= Convert.ToDouble(ThresholdLogWriteFreqTB.Text))) ||
                    (altitude_above_ground < Convert.ToInt32(ThresholdMinAltTB.Text)))
                {
                    long FlightSampleID;

                    FlightSampleID = FlightPathDB.WriteFlightPoint(lCurrentFlightID, latitude, longitude, altitude);
                    FlightPathDB.WriteFlightPointDetails(FlightSampleID, altitude_above_ground, engine1rpm, engine2rpm, engine3rpm, 
                                                         engine4rpm, lightsmask, ground_velocity, plane_pitch, plane_bank, plane_heading_true,
                                                         plane_heading_magnetic, plane_airspeed_indicated, airspeed_true, vertical_speed,
                                                         heading_indicator, flaps_handle_position, spoilers_handle_position,
                                                         gear_handle_position, ambient_wind_velocity, ambient_wind_direction, ambient_temperature, 
                                                         stall_warning, overspeed_warning, is_gear_retractable, spoiler_available);
                    dtLastDataRecord = DateTime.Now;
                }
            }
        }

        // function is called from the retrieval of information from the simconnect and hold the aircraft name info
        public void UseSimEnvData(string aircraft)
        {
            if (bLoggingEnabled == true)
            {
                lCurrentFlightID = FlightPathDB.WriteFlight(aircraft);
            }
        }

        // function that writes out KML file based on the flight chosen by the user
        private void CreateKMLButton_Click(object sender, EventArgs e)
        {
            int nCount;
            int nFlightID;
            string sfilename;

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

            nFlightID = (int) FlightPickerLV.SelectedItems[0].Tag;
            
            // This is the root element of the file
            var kml = new Kml();
            Folder mainFolder = new Folder();
            mainFolder.Name = "Flight Data";
            mainFolder.Description = new Description
            {
                Text = "Overall Data for the flight"
            };

            kml.Feature = mainFolder;
            var placemarkLine = new Placemark();
            mainFolder.AddFeature(placemarkLine);
            placemarkLine.Name = "Flight Path Line";
            placemarkLine.Description = new Description
            {
                Text = "Line of the flight"
            };
            placemarkLine.StyleUrl = new Uri("#transPurpleLineGreenPoly", UriKind.Relative);

            var linestring = new LineString();
            var coordinatecollection = new CoordinateCollection();

            linestring.Coordinates = coordinatecollection;
            linestring.AltitudeMode = AltitudeMode.Absolute;

            List<FlightPathData> FlightPath = new List<FlightPathData>();
            FlightPath = FlightPathDB.GetFlightPathData(nFlightID);
            foreach (FlightPathData fpd in FlightPath)
                coordinatecollection.Add(new Vector(fpd.latitude, fpd.longitude, fpd.altitude * 0.3048));
            placemarkLine.Geometry = linestring;

            Folder folder = new Folder();
            folder.Name = "Flight Path Data Points";
            folder.Description = new Description
            {
                Text = "Data Points along the flight path"
            };
            mainFolder.AddFeature(folder);

            nCount = 0;
            foreach (FlightPathData fpd in FlightPath)
            {
                var placemarkPoint = new Placemark();
                string descriptioncard;
                bool bAnyLightsOn = false;

                nCount++;
                placemarkPoint.Name = String.Concat("Flight Data Point ", nCount.ToString());
                descriptioncard = String.Concat("<br>Timestamp = ", new DateTime(fpd.timestamp).ToString());
                
                descriptioncard += String.Concat(String.Format("<br><br>Coordinates ({0:0.0000}, {1:0.0000}, {2} feet)", fpd.latitude, fpd.longitude, fpd.altitude));
                descriptioncard += String.Format("<br>Temperature: {0:0.00}C / {1:0.00}F", fpd.ambient_temperature, fpd.ambient_temperature * 9 / 5 + 32);
                descriptioncard += String.Format("<br>Wind: {0:0.00} knts from {1:0.00} degrees", fpd.ambient_wind_velocity, fpd.ambient_wind_direction);
                descriptioncard += String.Format("<br>Altitude Above Ground: {0} feet", fpd.altitudeaboveground);
                
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

                placemarkPoint.Time = new SharpKml.Dom.Timestamp
                {
                    When = new DateTime(fpd.timestamp)
                };
                placemarkPoint.Geometry = new SharpKml.Dom.Point
                {
                    Coordinate = new Vector(fpd.latitude, fpd.longitude, (double) fpd.altitude * 0.3048),
                    AltitudeMode = AltitudeMode.Absolute
                };
                folder.AddFeature(placemarkPoint);
            }

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
            bLoggingEnabled = false;
            simConnectIntegration.Disconnect();

            StartLoggingBtn.Enabled = true;
            PauseLoggingBtn.Enabled = false;
            StopLoggingBtn.Enabled = false;
            ContinueLogginBtn.Enabled = false;
            LoadFlightList();
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
            foreach (FlightListData flist in FlightList)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = new DateTime(flist.Start_flight_timestamp).ToString();
                lvi.SubItems.Add(flist.Aircraft);
                lvi.Tag = flist.FlightID1;
                FlightPickerLV.Items.Add(lvi);
            }
            FlightPickerLV.Sorting = SortOrder.Descending;
            FlightPickerLV.Sort();

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
        }
    }
}
