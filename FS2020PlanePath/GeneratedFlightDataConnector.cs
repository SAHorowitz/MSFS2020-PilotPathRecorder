using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace FS2020PlanePath
{

    public interface IFlightDataGenerator
    {
        string Name { get; }
        IEnumerator<FlightPathData> NextFlightPathSegment();
    }

    public class GeneratedFlightDataConnector : IFlightDataConnector
    {

        public GeneratedFlightDataConnector(
            Control parentControl,
            Action<FlightDataStructure> simPlaneDataHandler,
            Action<EnvironmentDataStructure> simPlaneEnvironmentChangeHandler,
            IFlightDataGenerator flightDataGenerator
        )
        {
            this.parentControl = parentControl;
            this.simPlaneDataHandler = simPlaneDataHandler;
            this.simPlaneEnvironmentChangeHandler = simPlaneEnvironmentChangeHandler;
            this.flightDataGenerator = flightDataGenerator;
            timer = new Timer();
            timer.Enabled = false;
            // NOTE: should match configuration of the SimConnect connector (i.e., once per second)
            // (see: SIMCONNECT_PERIOD.SECOND)
            timer.Interval = 1000;  
            timer.Tick += TimerTickHandler;
        }

        public void GetSimEnvInfo()
        {
            simPlaneEnvironmentChangeHandler.Invoke(
                new EnvironmentDataStructure
                {
                    title = flightDataGenerator.Name
                }
            );
        }

        public bool IsSimConnected()
        {
            return timer.Enabled;
        }

        public void Connect()
        {
            if (!IsSimConnected())
            {
                timer.Enabled = true;
                timer.Start();
            }
        }

        public void CloseConnection()
        {
            if (IsSimConnected())
            {
                timer.Stop();
                timer.Enabled = false;
            }
        }

        public bool IsSimInitialized()
        {
            return timer.Enabled;
        }

        public bool HandleWindowMessage(ref Message m)
        {
            if (IsSimInitialized())
            {
                if (m.Msg == WM_USER_SIMCONNECT)
                {
                    pumpFlightDataUpdates();
                    return true;
                }
            }
            return false;
        }

        private void TimerTickHandler(object sender, EventArgs e)
        {
            UserDialogUtils.PostMessage(parentControl.Handle, WM_USER_SIMCONNECT, 0, 0);
        }

        private void pumpFlightDataUpdates()
        {
            IEnumerator<FlightPathData> flightPathDataEnumerator = flightDataGenerator.NextFlightPathSegment();
            while (flightPathDataEnumerator.MoveNext())
            {
                simPlaneDataHandler.Invoke(FpdToFds(flightPathDataEnumerator.Current));
            }
        }

        private static FlightDataStructure FpdToFds(FlightPathData fpd)
        {
            return new FlightDataStructure
            {
                timestamp = fpd.timestamp,
                latitude = fpd.latitude,
                longitude = fpd.longitude,
                altitude = fpd.altitude,
                altitude_above_ground = fpd.altitudeaboveground,
                engine1rpm = fpd.Eng1Rpm,
                engine2rpm = fpd.Eng2Rpm,
                engine3rpm = fpd.Eng3Rpm,
                engine4rpm = fpd.Eng4Rpm,
                lightsmask = fpd.LightsMask,
                ground_velocity = fpd.ground_velocity,
                plane_pitch = fpd.plane_pitch,
                plane_bank = fpd.plane_bank,
                plane_heading_true = fpd.plane_heading_true,
                plane_heading_magnetic = fpd.plane_heading_magnetic,
                plane_airspeed_indicated = fpd.plane_airspeed_indicated,
                airspeed_true = fpd.airspeed_true,
                vertical_speed = fpd.vertical_speed,
                heading_indicator = fpd.heading_indicator,
                flaps_handle_position = fpd.flaps_handle_position,
                spoilers_handle_position = fpd.spoilers_handle_position,
                gear_handle_position = fpd.gear_handle_position,
                ambient_wind_velocity = fpd.ambient_wind_velocity,
                ambient_wind_direction = fpd.ambient_wind_direction,
                ambient_temperature = fpd.ambient_temperature,
                stall_warning = fpd.stall_warning,
                overspeed_warning = fpd.overspeed_warning,
                is_gear_retractable = fpd.is_gear_retractable,
                spoiler_available = fpd.spoiler_available,
                sim_on_ground = fpd.sim_on_ground,

                //
                //  No flight plan, so no values for the following...
                //

                //gps_wp_prev_latitude = null,
                //gps_wp_prev_longitude = null,
                //gps_wp_prev_altitude = null,
                //gps_wp_prev_id = null,

                //gps_wp_next_latitude = null,
                //gps_wp_next_longitude = null,
                //gps_wp_next_altitude = null,
                //gps_wp_next_id = null,

                //gps_flight_plan_wp_index = null,
                //gps_flight_plan_wp_count = null,

            };
        }

        private const int WM_USER_SIMCONNECT = 0x0402;

        private Timer timer;
        private Control parentControl;
        private Action<FlightDataStructure> simPlaneDataHandler;
        private Action<EnvironmentDataStructure> simPlaneEnvironmentChangeHandler;
        private IFlightDataGenerator flightDataGenerator;

    }

}
