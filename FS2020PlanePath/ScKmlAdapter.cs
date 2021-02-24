using System;

namespace FS2020PlanePath
{

    public class ScKmlAdapter
    {

        const double FEET_PER_METER = 3.28084;

        private KmlCameraParameterValues kmlCameraValues;

        public KmlCameraParameterValues KmlCameraValues => kmlCameraValues;

        public ScKmlAdapter(KmlCameraParameterValues kmlCameraParameterValues)
        {
            kmlCameraValues = kmlCameraParameterValues;
        }

        public void Update(
            FlightDataStructure flightDataStructure,
            int flightId,
            long seq
        )
        {

            // see: https://developers.google.com/kml/documentation/kmlreference
            // see: http://prepar3desp.com/SDK/Core%20Utilities%20Kit/SimConnect%20SDK/SimConnect.htm

            KmlCameraValues.latitude = flightDataStructure.latitude;
            KmlCameraValues.longitude = flightDataStructure.longitude;
            KmlCameraValues.altitude = flightDataStructure.altitude / FEET_PER_METER;
            KmlCameraValues.heading = flightDataStructure.plane_heading_true;
            KmlCameraValues.tilt = Math.Max(Math.Min(90 - flightDataStructure.plane_pitch, 180), 0);
            KmlCameraValues.roll = flightDataStructure.plane_bank;
            KmlCameraValues.seq = seq;
            KmlCameraValues.flightId = flightId;

            //DebugConversion(flightDataStructure);

        }

        private void DebugConversion(FlightDataStructure simPlaneDataStructure)
        {
            string input = $@"
    latitude({simPlaneDataStructure.latitude}),
    longitude({simPlaneDataStructure.longitude}),
    altitude({simPlaneDataStructure.altitude}),
    plane_bank({simPlaneDataStructure.plane_bank}),
    plane_pitch({simPlaneDataStructure.plane_pitch}),
    plane_heading_true({simPlaneDataStructure.plane_heading_true}),
";

            string output = $@"
    latitude({KmlCameraValues.latitude}),
    longitude({KmlCameraValues.longitude}),
    altitude({KmlCameraValues.altitude}),
    heading({KmlCameraValues.heading}),
    tilt({KmlCameraValues.tilt}),
    roll({KmlCameraValues.roll})
    seq({KmlCameraValues.seq})
";

            Console.WriteLine($"converted({input}) to({output})");
            
        }

    }

}
