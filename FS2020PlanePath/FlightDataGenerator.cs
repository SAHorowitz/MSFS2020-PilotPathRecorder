using System;
using System.Linq;
using System.Collections.Generic;

namespace FS2020PlanePath
{

    public class RandomWalkFlightAdvancerParameters
    {
        public double HeadingChangeScale { get; set; } = 1;
        public double VelocityChangeScale { get; set; } = 1;
        public double AltitudeChangeScale { get; set; } = 1;
        public long SamplesPerSecond { get; set; } = 2;
        public int RollingCount { get; set; } = 5;
    }

    public class RandomWalkFlightAdvancer
    {

        public RandomWalkFlightAdvancer(RandomWalkFlightAdvancerParameters randomWalkFlightParameters)
        {
            this.randomWalkFlightParameters = randomWalkFlightParameters;
            ticksPerAdvance = (long) ((TimeSpan.TicksPerSecond / randomWalkFlightParameters.SamplesPerSecond) + 0.5);
            secondsPerSample = ((double) ticksPerAdvance) / TimeSpan.TicksPerSecond;
            random = new Random();
            int rollingCount = this.randomWalkFlightParameters.RollingCount;
            heading = new MovingAverage(rollingCount);
            altitude = new MovingAverage(rollingCount);
            velocity = new MovingAverage(rollingCount);
        }

        public void advance(FlightPathData flightPathData)
        {

            double rawHeadingChange = randomWalkFlightParameters.HeadingChangeScale * centeredRandom();
            double newRawHeading = flightPathData.plane_heading_true + rawHeadingChange;
            double newRationalizedRawHeading = GeoCalcUtils.rationalizedCompassDirection(newRawHeading);
            double newHeading = heading.next(newRationalizedRawHeading);
            double headingChange = flightPathData.plane_heading_true - newHeading;
            flightPathData.plane_heading_true = newHeading;
            flightPathData.plane_heading_magnetic = newHeading;
            flightPathData.heading_indicator = newHeading;

            double rawVelocityChange = randomWalkFlightParameters.VelocityChangeScale * centeredRandom();
            double newRawVelocity = flightPathData.ground_velocity + rawVelocityChange;
            double newRationalizedRawVelocity = rationalizedVelocity(newRawVelocity);
            double newGroundVelocity = velocity.next(newRationalizedRawVelocity);
            flightPathData.ground_velocity = newGroundVelocity;
            flightPathData.airspeed_true = newGroundVelocity;

            double rawAltitudeChange = randomWalkFlightParameters.AltitudeChangeScale * centeredRandom();
            double newRawAltitude = flightPathData.altitude + rawAltitudeChange;
            double newRationalizedRawAltitude = rationalizedAltitude(newRawAltitude);
            double newAltitude = altitude.next(newRationalizedRawAltitude);
            double altitudeChange = flightPathData.altitude - newAltitude;
            flightPathData.altitude = (int) newAltitude;

            double distancePerTick = newGroundVelocity / TICKS_PER_HOUR;
            double distanceChange = distancePerTick * ticksPerAdvance;

            (double lat, double lon) newLatLon = GeoCalcUtils.calcLatLonOffset(
                GeoCalcUtils.normalizedIso6709GeoDirection(flightPathData.latitude),
                GeoCalcUtils.normalizedIso6709GeoDirection(flightPathData.longitude),
                newHeading,
                distanceChange
            );
            //Console.WriteLine($"p({GeoCalcUtils.pcoord(newLatLon)}");
            flightPathData.latitude = newLatLon.lat;
            flightPathData.longitude = newLatLon.lon;

            double altitudeChangePerSecond = altitudeChange / secondsPerSample;
            flightPathData.vertical_speed = (altitudeChangePerSecond / 60);

            // https://www.skybrary.aero/index.php/Rate_of_Turn#:~:text=The%20bank%20angle%20required%20to,10%20and%20then%20adding%207.
            double headingChangePerSecond = headingChange / secondsPerSample;
            double newRawPlaneBankAngle = ((flightPathData.airspeed_true / 10) + 7) * headingChangePerSecond / 3;
            flightPathData.plane_bank = rationalizeBankAngle(newRawPlaneBankAngle);
            //Console.WriteLine($"b{flightPathData.plane_bank} h{headingChangePerSecond}");

            // https://www.cfinotebook.net/notebook/aerodynamics-and-performance/performance-calculations
            double rawPlanePitchAngle = altitudeChangePerSecond / (100 * flightPathData.airspeed_true);
            flightPathData.plane_pitch = rationalizePitchAngle(rawPlanePitchAngle);
            //Console.WriteLine($"p{flightPathData.plane_pitch} a{altitudeChangePerSecond}");

            flightPathData.timestamp += ticksPerAdvance;

        }

        private double rationalizePitchAngle(double pitchAngle)
        {
            return (
                pitchAngle < -30 ? -30
              : pitchAngle > 30 ? 30
              : pitchAngle
            );
        }

        private double rationalizeBankAngle(double bankAngle)
        {
            return (
                bankAngle < -60 ? -60
              : bankAngle > 60 ? 60
              : bankAngle
            );
        }

        private double rationalizedAltitude(double altitude)
        {
            return (
                altitude < 0 ? 0
              : altitude > 60000 ? 60000
              : altitude
            );
        }

        private double rationalizedVelocity(double velocity)
        {
            return (
                velocity < 0 ? 0
              : velocity > 250 ? 250
              : velocity
            );
        }

        /// <returns>pseudo-random number evenly distributed across the range -1 < x < 1</returns>
        private double centeredRandom()
        {
            return 2 * (random.NextDouble() - 0.5);
        }

        private const long TICKS_PER_HOUR = TimeSpan.TicksPerSecond * 3600;

        private Random random;
        private long ticksPerAdvance;
        private double secondsPerSample;
        private RandomWalkFlightAdvancerParameters randomWalkFlightParameters;
        private MovingAverage heading;
        private MovingAverage altitude;
        private MovingAverage velocity;
    }

    public class MovingAverage
    {
        private double[] window;
        private int n, insert;
        private double sum;

        public MovingAverage(int size)
        {
            window = new double[size];
            insert = 0;
            sum = 0;
        }

        public double next(double val)
        {
            if (n < window.Length)
            {
                n++;
            }
            sum -= window[insert];
            sum += val;
            window[insert] = val;
            insert = (insert + 1) % window.Length;
            return (double) sum / n;
        }
    }

    public static class FlightPathDataUtils
    {

        public static FlightPathData CopyOf(FlightPathData source)
        {
            return new FlightPathData
            {
                timestamp = source.timestamp,
                longitude = source.longitude,
                latitude = source.latitude,
                altitude = source.altitude,
                plane_pitch = source.plane_pitch,
                plane_bank = source.plane_bank,
                plane_heading_true = source.plane_heading_true,
                ground_velocity = source.ground_velocity,
                vertical_speed = source.vertical_speed,
                airspeed_true = source.airspeed_true,
                heading_indicator = source.heading_indicator,
                plane_airspeed_indicated = source.plane_airspeed_indicated,
                plane_heading_magnetic = source.plane_heading_magnetic,
                altitudeaboveground = source.altitudeaboveground,
                Eng1Rpm = source.Eng1Rpm,
                Eng2Rpm = source.Eng2Rpm,
                Eng3Rpm = source.Eng3Rpm,
                Eng4Rpm = source.Eng4Rpm,
                LightsMask = source.LightsMask,
                flaps_handle_position = source.flaps_handle_position,
                spoilers_handle_position = source.spoilers_handle_position,
                gear_handle_position = source.gear_handle_position,
                ambient_wind_velocity = source.ambient_wind_velocity,
                ambient_wind_direction = source.ambient_wind_direction,
                ambient_temperature = source.ambient_temperature,
                stall_warning = source.stall_warning,
                overspeed_warning = source.overspeed_warning,
                is_gear_retractable = source.is_gear_retractable,
                spoiler_available = source.spoiler_available,
                sim_on_ground = source.sim_on_ground
            };
        }

    }

    public class FlightDataGenerator : AbstractFlightDataGenerator
    {

        public override string Name => name;

        public FlightDataGenerator(
            string name,
            FlightPathData initialFlightPathData,
            Action<FlightPathData> flightAdvancer
        )
        {
            this.name = name;
            currentSample = FlightPathDataUtils.CopyOf(initialFlightPathData);
            flightPathDataStream = FlightPathDataStream();
            this.flightAdvancer = flightAdvancer;
        }

        internal override List<FlightPathData> GetFlightPathSince(long sinceTimestamp)
        {
            long timeNowTicks = DateTime.Now.Ticks;
            return (
                flightPathDataStream.TakeWhile(
                    fp => (
                        fp.timestamp > sinceTimestamp 
                     && fp.timestamp <= Math.Max(timeNowTicks, sinceTimestamp)
                    )
                ).ToList()
            );
        }

        private IEnumerable<FlightPathData> FlightPathDataStream()
        {
            //Console.WriteLine("FlightPathDataStream re-initialized! - why?");
            while (true)
            {
                yield return FlightPathDataUtils.CopyOf(currentSample);
                flightAdvancer.Invoke(currentSample);
            }
        }

        private string name;
        private FlightPathData currentSample;
        private IEnumerable<FlightPathData> flightPathDataStream;
        private Action<FlightPathData> flightAdvancer;

    }

}
