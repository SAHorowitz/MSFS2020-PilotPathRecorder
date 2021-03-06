using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace FS2020PlanePath
{
    public class FlightPathData
    {
        public double latitude;
        public double longitude;
        public Int32 altitude;
        public long timestamp;
        public Int32 altitudeaboveground;
        public Int32 Eng1Rpm;
        public Int32 Eng2Rpm;
        public Int32 Eng3Rpm;
        public Int32 Eng4Rpm;
        public Int32 LightsMask;
        public double ground_velocity;
        public double plane_pitch;
        public double plane_bank;
        public double plane_heading_true;
        public double plane_heading_magnetic;
        public double plane_airspeed_indicated;
        public double airspeed_true;
        public double vertical_speed;
        public double heading_indicator;
        public Int32 flaps_handle_position;
        public Int32 spoilers_handle_position;
        public Int32 gear_handle_position;
        public double ambient_wind_velocity;
        public double ambient_wind_direction;
        public double ambient_temperature;
        public Int32 stall_warning;
        public Int32 overspeed_warning;
        public Int32 is_gear_retractable;
        public Int32 spoiler_available;
        public Int32 sim_on_ground;

        [Flags]
        public enum LightStates
        {
            Nav = 0x0001,
            Beacon = 0x0002,
            Landing = 0x0004,
            Taxi = 0x0008,
            Strobe = 0x0010,
            Panel = 0x0020,
            Recognition = 0x0040,
            Wing = 0x0080,
            Logo = 0x0100,
            Cabin = 0x0200
        }
    }

    class FlightListData
    {
        public Int32 FlightID;
        public string aircraft;
        public long start_flight_timestamp;
    }

    class FlightWaypointData
    {
        public double gps_wp_latitude;
        public double gps_wp_longitude;
        public Int32 gps_wp_altitude;
        public string gps_wp_name;

        public FlightWaypointData()
        {
        }

        public FlightWaypointData(double gps_wp_lat, double gps_wp_long, Int32 gps_wp_alt, string gps_wp_id)
        {
            gps_wp_latitude = gps_wp_lat;
            gps_wp_longitude = gps_wp_long;
            gps_wp_altitude = gps_wp_alt;
            gps_wp_name = gps_wp_id;
        }
    }

    class FlightPlan
    {
        public List<FlightWaypointData> flight_waypoints;
        private int flightplan_index;
        private int flightplan_count;

        public FlightPlan()
        {
            flight_waypoints = new List<FlightWaypointData>();
        }

        public bool IsWaypointValid(FlightWaypointData waypointData)
        {
            if ((waypointData.gps_wp_altitude > 0) &&
                (waypointData.gps_wp_latitude != 0) &&
                (waypointData.gps_wp_longitude != 0) &&
                (waypointData.gps_wp_name.Length > 0))
                return true;
            else
                return false;
        }

        public void AddFlightPlanWaypoint(FlightWaypointData prevWaypoint, FlightWaypointData nextWaypoint, int iflightplan_index, int iflightplan_count)
        {
            // if there is already at least one item in the list then compare last item to see if it matches
            if (flight_waypoints.Count > 0)
            {
                FlightWaypointData fwdLastWaypointinList;

                fwdLastWaypointinList = flight_waypoints[flight_waypoints.Count - 1];

                // if the next waypoint is not equal to last in the list then something has changed
                if (String.Compare(fwdLastWaypointinList.gps_wp_name, nextWaypoint.gps_wp_name) != 0)
                    // if last waypoint in list is equal to the prev waypoint then we can add next waypoint to the list
                    // because it means user crossed the waypoint and possibly heading to next or is done with flightplan
                    if (String.Compare(fwdLastWaypointinList.gps_wp_name, prevWaypoint.gps_wp_name) == 0)
                    {
                        if (IsWaypointValid(nextWaypoint) == true)
                            flight_waypoints.Add(nextWaypoint);
                        flightplan_index = iflightplan_index;
                        flightplan_count = iflightplan_count;
                    }
                    else
                    {
                        FlightWaypointData fwdSecondToLastWaypointinList;

                        fwdSecondToLastWaypointinList = flight_waypoints[flight_waypoints.Count - 2];

                        // if second to last is same as previous then user just changed next waypoint so just replace it
                        if (String.Compare(fwdSecondToLastWaypointinList.gps_wp_name, prevWaypoint.gps_wp_name) == 0)
                        {
                            flight_waypoints.RemoveAt(flight_waypoints.Count - 1);
                            if (IsWaypointValid(nextWaypoint) == true)
                                flight_waypoints.Add(nextWaypoint);
                        }
                        else
                        {
                            // these new waypoints don't match anything we have so user changed flightplan?
                            flight_waypoints.Clear();
                            if (IsWaypointValid(prevWaypoint) == true)
                                flight_waypoints.Add(prevWaypoint);
                            if (iflightplan_index < iflightplan_count)
                                if (IsWaypointValid(nextWaypoint) == true)
                                    flight_waypoints.Add(nextWaypoint);
                        }
                        flightplan_index = iflightplan_index;
                        flightplan_count = iflightplan_count;
                    }
            }
            else // add new items that are valid
            {
                if (IsWaypointValid(prevWaypoint) == true)
                    flight_waypoints.Add(prevWaypoint);
                // if not at end of flightplan then there should be a next waypoint
                if (iflightplan_index < iflightplan_count)
                    if (IsWaypointValid(nextWaypoint) == true)
                        flight_waypoints.Add(nextWaypoint);
                flightplan_index = iflightplan_index;
                flightplan_count = iflightplan_count;
            }
        }
    }

    class FS2020_SQLLiteDB
    {
        SQLiteConnection sqlite_conn;
        private const int TblVersion_Flights = 1;
        private const int TblVersion_FlightSamples = 1;
        private const int TblVersion_FlightSampleDetails = 3;
        private const int TblVersion_FlightWaypoints = 1;
        private const int TblVersion_FlightOptions = 1;

        internal const double DEFAULT_ABOVE_THRESHOLD_WRITE_FREQ = 5;
        internal const int DEFAULT_THRESHOLD_MIN_ALT = 500;
        internal const int DEFAULT_AUTOMATIC_LOGGING_THRESHOLD = 30;

        public FS2020_SQLLiteDB()
        {
            CreateConnection();
        }

        void CreateConnection()
        {
            sqlite_conn = new SQLiteConnection("Data Source=FlightSimFlightsDB.db; Version = 3; New = True; Compress = True; ");
         // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                sqlite_conn = null;
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "new SQLiteConnection", null), ex);
            }
        }

        private string GetDBQueryExtraInfo(string strCallerName, string strQueryType, SQLiteCommand sqlite_cmd)
        {
            string sAdtlInfo = strCallerName + " " + strQueryType;
            if (sqlite_cmd != null)
            {
                sAdtlInfo  += ": " + sqlite_cmd.CommandText.ToString();
                foreach (SQLiteParameter p in sqlite_cmd.Parameters)
                {
                    string isQuted = (p.Value is string) ? "'" : "";
                    sAdtlInfo += "\n " + p.ParameterName.ToString() + " = " + isQuted + p.Value.ToString() + isQuted;
                }
            }
            return sAdtlInfo;
        }

        public void CreateTables()
        {
            SQLiteCommand sqlite_cmd;
            string Createsql;

            if (CheckTableExists("Flights") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE Flights (FlightID INTEGER PRIMARY KEY, aircraft varchar(256), start_datetimestamp long)";
                sqlite_cmd.CommandText = Createsql;
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }

            if (CheckTableExists("TblVersions") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE TblVersions (tblVersionID INTEGER PRIMARY KEY, tblname varchar(256), tblversion int)";
                sqlite_cmd.CommandText = Createsql;
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }


            if (CheckTableExists("FlightSamples") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE FlightSamples (FlightSamplesID INTEGER PRIMARY KEY, FlightID integer NOT NULL, latitude double, longitude double, altitude int32, sample_datetimestamp long,  ";
                Createsql += "FOREIGN KEY (FlightID) REFERENCES Flight(FlightID) )";
                sqlite_cmd.CommandText = Createsql;
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }

            if (CheckTableExists("FlightSampleDetails") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE FlightSampleDetails (FlightSampleDetailsID INTEGER PRIMARY KEY, FlightSamplesID long NOT NULL, alitutdeaboveground int32, engine1rpm int32, engine2rpm int32, ";
                Createsql += "engine3rpm int32, engine4rpm int32, lightsmask int32, ground_velocity double, plane_pitch double, plane_bank double, plane_heading_true double, ";
                Createsql += "plane_heading_magnetic double, plane_airspeed_indicated double, airspeed_true double, vertical_speed double, heading_indicator double, flaps_handle_position int32, ";
                Createsql += "spoilers_handle_position int32, gear_handle_position int32, ambient_wind_velocity double, ambient_wind_direction double, ambient_temperature double, ";
                Createsql += "stall_warning int32, overspeed_warning int32, is_gear_retractable int32, spoiler_available int32, sim_on_ground int32, FOREIGN KEY (FlightSamplesID) ";
                Createsql += "REFERENCES FlightSamples(FlightSamplesID) )";
                sqlite_cmd.CommandText = Createsql;
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }

            if (CheckTableExists("FlightPathOptions") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE FlightPathOptions (OptionsID INTEGER PRIMARY KEY, optionname varchar(256), optionvalue varchar(512))";
                sqlite_cmd.CommandText = Createsql;
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }

            if (CheckTableExists("FlightWaypoints") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE FlightWaypoints (FlightWaypointsID INTEGER PRIMARY KEY, FlightID integer NOT NULL, latitude double, longitude double, altitude int32, name varchar(256),  ";
                Createsql += "FOREIGN KEY (FlightID) REFERENCES Flight(FlightID) )";
                sqlite_cmd.CommandText = Createsql;
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }

            // Fill in Table Versions if needed
            LoadUpTableVersions();

            // Check Table versions and update as needed
            UpdateTablesAsNeeded();

            // Load Up Table Options if needed
            LoadUpTableOptions();
        }

        private void LoadUpTableVersions()
        {
            SQLiteCommand sqlite_cmd;
            string selectsql;
            int nNumVersionRows;
            string insertsql;

            sqlite_cmd = sqlite_conn.CreateCommand();
            selectsql = "SELECT COUNT(tblVersionID) from TblVersions";
            sqlite_cmd.CommandText = selectsql;
            nNumVersionRows = Convert.ToInt32(sqlite_cmd.ExecuteScalar());

            sqlite_cmd = sqlite_conn.CreateCommand();
            insertsql = "INSERT INTO TblVersions (tblname, tblversion) VALUES (@tblname, @tblversion)";
            sqlite_cmd.CommandText = insertsql;

            // since we know the tables and the order we have added them to the program over time
            // then we can use the number of rows to know what table versions we should add
            // adding one afterwards ensures we catch all the missing from them on to the end
            if (nNumVersionRows == 0)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@tblname", "Flights");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_Flights);
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumVersionRows++;
            }
            if (nNumVersionRows == 1)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@tblname", "FlightSamples");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_FlightSamples);
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumVersionRows++;
            }
            if (nNumVersionRows == 2)
            { 
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@tblname", "FlightSampleDetails");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_FlightSampleDetails);
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumVersionRows++;
            }
            if (nNumVersionRows == 3)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@tblname", "FlightPathOptions");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_FlightOptions);
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumVersionRows++;
            }
            if (nNumVersionRows == 4)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@tblname", "FlightWaypoints");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_FlightWaypoints);
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumVersionRows++;
            }
        }

        private void UpdateTablesAsNeeded()
        {
            int nCurTableVersion = 0;

            nCurTableVersion = GetCurrentTableVersion("FlightSampleDetails");

            if (nCurTableVersion < TblVersion_FlightSampleDetails)
                UpdateFlightSampleDetailsTable(nCurTableVersion);
        }

        private int GetCurrentTableVersion(string sTable)
        {
            SQLiteCommand sqlite_cmd;
            string Selectsql;
            int nRetval = 0;

            sqlite_cmd = sqlite_conn.CreateCommand();
            Selectsql = "SELECT tblVersion FROM TblVersions WHERE tblName  = @tblname";
            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.AddWithValue("@tblname", sTable);
            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                    nRetval = r.GetInt32(0);
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }

            return (nRetval);
        }

        private void UpdateFlightSampleDetailsTable(int nCurTableVersion)
        {
            SQLiteCommand sqlite_cmd;
            string Updatesql;

            sqlite_cmd = sqlite_conn.CreateCommand();

            if (nCurTableVersion == 1)
            {
                Updatesql = "ALTER TABLE FlightSampleDetails ADD sim_on_ground int32";
                sqlite_cmd.CommandText = Updatesql;
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }

                sqlite_cmd.Parameters.Clear();
                Updatesql = "Update FlightSampleDetails SET sim_on_ground = 0";
                sqlite_cmd.CommandText = Updatesql;
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }

                sqlite_cmd.Parameters.Clear();
                Updatesql = "Update TblVersions SET tblVersion = @tblversion WHERE tblName = 'FlightSampleDetails'";
                sqlite_cmd.CommandText = Updatesql;
                nCurTableVersion = nCurTableVersion + 1;
                sqlite_cmd.Parameters.AddWithValue("@tblversion", nCurTableVersion);
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }
            if (nCurTableVersion == 2)
            {
                // 2 was to fix a mistake where create table did not contain sim_on_ground
                bool bSimOnGroundInDB = false;

                Updatesql = "Select * from FlightSampleDetails";
                sqlite_cmd.CommandText = Updatesql;

                try
                {
                    using (SQLiteDataReader r = sqlite_cmd.ExecuteReader())
                    {
                        // walk thru results of query and see if sim_on_ground column is there
                        for (var i = 0; i < r.FieldCount; i++)
                            if (String.Compare(r.GetName(i), "sim_on_ground") == 0)
                                bSimOnGroundInDB = true;
                    }
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                    throw ex;
                }
                // if sim_on_ground is not in db then we need to add it
                if (bSimOnGroundInDB == false)
                {
                    Updatesql = "ALTER TABLE FlightSampleDetails ADD sim_on_ground int32";
                    sqlite_cmd.CommandText = Updatesql;
                    try
                    {
                        sqlite_cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                        throw ex;
                    }

                    sqlite_cmd.Parameters.Clear();
                    Updatesql = "Update FlightSampleDetails SET sim_on_ground = 0";
                    sqlite_cmd.CommandText = Updatesql;
                    try
                    {
                        sqlite_cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                        throw ex;
                    }
                }

                sqlite_cmd.Parameters.Clear();
                Updatesql = "Update TblVersions SET tblVersion = @tblversion WHERE tblName = 'FlightSampleDetails'";
                sqlite_cmd.CommandText = Updatesql;
                nCurTableVersion = nCurTableVersion + 1;
                sqlite_cmd.Parameters.AddWithValue("@tblversion", nCurTableVersion);
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }
        }

        private void LoadUpTableOptions()
        {
            SQLiteCommand sqlite_cmd;
            string selectsql;
            int nNumOptionRows;
            string insertsql;

            sqlite_cmd = sqlite_conn.CreateCommand();
            selectsql = "SELECT COUNT(OptionsID) from FlightPathOptions";
            sqlite_cmd.CommandText = selectsql;

            try
            {
                nNumOptionRows = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteScalar", sqlite_cmd), ex);
                throw ex;
            }

            // since we know the options and the order we have added them to the program over time
            // then we can use the number of rows to know what options we should add
            // adding one afterwards ensures we catch all the missing from them on to the end
            sqlite_cmd = sqlite_conn.CreateCommand();
            insertsql = "INSERT INTO FlightPathOptions (optionname, optionvalue) VALUES (@optionname, @optionvalue)";
            sqlite_cmd.CommandText = insertsql;
            if (nNumOptionRows == 0)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "AboveThresholdWriteFreq");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", DEFAULT_ABOVE_THRESHOLD_WRITE_FREQ.ToString());
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumOptionRows++;
            }
            if (nNumOptionRows == 1)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "ThresholdMinAltitude");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", DEFAULT_THRESHOLD_MIN_ALT.ToString());
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumOptionRows++;
            }
            if (nNumOptionRows == 2)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "KMLFilePath");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "");
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumOptionRows++;
            }
            if (nNumOptionRows == 3)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "GoolgeEarthChoice");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "Application");
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumOptionRows++;
            }
            if (nNumOptionRows == 4)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "SpeedUpVideoPlayback");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "true");
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumOptionRows++;
            }
            if (nNumOptionRows == 5)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "AutomaticLogging");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "true");
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumOptionRows++;
            }
            if (nNumOptionRows == 6)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "AutomaticLoggingThreshold");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", DEFAULT_AUTOMATIC_LOGGING_THRESHOLD.ToString());
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
                nNumOptionRows++;
            }
        }

        public String GetTableOption(String optionname)
        {
            SQLiteCommand sqlite_cmd;
            string Selectsql;
            string sRetval = "";

            sqlite_cmd = sqlite_conn.CreateCommand();
            Selectsql = "SELECT optionvalue FROM FlightPathOptions WHERE optionname  = @optionname";
            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.AddWithValue("@optionname", optionname);
            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                    sRetval = r.GetString(0);
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }

            return (sRetval);
        }

        public void WriteTableOption(String optionname, String optionvalue)
        {
            SQLiteCommand sqlite_cmd;
            string Updatesql;

            sqlite_cmd = sqlite_conn.CreateCommand();
            Updatesql = "Update FlightPathOptions SET optionvalue = @optionvalue WHERE optionname  = @optionname";
            sqlite_cmd.CommandText = Updatesql;
            sqlite_cmd.Parameters.AddWithValue("@optionname", optionname);
            sqlite_cmd.Parameters.AddWithValue("@optionvalue", optionvalue);
            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }
        }

        private bool CheckTableExists(String tblName)
        {
            SQLiteCommand sqlite_cmd;
            bool bRetVal = false;

            string Selectsql;
            sqlite_cmd = sqlite_conn.CreateCommand();
            Selectsql = "SELECT name FROM sqlite_master WHERE type ='table' and name = @tblName";
            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.AddWithValue("@tblName", tblName);
            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                    bRetVal = true;
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }

            return (bRetVal);
        }

        public int WriteFlight(string aircraft)
        {
            SQLiteCommand sqlite_cmd;
            string sqlStr;
            SQLiteTransaction transaction = null;
            long FlightID;

            sqlite_cmd = sqlite_conn.CreateCommand();
            transaction = sqlite_conn.BeginTransaction();
            sqlStr = "Insert into Flights (aircraft, start_datetimestamp) VALUES (@aircraft, @start_datetimestamp)";
            sqlite_cmd.CommandText = sqlStr;
            sqlite_cmd.Parameters.AddWithValue("@aircraft", aircraft);
            sqlite_cmd.Parameters.AddWithValue("@start_datetimestamp", DateTime.Now.Ticks);
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                throw ex;
            }
            FlightID = sqlite_conn.LastInsertRowId;
            transaction.Commit();

            return Convert.ToInt32(FlightID);
        }

        public int WriteFlightPoint(long flightId, double latitude, double longitude, Int32 altitude)
        {
            SQLiteCommand sqlite_cmd;
            string Insertsql;
            SQLiteTransaction transaction = null;
            long FlightSampleID;

            sqlite_cmd = sqlite_conn.CreateCommand();
            transaction = sqlite_conn.BeginTransaction();
            Insertsql = "Insert into FlightSamples (FlightID, latitude, longitude, altitude, sample_datetimestamp) VALUES (@FlightID, @latitude, @longitude, @altitude, @sample_datetimestamp)";
            sqlite_cmd.CommandText = Insertsql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", flightId);
            sqlite_cmd.Parameters.AddWithValue("@latitude", latitude);
            sqlite_cmd.Parameters.AddWithValue("@longitude", longitude);
            sqlite_cmd.Parameters.AddWithValue("@altitude", altitude);
            sqlite_cmd.Parameters.AddWithValue("@sample_datetimestamp", DateTime.Now.Ticks);
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                throw ex;
            }

            FlightSampleID = sqlite_conn.LastInsertRowId;
            transaction.Commit();

            return Convert.ToInt32(FlightSampleID);
        }

        public void WriteFlightPointDetails(long flightSampleId, Int32 altitude_above_ground, Int32 engine1rpm, Int32 engine2rpm, Int32 engine3rpm, Int32 engine4rpm, Int32 lightsmask, double ground_velocity,
                                            double plane_pitch, double plane_bank, double plane_heading_true, double plane_heading_magnetic,
                                            double plane_airspeed_indicated, double airspeed_true, double vertical_speed, double heading_indicator,
                                            Int32 flaps_handle_position, Int32 spoilers_handle_position, Int32 gear_handle_position,
                                            double ambient_wind_velocity, double ambient_wind_direction, double ambient_temperature, Int32 stall_warning,
                                            Int32 overspeed_warning, Int32 is_gear_retractable, Int32 spoiler_available, Int32 sim_on_ground)
        {
            SQLiteCommand sqlite_cmd;
            string Insertsql;

            sqlite_cmd = sqlite_conn.CreateCommand();
            Insertsql = "Insert into FlightSampleDetails (FlightSamplesID, alitutdeaboveground, engine1rpm, engine2rpm, engine3rpm, engine4rpm, lightsmask, ground_velocity, plane_pitch, plane_bank, plane_heading_true, ";
            Insertsql += "plane_heading_magnetic, plane_airspeed_indicated, airspeed_true, vertical_speed, heading_indicator, flaps_handle_position, spoilers_handle_position, gear_handle_position, ambient_wind_velocity, ";
            Insertsql += "ambient_wind_direction, ambient_temperature, stall_warning, overspeed_warning, is_gear_retractable, spoiler_available, sim_on_ground) VALUES (@FlightSamplesID, @alitutdeaboveground, @engine1rpm, ";
            Insertsql += "@engine2rpm, @engine3rpm, @engine4rpm, @lightsmask, @ground_velocity, @plane_pitch, @plane_bank, @plane_heading_true, @plane_heading_magnetic, @plane_airspeed_indicated, @airspeed_true, ";
            Insertsql += "@vertical_speed, @heading_indicator, @flaps_handle_position, @spoilers_handle_position, @gear_handle_position, @ambient_wind_velocity, @ambient_wind_direction, @ambient_temperature, @stall_warning, ";
            Insertsql += "@overspeed_warning, @is_gear_retractable, @spoiler_available, @sim_on_ground)";
            sqlite_cmd.CommandText = Insertsql;
            sqlite_cmd.Parameters.AddWithValue("@FlightSamplesID", flightSampleId);
            sqlite_cmd.Parameters.AddWithValue("@alitutdeaboveground", altitude_above_ground);
            sqlite_cmd.Parameters.AddWithValue("@engine1rpm", engine1rpm);
            sqlite_cmd.Parameters.AddWithValue("@engine2rpm", engine2rpm);
            sqlite_cmd.Parameters.AddWithValue("@engine3rpm", engine3rpm);
            sqlite_cmd.Parameters.AddWithValue("@engine4rpm", engine4rpm);
            sqlite_cmd.Parameters.AddWithValue("@lightsmask", lightsmask);
            sqlite_cmd.Parameters.AddWithValue("@ground_velocity", ground_velocity);
            sqlite_cmd.Parameters.AddWithValue("@plane_pitch", plane_pitch);
            sqlite_cmd.Parameters.AddWithValue("@plane_bank", plane_bank);
            sqlite_cmd.Parameters.AddWithValue("@plane_heading_true", plane_heading_true);
            sqlite_cmd.Parameters.AddWithValue("@plane_heading_magnetic", plane_heading_magnetic);
            sqlite_cmd.Parameters.AddWithValue("@plane_airspeed_indicated", plane_airspeed_indicated);
            sqlite_cmd.Parameters.AddWithValue("@airspeed_true", airspeed_true);
            sqlite_cmd.Parameters.AddWithValue("@vertical_speed", vertical_speed);
            sqlite_cmd.Parameters.AddWithValue("@heading_indicator", heading_indicator);
            sqlite_cmd.Parameters.AddWithValue("@flaps_handle_position", flaps_handle_position);
            sqlite_cmd.Parameters.AddWithValue("@spoilers_handle_position", spoilers_handle_position);
            sqlite_cmd.Parameters.AddWithValue("@gear_handle_position", gear_handle_position);
            sqlite_cmd.Parameters.AddWithValue("@ambient_wind_velocity", ambient_wind_velocity);
            sqlite_cmd.Parameters.AddWithValue("@ambient_wind_direction", ambient_wind_direction);
            sqlite_cmd.Parameters.AddWithValue("@ambient_temperature", ambient_temperature);
            sqlite_cmd.Parameters.AddWithValue("@stall_warning", stall_warning);
            sqlite_cmd.Parameters.AddWithValue("@overspeed_warning", overspeed_warning);
            sqlite_cmd.Parameters.AddWithValue("@is_gear_retractable", is_gear_retractable);
            sqlite_cmd.Parameters.AddWithValue("@spoiler_available", spoiler_available);
            sqlite_cmd.Parameters.AddWithValue("@sim_on_ground", sim_on_ground);
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                throw ex;
            }
        }

        public void WriteFlightPlan(int pk, List<FlightWaypointData> flight_waypoints)
        {
            SQLiteCommand sqlite_cmd;
            string Insertsql;
            SQLiteTransaction transaction = null;

            sqlite_cmd = sqlite_conn.CreateCommand();
            transaction = sqlite_conn.BeginTransaction();
            Insertsql = "Insert into FlightWaypoints (FlightID, latitude, longitude, altitude, name) VALUES (@FlightID, @latitude, @longitude, @altitude, @name)";
            sqlite_cmd.CommandText = Insertsql;

            foreach (FlightWaypointData waypoint in flight_waypoints)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@FlightID", pk);
                sqlite_cmd.Parameters.AddWithValue("@latitude", waypoint.gps_wp_latitude);
                sqlite_cmd.Parameters.AddWithValue("@longitude", waypoint.gps_wp_longitude);
                sqlite_cmd.Parameters.AddWithValue("@altitude", waypoint.gps_wp_altitude);
                sqlite_cmd.Parameters.AddWithValue("@name", waypoint.gps_wp_name);
                try
                {
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                    throw ex;
                }
            }
            transaction.Commit();
        }

        public List<FlightWaypointData> GetFlightWaypoints(int pk)
        {
            List<FlightWaypointData> FlightWaypoints = new List<FlightWaypointData>();
            SQLiteCommand sqlite_cmd;
            string Selectsql;
            sqlite_cmd = sqlite_conn.CreateCommand();
            Selectsql = "Select latitude, longitude, altitude, name FROM FlightWaypoints WHERE FlightID = @FlightID";
            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", pk);
            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                {
                    FlightWaypointData waypointData = new FlightWaypointData();
                    waypointData.gps_wp_latitude = r.GetDouble(0);
                    waypointData.gps_wp_longitude = r.GetDouble(1);
                    waypointData.gps_wp_altitude = r.GetInt32(2);
                    waypointData.gps_wp_name = r.GetString(3);

                    FlightWaypoints.Add(waypointData);
                }
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }

            return FlightWaypoints;
        }

        public List<FlightPathData> GetFlightPathSinceTimestamp(int pk, long startingTimestamp)
        {

            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = @"
select 
    sample_datetimestamp, 
    cast(latitude as double), 
    cast(longitude as double), 
    altitude, 
    cast(plane_pitch as double), 
    cast(plane_bank as double), 
    cast(plane_heading_true as double),
    cast (ground_velocity as double),
    cast (plane_heading_magnetic as double),
    cast(plane_airspeed_indicated as double),
    cast(airspeed_true as double), 
    cast(vertical_speed as double), 
    cast(heading_indicator as double), 
    flaps_handle_position,
    spoilers_handle_position, 
    gear_handle_position, 
    cast (ambient_wind_velocity as double), 
    cast (ambient_wind_direction as double), 
    cast (ambient_temperature as double),
    stall_warning, 
    overspeed_warning, 
    is_gear_retractable, 
    spoiler_available, 
    sim_on_ground,
    alitutdeaboveground
from
    flightsamples s, flightsampledetails d 
where 
    s.flightsamplesid = d.flightsamplesid 
and sample_datetimestamp > @earliestDateTimestamp 
and flightid = @FlightID
"; ;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", pk);
            sqlite_cmd.Parameters.AddWithValue("@earliestDateTimestamp", startingTimestamp);

            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();
                List<FlightPathData> FlightPath = new List<FlightPathData>();
                while (r.Read())
                {
                    FlightPath.Add(
                        new FlightPathData {
                            timestamp = r.GetInt64(0),
                            latitude = r.GetDouble(1),
                            longitude = r.GetDouble(2),
                            altitude = r.GetInt32(3),
                            plane_pitch = r.GetDouble(4),
                            plane_bank = r.GetDouble(5),
                            plane_heading_true = r.GetDouble(6),
                            ground_velocity = r.GetDouble(7),
                            plane_heading_magnetic = r.GetDouble(8),
                            plane_airspeed_indicated = r.GetDouble(9),
                            airspeed_true = r.GetDouble(10),
                            vertical_speed = r.GetDouble(11),
                            heading_indicator = r.GetDouble(12),
                            flaps_handle_position = r.GetInt32(13),
                            spoilers_handle_position = r.GetInt32(14),
                            gear_handle_position = r.GetInt32(15),
                            ambient_wind_velocity = r.GetDouble(16),
                            ambient_wind_direction = r.GetDouble(17),
                            ambient_temperature = r.GetDouble(18),
                            stall_warning = r.GetInt32(19),
                            overspeed_warning = r.GetInt32(20),
                            is_gear_retractable = r.GetInt32(21),
                            sim_on_ground = r.GetInt32(22),
                            altitudeaboveground = r.GetInt32(23)
                        }
                    );
                }
                Console.WriteLine($"query for flightId({pk}) since({startingTimestamp}) found({FlightPath.Count}) entries");
                return FlightPath;
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }

        }

        public List<FlightPathData> GetFlightPathData(int pk)
        {
            List<FlightPathData> FlightPath = new List<FlightPathData>();
            SQLiteCommand sqlite_cmd;
            string Selectsql;
            sqlite_cmd = sqlite_conn.CreateCommand();
            Selectsql = "SELECT cast(latitude as double), cast (longitude as double), altitude, sample_datetimestamp, alitutdeaboveground, engine1rpm, engine2rpm, engine3rpm, engine4rpm, lightsmask, ";
            Selectsql += "cast (ground_velocity as double), cast (plane_pitch as double), cast (plane_bank as double), cast (plane_heading_true as double), cast (plane_heading_magnetic as double), ";
            Selectsql += "cast (plane_airspeed_indicated as double), cast (airspeed_true as double), cast (vertical_speed as double), cast (heading_indicator as double), flaps_handle_position, ";
            Selectsql += "spoilers_handle_position, gear_handle_position, cast (ambient_wind_velocity as double), cast (ambient_wind_direction as double), cast (ambient_temperature as double),";
            Selectsql += "stall_warning, overspeed_warning, is_gear_retractable, spoiler_available, sim_on_ground ";
            Selectsql += "FROM FlightSamples, FlightSampleDetails WHERE FlightSampleDetails.FlightSamplesID = FlightSamples.FlightSamplesID AND FlightID = @FlightID";
            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", pk);
            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                {
                    FlightPathData data = new FlightPathData();
                    data.latitude = r.GetDouble(0);
                    data.longitude = r.GetDouble(1);
                    data.altitude = r.GetInt32(2);
                    data.timestamp = r.GetInt64(3);
                    data.altitudeaboveground = r.GetInt32(4);
                    data.Eng1Rpm = r.GetInt32(5);
                    data.Eng2Rpm = r.GetInt32(6);
                    data.Eng3Rpm = r.GetInt32(7);
                    data.Eng4Rpm = r.GetInt32(8);
                    data.LightsMask = r.GetInt32(9);
                    data.ground_velocity = r.GetDouble(10);
                    data.plane_pitch = r.GetDouble(11);
                    data.plane_bank = r.GetDouble(12);
                    data.plane_heading_true = r.GetDouble(13);
                    data.plane_heading_magnetic = r.GetDouble(14);
                    data.plane_airspeed_indicated = r.GetDouble(15);
                    data.airspeed_true = r.GetDouble(16);
                    data.vertical_speed = r.GetDouble(17);
                    data.heading_indicator = r.GetDouble(18);
                    data.flaps_handle_position = r.GetInt32(19);
                    data.spoilers_handle_position = r.GetInt32(20);
                    data.gear_handle_position = r.GetInt32(21);
                    data.ambient_wind_velocity = r.GetDouble(22);
                    data.ambient_wind_direction = r.GetDouble(23);
                    data.ambient_temperature = r.GetDouble(24);
                    data.stall_warning = r.GetInt32(25);
                    data.overspeed_warning = r.GetInt32(26);
                    data.is_gear_retractable = r.GetInt32(27);
                    data.spoiler_available = r.GetInt32(28);
                    data.sim_on_ground = r.GetInt32(29);

                    FlightPath.Add(data);
                }
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }

            return FlightPath;
        }

        public List<FlightListData> GetFlightList()
        {
            List<FlightListData> FlightList = new List<FlightListData>();
            SQLiteCommand sqlite_cmd;
            string Selectsql;
            sqlite_cmd = sqlite_conn.CreateCommand();
            Selectsql = "SELECT FlightID, aircraft, start_datetimestamp FROM Flights ORDER BY FlightID ASC";
            sqlite_cmd.CommandText = Selectsql;
            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                {
                    FlightListData data = new FlightListData();
                    data.FlightID = r.GetInt32(0);
                    data.aircraft = r.GetString(1);
                    data.start_flight_timestamp = r.GetInt64(2);
                    FlightList.Add(data);
                }
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }

            return FlightList;
        }

        public void DeleteFlight(int nFlightID)
        {
            SQLiteCommand sqlite_cmd;
            string Deletesql;
            SQLiteTransaction transaction = null;

            sqlite_cmd = sqlite_conn.CreateCommand();
            transaction = sqlite_conn.BeginTransaction();
            
            Deletesql = "Delete from FlightWaypoints WHERE FlightID = @FlightID";
            sqlite_cmd.CommandText = Deletesql;
            sqlite_cmd.Parameters.Clear();
            sqlite_cmd.Parameters.AddWithValue("@FlightID", nFlightID);
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                throw ex;
            }

            Deletesql = "Delete from FlightSampleDetails WHERE FlightSampleDetails.FlightSamplesID IN (select FlightSamplesID From FlightSamples WHERE FlightID = @FlightID)";
            sqlite_cmd.CommandText = Deletesql;
            sqlite_cmd.Parameters.Clear();
            sqlite_cmd.Parameters.AddWithValue("@FlightID", nFlightID);
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                throw ex;
            }

            sqlite_cmd = sqlite_conn.CreateCommand();
            Deletesql = "Delete from FlightSamples WHERE FlightID = @FlightID";
            sqlite_cmd.CommandText = Deletesql;
            sqlite_cmd.Parameters.Clear();
            sqlite_cmd.Parameters.AddWithValue("@FlightID", nFlightID);
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                throw ex;
            }

            sqlite_cmd = sqlite_conn.CreateCommand();
            Deletesql = "Delete from Flights WHERE FlightID = @FlightID";
            sqlite_cmd.CommandText = Deletesql;
            sqlite_cmd.Parameters.Clear();
            sqlite_cmd.Parameters.AddWithValue("@FlightID", nFlightID);
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                throw ex;
            }

            transaction.Commit();
        }
    }
}

