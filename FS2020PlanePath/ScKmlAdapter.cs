using System;

namespace FS2020PlanePath
{

    class ScKmlAdapter
    {

        const double FEET_PER_METER = 3.28084;

        // see: https://developers.google.com/kml/documentation/kmlreference

        private double latitude;
        private double longitude;
        private double altitude;    // meters
        private double heading;
        private double tilt;
        private double roll;

        public void Update(MSFS2020_SimConnectIntergration.SimPlaneDataStructure simPlaneDataStructure)
        {

            // see: http://prepar3desp.com/SDK/Core%20Utilities%20Kit/SimConnect%20SDK/SimConnect.htm

            latitude = simPlaneDataStructure.latitude;
            longitude = simPlaneDataStructure.longitude;
            altitude = simPlaneDataStructure.altitude / FEET_PER_METER;
            heading = simPlaneDataStructure.plane_heading_true;
            tilt = Math.Max(Math.Min(90 - simPlaneDataStructure.plane_pitch, 180), 0);
            roll = simPlaneDataStructure.plane_bank;

            // DebugConversion(simPlaneDataStructure);

        }

        public string CameraKmlTemplate { get; set; } = @"<?xml version='1.0' encoding='UTF-8'?>
<kml xmlns = 'http://www.opengis.net/kml/2.2' xmlns:gx='http://www.google.com/kml/ext/2.2' xmlns:kml='http://www.opengis.net/kml/2.2' xmlns:atom='http://www.w3.org/2005/Atom'>

  <NetworkLinkControl>
    <Camera>
      <longitude>{longitude}</longitude>
      <latitude>{latitude}</latitude>
      <altitude>{altitude}</altitude>
      <heading>{heading}</heading>
      <tilt>{tilt}</tilt>
      <roll>{roll}</roll>
      <altitudeMode>absolute</altitudeMode>
    </Camera>
  </NetworkLinkControl>

</kml>";


        public string GetCameraKml()
        {
            string cameraKml = CameraKmlTemplate;
            string[][] substitutions =
            {
                new string[] { "{longitude}", longitude.ToString() },
                new string[] { "{latitude}", latitude.ToString() },
                new string[] { "{altitude}", altitude.ToString() },
                new string[] { "{heading}", heading.ToString() },
                new string[] { "{tilt}", tilt.ToString() },
                new string[] { "{roll}", roll.ToString() }
            };
            foreach (string[] sub in substitutions)
            {
                while(true) 
                {
                    string newCameraKml = cameraKml.Replace(sub[0], sub[1]);
                    if (newCameraKml == cameraKml)
                    {
                        break;
                    }
                    cameraKml = newCameraKml;
                }
            }
            return cameraKml;
        }

        private void DebugConversion(MSFS2020_SimConnectIntergration.SimPlaneDataStructure simPlaneDataStructure)
        {
            string input = $@"
    latitude({simPlaneDataStructure.latitude}),
    longitude({simPlaneDataStructure.longitude}),
    altitude({simPlaneDataStructure.altitude}),
    plane_bank({simPlaneDataStructure.plane_bank}),
    plane_pitch({simPlaneDataStructure.plane_pitch}),
    plane_heading_true({simPlaneDataStructure.plane_heading_true}),
    plane_heading_magnetic({simPlaneDataStructure.plane_heading_magnetic}),
    heading_indicator({simPlaneDataStructure.heading_indicator}),
    altitude_above_ground({simPlaneDataStructure.altitude_above_ground})
";

            string output = $@"
    latitude({latitude}),
    longitude({longitude}),
    altitude({altitude}),
    heading({heading}),
    tilt({tilt}),
    roll({roll})
";

            Console.WriteLine($"converted({input}) to({output})");
            
        }

    }


}
