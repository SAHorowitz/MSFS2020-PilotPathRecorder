using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace FS2020PlanePath.XUnitTests
{

    public class KmlLiveCamTests
    {
        private readonly ITestOutputHelper logger;
        private MultiTrackKmlGenerator multiTrackKmlGenerator = new MultiTrackKmlGenerator();

        public KmlLiveCamTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Fact]
        public void TestCamerTemplateTextRendering()
        {
            var kmlLiveCam = new KmlLiveCam( 
                new LiveCamEntity(
                    new LiveCamLensEntity("camera", "<cam({seq})"),
                    new LiveCamLensEntity("link", "<link({alias},{lensUrl})")
                )
            );
            KmlCameraParameterValues camValues = new KmlCameraParameterValues
            {
                seq = 99
            };
            KmlCameraParameterValues linkValues = new KmlCameraParameterValues
            {
                liveCamUriPath = "lcp",
                alias = "a76"
            };
            Assert.Equal("<cam(99)", kmlLiveCam.GetLens("camera").Render(camValues));
            Assert.Equal("<link(a76,/lcp/a76/)", kmlLiveCam.GetLens("link").Render(linkValues));
        }

        [Fact]
        public void TestCamerTemplateScriptRendering()
        {
            var kmlLiveCam = new KmlLiveCam(
                new LiveCamEntity(
                    new LiveCamLensEntity("camera", "return $\"seq({seq})\";"),
                    new LiveCamLensEntity("link", "return $\"lensUrl({lensUrl})\";")
                )
            );
            KmlCameraParameterValues camValues = new KmlCameraParameterValues { seq = 678 };
            KmlCameraParameterValues linkValues = new KmlCameraParameterValues { listenerUrl = "hey!", alias = "a", lens = "l" };
            Assert.Equal("seq(678)", kmlLiveCam.GetLens("camera").Render(camValues));
            Assert.Equal("lensUrl(hey!//a/l)", kmlLiveCam.GetLens("link").Render(linkValues));
        }

        [Fact]
        public void GenerateMultiTrackKmlDocTest()
        {
            foreach (KmlCameraParameterValues kmlCameraParameterValues in getKmlCameraUpdates(0))
            {
                multiTrackKmlGenerator.AddKmlCameraParameterValues(kmlCameraParameterValues);
            }
            logger.WriteLine($"{multiTrackKmlGenerator.GetMultiTrackKml()}");
        }

        [Fact]
        public void RazorMultiTrackTest()
        {
            string razorMultiTrackCameraTemplate = @"
@{ var updates = Model.multitrack.GetUpdates(); }
<?xml version='1.0' encoding='UTF-8'?>
<kml
  xmlns='http://www.opengis.net/kml/2.2'
  xmlns:gx='http://www.google.com/kml/ext/2.2'
>
  <NetworkLinkControl>
    <cookie>seq=@Model.multitrack.seq</cookie>
    <cookie>flightId=@Model.multitrack.flightId</cookie>
    <Update>
      <Create>
        <gx:MultiTrack targetId='pp'>
          <gx:Track>
            @foreach (var update in updates) {
            <gx:coord>
              @update.longitude
              @update.latitude
              @update.altitude
            </gx:coord>
            }
            @foreach (var update in updates) {
            <gx:angles>
              @update.heading
              @update.tilt
              @update.roll
            </gx:angles>
            }
          </gx:Track>
        </gx:MultiTrack>
      </Create>
    </Update>
  </NetworkLinkControl>
</kml>";
            KmlCameraParameterValues kmlCameraParameterValues = new KmlCameraParameterValues();

            string renderedKml = (
                new TemplateRendererFactory((message, details) => "renderer error: message({message}), details({details})")
                .newTemplateRenderer<KmlCameraParameterValues>(razorMultiTrackCameraTemplate)
                .Render(kmlCameraParameterValues)
            );

            logger.WriteLine($"renderedKml({renderedKml})");
        }

        private static KmlCameraParameterValues[] getKmlCameraUpdates(int seq0)
        {
            Random random = new Random();
            int seq = seq0;
            double lat0 = 37.7749;
            double lon0 = 122.4194;
            double alt0 = 1000 + seq0;
            double hdg0 = random.Next(360);
            double pit0 = 0;
            double rol0 = 0;
            List<KmlCameraParameterValues> valueList = new List<KmlCameraParameterValues>();
            int nUdates = random.Next(10);
            for (int i = 0; i < nUdates; i++)
            {
                double latOffset = seq * random.NextDouble() * 0.001;
                double lonOffset = seq * random.NextDouble() * 0.002;
                double altOffset = seq * random.NextDouble() * 10;
                valueList.Add(
                    new KmlCameraParameterValues
                    {
                        seq = seq,
                        latitude = lat0 + latOffset,
                        longitude = lon0 + lonOffset,
                        altitude = alt0 + altOffset,
                        heading = hdg0 + random.Next(3),
                        tilt = pit0 + random.Next(5),
                        roll = rol0 + random.Next(10)
                    }
                );
                seq = seq + 1;
            }
            return valueList.ToArray();
        }

    }

}
