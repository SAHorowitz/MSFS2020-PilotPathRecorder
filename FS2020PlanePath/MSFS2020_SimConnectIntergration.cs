using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.FlightSimulator.SimConnect;
using SharpKml.Dom;

namespace FS2020PlanePath
{
    public class MSFS2020_SimConnectIntergration
    {
        private /*static*/ SimConnect simConnect;
        public const int WM_USER_SIMCONNECT = 0x0402;
        private MainPage fForm;

        public MainPage FForm { get => fForm; set => fForm = value; }
        public /*static*/ SimConnect SimConnect { get => simConnect; }

        enum DATA_REQUESTS
        {
            DataRequest,
            SimEnvironmentReq,
            SUBSCRIBE_REQ,
            NONSUBSCRIBE_REQ,
        }

        enum EVENTS
        {
            ID0,
        };

        enum DEFINITIONS
        {
            DataStructure,
            SimEnvironmentDataStructure,
        }

        // this is how you declare a data structure so that
        // simconnect knows how to fill it/read it.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct DataStructure
        {
            public double latitude;
            public double longitude;
            public Int32 altitude;
            public Int32 altitude_above_ground;
            public Int32 engine1rpm;
            public Int32 engine2rpm;
            public Int32 engine3rpm;
            public Int32 engine4rpm;
            public Int32 lightsmask;
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
            public Int32 spoiler_avaialable;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct SimEnvironmentDataStructure
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string title;
        }

        public /*static*/ void CloseConnection()
        {
            if (simConnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close()
                simConnect.Dispose();
                simConnect = null;
            }
        }

        public bool Initialize()
        {
            try
            {
                SetupEvents();
                return true;
            }
            catch (COMException ex)
            {
                // eat exception and return false 
                return false;
            }
        }

        private /*static*/ void SetupEvents()
        {
            try
            {
                simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
                simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

                simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnRecvException);

                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Plane Alt Above Ground", "feet", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "General Eng RPM:1", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "General Eng RPM:2", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "General Eng RPM:3", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "General Eng RPM:4", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Light States", "mask", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Ground Velocity", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Plane Pitch Degrees", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Plane Bank Degrees", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Plane Heading Degrees Magnetic", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Airspeed Indicated", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Airspeed True", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Vertical Speed", "feet per minute", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Heading Indicator", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Flaps Handle Index", "number", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Spoilers Handle Position", "position", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Gear Handle Position", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Ambient Wind Velocity", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Ambient Wind Direction", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Ambient Temperature", "celsius", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Stall Warning", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Overspeed Warning", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Is Gear Retractable", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnect.AddToDataDefinition(DEFINITIONS.DataStructure, "Spoiler Available", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                SimConnect.AddToDataDefinition(DEFINITIONS.SimEnvironmentDataStructure, "TITLE", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                // IMPORTANT: register it with the simconnect managed wrapper marshaller
                // if you skip this step, you will only receive a uint in the .dwData field.
                simConnect.RegisterDataDefineStruct<DataStructure>(DEFINITIONS.DataStructure);
                SimConnect.RegisterDataDefineStruct<SimEnvironmentDataStructure> (DEFINITIONS.SimEnvironmentDataStructure);

                simConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(SimConnect_OnRecvSimobjectData);
                simConnect.RequestDataOnSimObject(DATA_REQUESTS.SimEnvironmentReq, DEFINITIONS.SimEnvironmentDataStructure, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
                simConnect.RequestFacilitiesList(SIMCONNECT_FACILITY_LIST_TYPE.AIRPORT, DATA_REQUESTS.NONSUBSCRIBE_REQ);
                simConnect.RequestFacilitiesList(SIMCONNECT_FACILITY_LIST_TYPE.VOR, DATA_REQUESTS.NONSUBSCRIBE_REQ);

            }
            catch (COMException ex)
            {
                // currently eat exceptions
            }
        }

        // don't currently do anything with receiving exception event
        void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            // return "Exception received: " + data.dwException;
        }

        // don't currently do anything with receiving open event
        void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
        }

        
        void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            CloseConnection();
        }

        // when receiving object data first asked for simenvironment info to get aircraft title one time
        // once that is received then ask once a second for aircraft info
        void SimConnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            switch ((DATA_REQUESTS)data.dwRequestID)
            {
                case DATA_REQUESTS.DataRequest:
                    DataStructure s1 = (DataStructure)data.dwData[0];

                    FForm.UseData(s1.latitude, s1.longitude, s1.altitude, s1.altitude_above_ground, s1.engine1rpm, s1.engine2rpm, s1.engine3rpm, s1.engine4rpm, s1.lightsmask,
                                  s1.ground_velocity, s1.plane_pitch, s1.plane_bank, s1.plane_heading_true, s1.plane_heading_magnetic, s1.plane_airspeed_indicated, 
                                  s1.airspeed_true, s1.vertical_speed, s1.heading_indicator, s1.flaps_handle_position, s1.spoilers_handle_position,
                                  Convert.ToBoolean(s1.gear_handle_position), s1.ambient_wind_velocity, s1.ambient_wind_direction, s1.ambient_temperature, 
                                  Convert.ToBoolean(s1.stall_warning), Convert.ToBoolean(s1.overspeed_warning), Convert.ToBoolean(s1.is_gear_retractable), Convert.ToBoolean(s1.spoiler_avaialable));
                    break;


                case DATA_REQUESTS.SimEnvironmentReq:
                    SimEnvironmentDataStructure s2 = (SimEnvironmentDataStructure)data.dwData[0];
                    FForm.UseSimEnvData(s2.title); 
                    simConnect.RequestDataOnSimObject(DATA_REQUESTS.DataRequest, DEFINITIONS.DataStructure, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND , SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
                    break;

                default:
                    break;
            }
        }

        public bool Connect()
        {
            if (simConnect != null)
                return false;

            try
            {

                simConnect = new SimConnect("MainPage", fForm.Handle, WM_USER_SIMCONNECT, null, 0);
                return true;
            }
            catch (COMException ex)
            {
                return false;
            }
        }

        public void Disconnect()
        {
            if (simConnect != null)
            {
                simConnect.Dispose();
                simConnect = null;
            }
        }

        public bool IsSimConnected()
        {
            return (SimConnect != null);
        }
    }
 }
