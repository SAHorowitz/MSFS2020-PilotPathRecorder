﻿using KBCsv;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using ExtensionMethods;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static void BindFunction(this SQLiteConnection connection, SQLiteFunction function)
        {
            var attributes = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).Cast<SQLiteFunctionAttribute>().ToArray();
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException("SQLiteFunction doesn't have SQLiteFunctionAttribute");
            }
            connection.BindFunction(attributes[0], function);
        }
    }
}

namespace FS2020PlanePath
{
    class AirportData
    {
        public string ident;
        public double latitude;
        public double longitude;
        public string name;
        public string city;
        public string state;
        public int num_runway_hard;
        public int num_runway_soft;
        public double distance;
    }

    class FlightPathData
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
        public bool waypoint_valid;

        public FlightWaypointData()
        {
        }

        public FlightWaypointData(double gps_wp_lat, double gps_wp_long, Int32 gps_wp_alt, string gps_wp_id)
        {
            gps_wp_latitude = gps_wp_lat;
            gps_wp_longitude = gps_wp_long;
            gps_wp_altitude = gps_wp_alt;
            gps_wp_name = gps_wp_id;

            if ((gps_wp_altitude >= 0) &&
                (gps_wp_latitude != 0) &&
                (gps_wp_longitude != 0) &&
                (gps_wp_name.Length > 0))
                waypoint_valid = true;
            else
                waypoint_valid = false;
        }
    }

    class FlightPlan
    {
        public List<FlightWaypointData> flight_waypoints;

        public FlightPlan()
        {
            flight_waypoints = new List<FlightWaypointData>();
        }

        public void AddFlightPlanWaypoint(FlightWaypointData prevWaypoint, FlightWaypointData nextWaypoint, int iflightplan_index, int iflightplan_count)
        {
            // if no waypoints yet or if last waypoint does not equal waypoint we just passed then add it to waypoint list.  Note this does not add the last waypoint of flight
            // to the waypoint list since very last waypoint is never past but StopLoggingAction will add last waypoint if it is an airport and within 2 miles of where flight ended
            if ((flight_waypoints.Count == 0) || 
                ((flight_waypoints.Count > 0) && (String.Compare(flight_waypoints.Last().gps_wp_name, prevWaypoint.gps_wp_name) != 0)))
                flight_waypoints.Add(prevWaypoint);
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
        private const int TblVersion_AirportInfo = 1;

        [SQLiteFunction(Name = "power", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class PowerSQLiteFunction : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Pow(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]));
            }
        }

        [SQLiteFunction(Name = "cos", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class CosineFunction : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Cos(Convert.ToDouble((double)args[0]));
            }
        }

        [SQLiteFunction(Name = "sqrt", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class SqrtFunction : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Sqrt(Convert.ToDouble((double)args[0]));
            }
        }

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
                sAdtlInfo += ": " + sqlite_cmd.CommandText.ToString();
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

            if (CheckTableExists("AirportInfo") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE AirportInfo (AirportInfoID INTEGER PRIMARY KEY, ident varchar(10) NOT NULL, latitude double NOT NULL, longitude double NOT NULL, name varchar(50) COLLATE NOCASE, ";
                Createsql += "city varchar(50) COLLATE NOCASE, state varchar(50) COLLATE NOCASE, num_runway_hard integer NOT NULL, num_runway_soft integer NOT NULL)";
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
            if (nNumVersionRows == 5)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@tblname", "AirportInfo");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_AirportInfo);
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
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "5");
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
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "500");
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
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "30");
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
            if (nNumOptionRows == 7)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "KMLFileNameTemplate");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "{Plane.Name}_{Flight.Start.Timestamp:M_d_yyyy h_mm_ss tt}");
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
            if (nNumOptionRows == 8)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "AirportListVersion");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "0");
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
            if (nNumOptionRows == 9)
            {
                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "AutomaticLoggingStopEngineOff");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "false");
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

        public int WriteFlightPoint(long pk, double latitude, double longitude, Int32 altitude)
        {
            SQLiteCommand sqlite_cmd;
            string Insertsql;
            SQLiteTransaction transaction = null;
            long FlightSampleID;

            sqlite_cmd = sqlite_conn.CreateCommand();
            transaction = sqlite_conn.BeginTransaction();
            Insertsql = "Insert into FlightSamples (FlightID, latitude, longitude, altitude, sample_datetimestamp) VALUES (@FlightID, @latitude, @longitude, @altitude, @sample_datetimestamp)";
            sqlite_cmd.CommandText = Insertsql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", pk);
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

        public void WriteFlightPointDetails(long pk, Int32 altitude_above_ground, Int32 engine1rpm, Int32 engine2rpm, Int32 engine3rpm, Int32 engine4rpm, Int32 lightsmask, double ground_velocity,
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
            sqlite_cmd.Parameters.AddWithValue("@FlightSamplesID", pk);
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
                if (waypoint.waypoint_valid == true)
                {
                    sqlite_cmd.Parameters.Clear();
                    sqlite_cmd.Parameters.AddWithValue("@FlightID", pk);
                    sqlite_cmd.Parameters.AddWithValue("@latitude", waypoint.gps_wp_latitude);
                    sqlite_cmd.Parameters.AddWithValue("@longitude", waypoint.gps_wp_longitude);
                    sqlite_cmd.Parameters.AddWithValue("@altitude", waypoint.gps_wp_altitude);

                    // verify that waypoint name is a valid string
                    Regex r = new Regex(@"[^\w\.@-]");
                    if (r.IsMatch(waypoint.gps_wp_name))
                        waypoint.gps_wp_name = Program.sInvalidWaypointName;

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

        // Gets either first or last entry in filght path for a given flight
        public FlightPathData GetFlightPathEntry(bool bFirst, int pk)
        {
            FlightPathData data = new FlightPathData();
            SQLiteCommand sqlite_cmd;
            string Selectsql;
            sqlite_cmd = sqlite_conn.CreateCommand();
            Selectsql = "SELECT cast(latitude as double), cast (longitude as double), altitude, sample_datetimestamp, alitutdeaboveground, engine1rpm, engine2rpm, engine3rpm, engine4rpm, lightsmask, ";
            Selectsql += "cast (ground_velocity as double), cast (plane_pitch as double), cast (plane_bank as double), cast (plane_heading_true as double), cast (plane_heading_magnetic as double), ";
            Selectsql += "cast (plane_airspeed_indicated as double), cast (airspeed_true as double), cast (vertical_speed as double), cast (heading_indicator as double), flaps_handle_position, ";
            Selectsql += "spoilers_handle_position, gear_handle_position, cast (ambient_wind_velocity as double), cast (ambient_wind_direction as double), cast (ambient_temperature as double),";
            Selectsql += "stall_warning, overspeed_warning, is_gear_retractable, spoiler_available, sim_on_ground ";
            Selectsql += "FROM FlightSamples, FlightSampleDetails WHERE FlightSampleDetails.FlightSamplesID = FlightSamples.FlightSamplesID AND FlightSamples.FlightSamplesID = ";

            if (bFirst == true)
                Selectsql += "(select min (FlightSamples.FlightSamplesID) from FlightSamples where FlightSamples.FlightID = @FlightID)";
            else
                Selectsql += "(select max (FlightSamples.FlightSamplesID) from FlightSamples where FlightSamples.FlightID = @FlightID)";

            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", pk);
            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                {
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
                }
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }
            return data;
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

        public FlightListData GetFlight(int nFlightID)
        {
            FlightListData FlightData = null;
            SQLiteCommand sqlite_cmd;
            string Selectsql;
            sqlite_cmd = sqlite_conn.CreateCommand();
            Selectsql = "SELECT FlightID, aircraft, start_datetimestamp FROM Flights WHERE FlightID = @FlightID";
            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.Clear();
            sqlite_cmd.Parameters.AddWithValue("@FlightID", nFlightID);

            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                {
                    FlightData = new FlightListData();
                    FlightData.FlightID = r.GetInt32(0);
                    FlightData.aircraft = r.GetString(1);
                    FlightData.start_flight_timestamp = r.GetInt64(2);
                }
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }
            return FlightData;
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

        // Loads airport information from CSV file into database
        public void LoadUpAirportInfo(string sFileName)
        {
            SQLiteCommand sqlite_cmd;
            SQLiteTransaction transaction = null;
            string sVersion;

            sqlite_cmd = sqlite_conn.CreateCommand();
            transaction = sqlite_conn.BeginTransaction();

            // delete all airport_info rows before loading new ones
            sqlite_cmd.CommandText = "Delete From AirportInfo";
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteNonQuery", sqlite_cmd), ex);
                throw ex;
            }

            // walk through CSV and load up all the data
            sqlite_cmd.CommandText = "INSERT INTO AirportInfo(ident, latitude, longitude, name, city, state, num_runway_hard, num_runway_soft) VALUES(@c0, @c1, @c2, @c3, @c4, @c5, @c6, @c7)";

            using (var fileStream = new FileStream(".\\" + sFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                using (CsvReader reader = new CsvReader(streamReader))
                {
                    reader.ValueSeparator = ',';
                    reader.ReadHeaderRecord();
                    while (reader.HasMoreRecords)
                    {
                        DataRecord record = reader.ReadDataRecord();
                        sqlite_cmd.Parameters.Clear();
                        for (int i = 0; i < reader.HeaderRecord.Count; i++)
                        {
                            sqlite_cmd.Parameters.AddWithValue("@c" + i, record[i]);
                        }
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
            }
            // Figure out what version the file was and write it to options
            sVersion = GetAirportListVersion(sFileName);
            WriteTableOption("AirportListVersion", sVersion);
            transaction.Commit();
        }

        // Returns version of the AirportList file based on fact that file is always named AirportListvx where xx is version number
        public string GetAirportListVersion(string sFileName)
        {
            int nStrPos;
            int nVerEndPos;
            string sVersion;

            nStrPos = sFileName.IndexOf('v', 0);
            nStrPos++;
            nVerEndPos = sFileName.IndexOf('.', nStrPos);
            sVersion = sFileName.Substring(nStrPos, nVerEndPos - nStrPos);

            return sVersion;
        }

        public AirportData GetClosestAirportToLatLong(bool bHardRunwayOnly, double Orig_Latitude, double Orig_Longitude)
        {
            AirportData apData = new AirportData();
            SQLiteCommand sqlite_cmd;
            string Selectsql;

            sqlite_conn.BindFunction(new SqrtFunction());
            sqlite_conn.BindFunction(new CosineFunction());
            sqlite_conn.BindFunction(new PowerSQLiteFunction());

            sqlite_cmd = sqlite_conn.CreateCommand();

            Selectsql = "select * from (select ident, latitude, longitude, name, city, state, num_runway_hard, num_runway_soft, ";
            Selectsql += "sqrt(power(69.1 * (latitude - @Orig_Latitude), 2) + power(69.1 * ((@Orig_longitude - longitude) * cos(latitude / 57.3)), 2)) AS distance FROM AirportInfo ";

            if (bHardRunwayOnly == true)
                Selectsql += "WHERE num_runway_hard > 0 ORDER BY distance) LIMIT 1";
            else
                Selectsql += "ORDER BY distance) LIMIT 1";
            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.Clear();
            sqlite_cmd.Parameters.AddWithValue("@Orig_longitude", Orig_Longitude);
            sqlite_cmd.Parameters.AddWithValue("@Orig_Latitude", Orig_Latitude);

            try
            {
                SQLiteDataReader r = sqlite_cmd.ExecuteReader();

                while (r.Read())
                {
                    apData.ident = r.GetString(0);
                    apData.latitude = r.GetDouble(1);
                    apData.longitude = r.GetDouble(2);
                    apData.name = r.GetString(3);
                    apData.city = r.GetString(4);
                    apData.state = r.GetString(5);
                    apData.num_runway_hard = r.GetInt32(6);
                    apData.num_runway_soft = r.GetInt32(7);
                    apData.distance = r.GetDouble(8);
                }
            }
            catch (Exception ex)
            {
                Program.ErrorLogging(GetDBQueryExtraInfo(System.Reflection.MethodBase.GetCurrentMethod().Name, "ExecuteReader", sqlite_cmd), ex);
                throw ex;
            }
            sqlite_conn.UnbindAllFunctions(false);

            return apData;
        }
    }
}


