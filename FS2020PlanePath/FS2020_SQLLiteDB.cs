using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FS2020PlanePath
{
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
        Int32 FlightID;
        string aircraft;
        long start_flight_timestamp;

        public int FlightID1 { get => FlightID; set => FlightID = value; }
        public string Aircraft { get => aircraft; set => aircraft = value; }
        public long Start_flight_timestamp { get => start_flight_timestamp; set => start_flight_timestamp = value; }
    }

    class FS2020_SQLLiteDB
    {
        SQLiteConnection sqlite_conn;
        private const int TblVersion_Flights = 1;
        private const int TblVersion_FlightSamples = 1;
        private const int TblVersion_FlightSampleDetails = 1;
        private const int TblVersion_FlightOptions = 1;

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
            }
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
                sqlite_cmd.ExecuteNonQuery();
            }

            if (CheckTableExists("TblVersions") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE TblVersions (tblVersionID INTEGER PRIMARY KEY, tblname varchar(256), tblversion int)";
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }


            if (CheckTableExists("FlightSamples") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE FlightSamples (FlightSamplesID INTEGER PRIMARY KEY, FlightID integer NOT NULL, latitude double, longitude double, altitude int32, sample_datetimestamp long,  ";
                Createsql += "FOREIGN KEY (FlightID) REFERENCES Flight(FlightID) )";
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }

            if (CheckTableExists("FlightSampleDetails") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE FlightSampleDetails (FlightSampleDetailsID INTEGER PRIMARY KEY, FlightSamplesID long NOT NULL, alitutdeaboveground int32, engine1rpm int32, engine2rpm int32, ";
                Createsql += "engine3rpm int32, engine4rpm int32, lightsmask int32, ground_velocity double, plane_pitch double, plane_bank double, plane_heading_true double, ";
                Createsql += "plane_heading_magnetic double, plane_airspeed_indicated double, airspeed_true double, vertical_speed double, heading_indicator double, flaps_handle_position int32, ";
                Createsql += "spoilers_handle_position int32, gear_handle_position int32, ambient_wind_velocity double, ambient_wind_direction double, ambient_temperature double, ";
                Createsql += "stall_warning int32, overspeed_warning int32, is_gear_retractable int32, spoiler_available int32, FOREIGN KEY (FlightSamplesID) REFERENCES FlightSamples(FlightSamplesID) )";
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }

            if (CheckTableExists("FlightPathOptions") == false)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                Createsql = "CREATE TABLE FlightPathOptions (OptionsID INTEGER PRIMARY KEY, optionname varchar(256), optionvalue varchar(512))";
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }
            // Fill in Table Versions if needed
            LoadUpTableVersions();

            // Load Up Table Options if needed
            LoadUpTableOptions();
        }

        private void LoadUpTableVersions()
        {
            SQLiteCommand sqlite_cmd;
            string selectsql;

            sqlite_cmd = sqlite_conn.CreateCommand();
            selectsql = "SELECT COUNT(tblVersionID) from TblVersions";
            sqlite_cmd.CommandText = selectsql;
            
            // if there are no table rows then we need to fill them
            if (Convert.ToInt32(sqlite_cmd.ExecuteScalar()) == 0)
            {
                string insertsql;

                sqlite_cmd = sqlite_conn.CreateCommand();
                insertsql = "INSERT INTO TblVersions (tblname, tblversion) VALUES (@tblname, @tblversion)";
                sqlite_cmd.CommandText = insertsql;
                sqlite_cmd.Parameters.AddWithValue("@tblname", "Flights");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_Flights);
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@tblname", "FlightSamples");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_FlightSamples);
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@tblname", "FlightSampleDetails");
                sqlite_cmd.Parameters.AddWithValue("@tblversion", TblVersion_FlightOptions);
                sqlite_cmd.ExecuteNonQuery();
            }
        }

        private void LoadUpTableOptions()
        {
            SQLiteCommand sqlite_cmd;
            string selectsql;

            sqlite_cmd = sqlite_conn.CreateCommand();
            selectsql = "SELECT COUNT(OptionsID) from FlightPathOptions";
            sqlite_cmd.CommandText = selectsql;

            // if there are no table rows then we need to fill them
            if (Convert.ToInt32(sqlite_cmd.ExecuteScalar()) == 0)
            {
                string insertsql;

                sqlite_cmd = sqlite_conn.CreateCommand();
                insertsql = "INSERT INTO FlightPathOptions (optionname, optionvalue) VALUES (@optionname, @optionvalue)";
                sqlite_cmd.CommandText = insertsql;
                sqlite_cmd.Parameters.AddWithValue("@optionname", "AboveThresholdWriteFreq");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "5");
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "ThresholdMinAltitude");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "500");
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmd.Parameters.Clear();
                sqlite_cmd.Parameters.AddWithValue("@optionname", "KMLFilePath");
                sqlite_cmd.Parameters.AddWithValue("@optionvalue", "");
                sqlite_cmd.ExecuteNonQuery();
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
            SQLiteDataReader r = sqlite_cmd.ExecuteReader();
            while (r.Read())
                sRetval = r.GetString(0);

            return (sRetval);
        }

        public String WriteTableOption(String optionname, String optionvalue)
        {
            SQLiteCommand sqlite_cmd;
            string Updatesql;
            string sRetval = "";

            sqlite_cmd = sqlite_conn.CreateCommand();
            Updatesql = "Update FlightPathOptions SET optionvalue = @optionvalue WHERE optionname  = @optionname";
            sqlite_cmd.CommandText = Updatesql;
            sqlite_cmd.Parameters.AddWithValue("@optionname", optionname);
            sqlite_cmd.Parameters.AddWithValue("@optionvalue", optionvalue);
            SQLiteDataReader r = sqlite_cmd.ExecuteReader();
            while (r.Read())
                sRetval = r.GetString(0);

            return (sRetval);
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
            SQLiteDataReader r = sqlite_cmd.ExecuteReader();
            while (r.Read())
                bRetVal = true;

            return (bRetVal);
        }

        public long WriteFlight(string aircraft)
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
            sqlite_cmd.ExecuteNonQuery();
            FlightID = sqlite_conn.LastInsertRowId;
            transaction.Commit();

            return FlightID;
        }

        public long WriteFlightPoint(long pk, double latitude, double longitude, Int32 altitude)
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
            sqlite_cmd.ExecuteNonQuery();
            FlightSampleID = sqlite_conn.LastInsertRowId;
            transaction.Commit();

            return FlightSampleID;
        }

        public void WriteFlightPointDetails(long pk, Int32 altitude_above_ground, Int32 engine1rpm, Int32 engine2rpm, Int32 engine3rpm, Int32 engine4rpm, Int32 lightsmask, double ground_velocity,
                                            double plane_pitch, double plane_bank, double plane_heading_true, double plane_heading_magnetic,
                                            double plane_airspeed_indicated, double airspeed_true, double vertical_speed, double heading_indicator,
                                            Int32 flaps_handle_position, Int32 spoilers_handle_position, Int32 gear_handle_position,
                                            double ambient_wind_velocity, double ambient_wind_direction, double ambient_temperature, Int32 stall_warning,
                                            Int32 overspeed_warning, Int32 is_gear_retractable, Int32 spoiler_available)
        {
            SQLiteCommand sqlite_cmd;
            string Insertsql;

            sqlite_cmd = sqlite_conn.CreateCommand();
            Insertsql = "Insert into FlightSampleDetails (FlightSamplesID, alitutdeaboveground, engine1rpm, engine2rpm, engine3rpm, engine4rpm, lightsmask, ground_velocity, plane_pitch, plane_bank, plane_heading_true, ";
            Insertsql += "plane_heading_magnetic, plane_airspeed_indicated, airspeed_true, vertical_speed, heading_indicator, flaps_handle_position, spoilers_handle_position, gear_handle_position, ambient_wind_velocity, ";
            Insertsql += "ambient_wind_direction, ambient_temperature, stall_warning, overspeed_warning, is_gear_retractable, spoiler_available) VALUES (@FlightSamplesID, @alitutdeaboveground, @engine1rpm, @engine2rpm, ";
            Insertsql += "@engine3rpm, @engine4rpm, @lightsmask, @ground_velocity, @plane_pitch, @plane_bank, @plane_heading_true, @plane_heading_magnetic, @plane_airspeed_indicated, @airspeed_true, @vertical_speed, ";
            Insertsql += "@heading_indicator, @flaps_handle_position, @spoilers_handle_position, @gear_handle_position, @ambient_wind_velocity, @ambient_wind_direction, @ambient_temperature, @stall_warning, ";
            Insertsql += "@overspeed_warning, @is_gear_retractable, @spoiler_available)";
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
            sqlite_cmd.ExecuteNonQuery();
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
            Selectsql += "stall_warning, overspeed_warning, is_gear_retractable, spoiler_available ";
            Selectsql += "FROM FlightSamples, FlightSampleDetails WHERE FlightSampleDetails.FlightSamplesID = FlightSamples.FlightSamplesID AND FlightID = @FlightID";
            sqlite_cmd.CommandText = Selectsql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", pk);
            SQLiteDataReader r = sqlite_cmd.ExecuteReader();
            int n = 0;
            while (r.Read())
            {
                n++;
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

                FlightPath.Add(data);
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
            SQLiteDataReader r = sqlite_cmd.ExecuteReader();
            int n = 0;
            while (r.Read())
            {
                n++;
                FlightListData data = new FlightListData();
                data.FlightID1 = r.GetInt32(0);
                data.Aircraft = r.GetString(1);
                data.Start_flight_timestamp = r.GetInt64(2);
                FlightList.Add(data);
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
            Deletesql = "Delete from FlightSampleDetails WHERE FlightSampleDetails.FlightSamplesID IN (select FlightSamplesID From FlightSamples WHERE FlightID = @FlightID)";
            sqlite_cmd.CommandText = Deletesql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", nFlightID);
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd = sqlite_conn.CreateCommand();
            Deletesql = "Delete from FlightSamples WHERE FlightID = @FlightID";
            sqlite_cmd.CommandText = Deletesql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", nFlightID);
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd = sqlite_conn.CreateCommand();
            Deletesql = "Delete from Flights WHERE FlightID = @FlightID";
            sqlite_cmd.CommandText = Deletesql;
            sqlite_cmd.Parameters.AddWithValue("@FlightID", nFlightID);
            sqlite_cmd.ExecuteNonQuery();
            transaction.Commit();
        }
    }
}

