using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FS2020PlanePath
{

    public interface IFlightDataConnector
    {
        bool IsSimConnected();
        void Connect();
        void GetSimEnvInfo();
        bool HandleWindowMessage(ref Message m);
        void CloseConnection();
    }

    public class FlightDataConnectorBuilder
    {
        FlightDataConnector flightDataConnector = new FlightDataConnector();

        public FlightDataConnectorBuilder inMode(string operationalMode)
        {
            flightDataConnector.SetMode(operationalMode);
            return this;
        }

        public FlightDataConnectorBuilder withConnectorFactory(string name, Func<IFlightDataConnector> connectorFactory)
        {
            flightDataConnector.AddConnectorFactory(name, connectorFactory);
            return this;
        }

        public FlightDataConnector build()
        {
            return flightDataConnector;
        }

    }

    public class FlightDataConnector : IFlightDataConnector
    {

        private Dictionary<string, Func<IFlightDataConnector>> ConnectorRegistry { get; } = new Dictionary<string, Func<IFlightDataConnector>>();

        public void AddConnectorFactory(string name, Func<IFlightDataConnector> connectorFactory)
        {
            ConnectorRegistry[name] = connectorFactory;
        }

        public void SetMode(string operationalMode)
        {
            if (IsSimConnected())
            {
                CloseConnection();
            }
            Console.WriteLine($"setting operationalMode({operationalMode})");
            this.operationalMode = operationalMode;
            exceptionMessage = default(string);
        }

        public string Mode => operationalMode;

        public string[] Diagnostics {
            get
            {
                if (exceptionMessage != default(string))
                {
                    return new string[] { exceptionMessage };
                }
                return new string[0];
            }
        }

        public void Connect()
        {
            Console.WriteLine($"connecting({operationalMode})");

            if (operationalMode == default(string) || !ConnectorRegistry.ContainsKey(operationalMode))
            {
                Console.WriteLine($"request to open in undefined operational mode '{operationalMode}' ignored");
                return;
            }

            try
            {
                exceptionMessage = default(string);
                delegateConnector = ConnectorRegistry[operationalMode].Invoke();
                delegateConnector.Connect();
            }
            catch (Exception e)
            {
                exceptionMessage = $"connection error: {e.Message}";
                CloseConnection();
            }

        }

        public void CloseConnection()
        {
            Console.WriteLine($"disconnecting({operationalMode})");
            try
            {
                if (IsSimConnected())
                {
                    delegateConnector.CloseConnection();
                }
            }
            catch (Exception e)
            {
                exceptionMessage = $"disconnect error: {e.Message}";
            }

            delegateConnector = default(IFlightDataConnector);
        }

        public bool IsSimConnected()
        {
            return delegateConnector != default(IFlightDataConnector);
        }

        public void GetSimEnvInfo()
        {
            try { 
                if (IsSimConnected())
                {
                    delegateConnector.GetSimEnvInfo();
                }
            } catch(Exception e)
            {
                exceptionMessage = $"get env error: {e.Message}";
            }
        }

        public bool HandleWindowMessage(ref Message m)
        {
            try
            {
                return IsSimConnected() && delegateConnector.HandleWindowMessage(ref m);
            }
            catch(Exception e)
            {
                exceptionMessage = $"message error: {e.Message}";
                return false;
            }
            
        }

        private IFlightDataConnector delegateConnector;
        private string operationalMode;
        private string exceptionMessage;
    }

    public struct FlightDataStructure
    {
        public long timestamp;
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
        public string gps_wp_prev_id;
        public double gps_wp_next_latitude;
        public double gps_wp_next_longitude;
        public Int32 gps_wp_next_altitude;
        public string gps_wp_next_id;
        public Int32 gps_flight_plan_wp_index;
        public Int32 gps_flight_plan_wp_count;
        public Int32 sim_on_ground;
    }

    public struct EnvironmentDataStructure
    {
        public string title;
    }

}
