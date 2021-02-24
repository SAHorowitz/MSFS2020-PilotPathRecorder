using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.FlightSimulator.SimConnect;

namespace FS2020PlanePath
{
    public class SimConnectFlightDataConnector : IFlightDataConnector
    {

        private const int WM_USER_SIMCONNECT = 0x0402;

        private SimConnect simConnect;
        private Control parentControl;
        private Action<FlightDataStructure> flightDataHandler;
        private Action<EnvironmentDataStructure> environmentDataHandler;

        enum DATA_REQUESTS
        {
            DataRequest,
            SimEnvironmentReq,
        }

        enum EVENTS
        {
            FlightPlanActivated,
            FlightPlanDeactivated,
            SimStop,
            SimStart,
        };

        enum DEFINITIONS
        {
            SimPlaneDataStructure,
            SimEnvironmentDataStructure,
        }

        // this is how you declare a data structure so that
        // simconnect knows how to fill it/read it.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct SimPlaneDataStructure
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
            public Int32 spoiler_available;
            public double gps_wp_prev_latitude;
            public double gps_wp_prev_longitude;
            public Int32 gps_wp_prev_altitude;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string gps_wp_prev_id;
            public double gps_wp_next_latitude;
            public double gps_wp_next_longitude;
            public Int32 gps_wp_next_altitude;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string gps_wp_next_id;
            public Int32 gps_flight_plan_wp_index;
            public Int32 gps_flight_plan_wp_count;
            public Int32 sim_on_ground;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct SimEnvironmentDataStructure
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string title;
        }

        public SimConnectFlightDataConnector(
            Control parentControl,
            Action<FlightDataStructure> flightDataHandler,
            Action<EnvironmentDataStructure> environmentDataHandler
        )
        {
            this.parentControl = parentControl;
            this.flightDataHandler = flightDataHandler;
            this.environmentDataHandler = environmentDataHandler;
        }

        public void CloseConnection()
        {
            if (simConnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close()
                simConnect.Dispose();
                simConnect = null;
            }
        }

        public bool HandleWindowMessage(ref Message m)
        {
            if (m.Msg == SimConnectFlightDataConnector.WM_USER_SIMCONNECT)
            {
                if (simConnect != null)
                {
                    simConnect.ReceiveMessage();
                }
                return true;
            }
            return false;
        }

        private void SetupEvents()
        {

            simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
            simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

            simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnRecvException);

            // for future use
            /*                SimConnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(SimConnect_OnRecvEvent);
                            SimConnect.OnRecvEventFilename += new SimConnect.RecvEventFilenameEventHandler(SimConnect_OnRecvFilename);*/

            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Plane Alt Above Ground", "feet", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "General Eng RPM:1", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "General Eng RPM:2", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "General Eng RPM:3", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "General Eng RPM:4", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Light States", "mask", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Ground Velocity", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Plane Pitch Degrees", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Plane Bank Degrees", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Plane Heading Degrees Magnetic", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Airspeed Indicated", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Airspeed True", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Vertical Speed", "feet per minute", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Heading Indicator", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Flaps Handle Index", "number", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Spoilers Handle Position", "position", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Gear Handle Position", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Ambient Wind Velocity", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Ambient Wind Direction", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Ambient Temperature", "celsius", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Stall Warning", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Overspeed Warning", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Is Gear Retractable", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Spoiler Available", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS WP Prev Lat", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS WP Prev Lon", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS WP Prev ALT", "feet", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS WP Prev ID", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS WP Next Lat", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS WP Next Lon", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS WP Next ALT", "feet", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS WP NEXT ID", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS Flight Plan WP Index", "number", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "GPS Flight Plan WP Count", "number", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.SimPlaneDataStructure, "Sim On Ground", "bool", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simConnect.AddToDataDefinition(DEFINITIONS.SimEnvironmentDataStructure, "TITLE", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // IMPORTANT: register it with the simconnect managed wrapper marshaller
            // if you skip this step, you will only receive a uint in the .dwData field.
            simConnect.RegisterDataDefineStruct<SimPlaneDataStructure>(DEFINITIONS.SimPlaneDataStructure);
            simConnect.RegisterDataDefineStruct<SimEnvironmentDataStructure>(DEFINITIONS.SimEnvironmentDataStructure);

            simConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(SimConnect_OnRecvSimobjectData);
            simConnect.RequestDataOnSimObject(DATA_REQUESTS.DataRequest, DEFINITIONS.SimPlaneDataStructure, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

            // for future use
            /*              simConnect.SubscribeToSystemEvent(EVENTS.FlightPlanActivated, "FlightPlanActivated");
                            simConnect.SubscribeToSystemEvent(EVENTS.FlightPlanDeactivated, "FlightPlanDeactivated");
                            simConnect.SubscribeToSystemEvent(EVENTS.SimStop, "SimStop");
                            simConnect.SubscribeToSystemEvent(EVENTS.SimStart, "SimStart");
                            simConnect.MapClientEventToSimEvent(EVENTS.FlightPlanActivated, "");
                            SimConnect.SetSystemEventState(EVENTS.FlightPlanActivated, SIMCONNECT_STATE.OFF);
                */
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

        // for future use
        /*        void SimConnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
                {
                    switch ((EVENTS)(data.uEventID))
                    {
                        case EVENTS.FlightPlanActivated:
                            int n = 5;
                            break;

                        case EVENTS.FlightPlanDeactivated:
                            int aa = 7;
                            break;

                        case EVENTS.SimStop:
                            int x = 6;
                            break;

                        case EVENTS.SimStart:
                            int z = 7;
                            break;


                        default:
                            break;
                    }

                }*/

        // for future use
        /*        void SimConnect_OnRecvFilename(SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME data)
                {
                    switch ((EVENTS)(data.uEventID))
                    {
                        case EVENTS.FlightPlanActivated:
                            int n = 5;
                            break;

                        default:
                            break;
                    }
                }*/

        // when receiving object data first asked for simenvironment info to get aircraft title one time
        // once that is received then ask once a second for aircraft info
        void SimConnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            switch ((DATA_REQUESTS) data.dwRequestID)
            {
                case DATA_REQUESTS.DataRequest:
                    flightDataHandler.Invoke(FlightData((SimPlaneDataStructure) data.dwData[0]));
                    break;

                case DATA_REQUESTS.SimEnvironmentReq:
                    environmentDataHandler.Invoke(EnvironmentData((SimEnvironmentDataStructure) data.dwData[0]));
                    break;

                default:
                    break;
            }
        }

        public void GetSimEnvInfo()
        {
            simConnect.RequestDataOnSimObject(DATA_REQUESTS.SimEnvironmentReq, DEFINITIONS.SimEnvironmentDataStructure, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
        }

        public void Connect()
        {
            if (simConnect != null)
            {
                CloseConnection();
            }

            simConnect = new SimConnect("MainPage", parentControl.Handle, WM_USER_SIMCONNECT, null, 0);
            SetupEvents();
        }

/*        public void Disconnect()
        {
            CloseConnection();
            if (simConnect != null)
            {
                b
                simConnect.Dispose();
                simConnect = null;
            }
        }*/

        public bool IsSimConnected()
        {
            return simConnect != default(SimConnect);
        }


        private static EnvironmentDataStructure EnvironmentData(SimEnvironmentDataStructure newSimConnectEnvironmentStructure)
        {
            return new EnvironmentDataStructure
            {
                title = newSimConnectEnvironmentStructure.title
            };
        }

        private static FlightDataStructure FlightData(SimPlaneDataStructure newSimConnectData)
        {
            return new FlightDataStructure
            {
                latitude = newSimConnectData.latitude,
                longitude = newSimConnectData.longitude,
                altitude = newSimConnectData.altitude,
                altitude_above_ground = newSimConnectData.altitude_above_ground,
                engine1rpm = newSimConnectData.engine1rpm,
                engine2rpm = newSimConnectData.engine2rpm,
                engine3rpm = newSimConnectData.engine3rpm,
                engine4rpm = newSimConnectData.engine4rpm,
                lightsmask = newSimConnectData.lightsmask,
                ground_velocity = newSimConnectData.ground_velocity,
                plane_pitch = newSimConnectData.plane_pitch,
                plane_bank = newSimConnectData.plane_bank,
                plane_heading_true = newSimConnectData.plane_heading_true,
                plane_heading_magnetic = newSimConnectData.plane_heading_magnetic,
                plane_airspeed_indicated = newSimConnectData.plane_airspeed_indicated,
                airspeed_true = newSimConnectData.airspeed_true,
                vertical_speed = newSimConnectData.vertical_speed,
                heading_indicator = newSimConnectData.heading_indicator,
                flaps_handle_position = newSimConnectData.flaps_handle_position,
                spoilers_handle_position = newSimConnectData.spoilers_handle_position,
                gear_handle_position = newSimConnectData.gear_handle_position,
                ambient_wind_velocity = newSimConnectData.ambient_wind_velocity,
                ambient_wind_direction = newSimConnectData.ambient_wind_direction,
                ambient_temperature = newSimConnectData.ambient_temperature,
                stall_warning = newSimConnectData.stall_warning,
                overspeed_warning = newSimConnectData.overspeed_warning,
                is_gear_retractable = newSimConnectData.is_gear_retractable,
                spoiler_available = newSimConnectData.spoiler_available,
                gps_wp_prev_latitude = newSimConnectData.gps_wp_prev_latitude,
                gps_wp_prev_longitude = newSimConnectData.gps_wp_prev_longitude,
                gps_wp_prev_altitude = newSimConnectData.gps_wp_prev_altitude,
                gps_wp_prev_id = newSimConnectData.gps_wp_prev_id,
                gps_wp_next_latitude = newSimConnectData.gps_wp_next_latitude,
                gps_wp_next_longitude = newSimConnectData.gps_wp_next_longitude,
                gps_wp_next_altitude = newSimConnectData.gps_wp_next_altitude,
                gps_wp_next_id = newSimConnectData.gps_wp_next_id,
                gps_flight_plan_wp_index = newSimConnectData.gps_flight_plan_wp_index,
                gps_flight_plan_wp_count = newSimConnectData.gps_flight_plan_wp_count,
                sim_on_ground = newSimConnectData.sim_on_ground
            };
        }

    }

}
