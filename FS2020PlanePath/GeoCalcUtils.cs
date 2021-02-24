using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS2020PlanePath
{
    public class GeoCalcUtils
    {

        public const double PI = 3.14159265359;
        public const double Rnm = 3_440.1;   // ~ earth's radius in nautical miles

        /// <summary>
        /// Calculates coordinates resulting from travel from initial coordinate along bearing for distance.
        /// </summary>
        /// <param name="latitude">initial latitude</param>
        /// <param name="longitude">initial longitude</param>
        /// <param name="bearing">direction of travel from the initial position</param>
        /// <param name="distance">distance (nautical miles)</param>
        // <returns>new (latitude, longitude) coordinates after travel</returns>
        public static (double, double) calcLatLonOffset(double latitude, double longitude, double bearing, double distance)
        {

            double lat1Rad = degreesToRadians(latitude);
            double lon1Rad = degreesToRadians(longitude);
            double brngRad = degreesToRadians(bearing);

            double distRad = distance / Rnm;
            double lat2Rad = Math.Asin(Math.Sin(lat1Rad) * Math.Cos(distRad) + Math.Cos(lat1Rad) * Math.Sin(distRad) * Math.Cos(brngRad));
            double lon2Rad = lon1Rad + Math.Atan2(Math.Sin(brngRad) * Math.Sin(distRad) * Math.Cos(lat1Rad), Math.Cos(distRad) - Math.Sin(lat1Rad) * Math.Sin(lat2Rad));

            return (radiansToDegrees(lat2Rad), radiansToDegrees(lon2Rad));
        }

        /// <param name="iso6709Coordinate">"ISO 6709-like" latitude or longitude coordinate, ranging from -180 <= x < +180</param>
        /// <returns>normalized coordinate, ranging from 0 <= x < 360</returns>
        public static double normalizedIso6709GeoDirection(double iso6709Coordinate)
        {
            return rationalizedAngleDegrees(iso6709Coordinate, 180);
        }

        /// <param name="compassCoordinate">"Compass-like" direction resulting from transformations</param>
        /// <returns>rationalized Compass coordinate, ranging from 0 <= x < 360</returns>
        public static double rationalizedCompassDirection(double compassCoordinate)
        {
            return rationalizedAngleDegrees(compassCoordinate, 360);
        }

        ///  <param name="coord">a geographic (latitude, longitude) coordinate</param>
        ///  <returns>human-readable (e.g. "printable") representation of the geographic coordinate</returns>
        public static string pcoord((double lat, double lon) coord)
        {
            return $"{coord.lat},{coord.lon}";
        }

        ///  <param name="degrees">an angle, in degrees</param>
        ///  <returns>the angle, in radians</returns>
        private static double degreesToRadians(double degrees)
        {
            return degrees / 180 * PI; 
        }

        ///  <param name="radians">an angle, in radians</param>
        ///  <returns>the angle, in degrees</returns>
        private static double radiansToDegrees(double radians)
        {
            return radians / PI * 180;
        }

        /// <param name="irrationalAngle">Angle in degree units, possibly resulting from transformations</param>
        //  <param name="maxRationalAngle">Maximum rational angle (e.g., 180 or 360) value</param>
        /// <returns>rationalized angle degrees, ranging from 0 <= x < 360</returns>
        private static double rationalizedAngleDegrees(double irrationalAngle, double maxRationalAngle)
        {
            double normalizedAngle = irrationalAngle;
            while (normalizedAngle < -Math.Abs(maxRationalAngle))
            {
                normalizedAngle += 360;
            }
            while (normalizedAngle > Math.Abs(maxRationalAngle))
            {
                normalizedAngle -= 360;
            }
            return normalizedAngle;
        }

    }

}
