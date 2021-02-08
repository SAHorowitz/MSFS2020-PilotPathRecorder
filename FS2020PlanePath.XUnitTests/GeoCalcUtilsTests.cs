using System;
using Xunit;
using Xunit.Abstractions;

namespace FS2020PlanePath.XUnitTests
{
    public class GeoCalcUtilsTests
    {

        private readonly ITestOutputHelper logger;

        public GeoCalcUtilsTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Fact]
        public void CalcLatLonElCerritoToOakland()
        {
            (double lat, double lon) startCoord = (37.9017235, -122.2623529);   // El Cerrito BART Station
            (double lat, double lon) targetCoord = (37.856853, -122.2370224);   // Oakland BART Station
            (double lat, double lon) endCoord;

            endCoord = GeoCalcUtils.calcLatLonOffset(
                startCoord.lat,
                startCoord.lon,
                175,
                7
            );

            logger.WriteLine($"  startCoord({GeoCalcUtils.pcoord(startCoord)})");
            logger.WriteLine($"    endCoord({GeoCalcUtils.pcoord(endCoord)})");
            logger.WriteLine($" targetCoord({GeoCalcUtils.pcoord(targetCoord)})");
        }

        [Fact]
        public void CalcLatLonNorthToSouthPole()
        {
            (double lat, double lon) startCoord = (90, 0);       // North Pole
            (double lat, double lon) targetCoord = (-90, 0);    // South Pole
            (double lat, double lon) endCoord;

            endCoord = GeoCalcUtils.calcLatLonOffset(
                startCoord.lat,
                startCoord.lon,
                180,
                GeoCalcUtils.PI * GeoCalcUtils.Rnm
            );

            logger.WriteLine($"  startCoord({GeoCalcUtils.pcoord(startCoord)})");
            logger.WriteLine($"    endCoord({GeoCalcUtils.pcoord(endCoord)})");
            logger.WriteLine($" targetCoord({GeoCalcUtils.pcoord(targetCoord)})");
        }

        [Fact]
        public void CalcLatLonSundryTests()
        {
            assertCorrectLatLon(
                "North Pole to 87.5N",
                (87.475747, -99.985739),
                (5, 180),       // TODO: error is too large - FIX!!
                (90, 0),
                (321.28, 150.7)
            );
            assertCorrectLatLon(
                "Berkeley To Oakland",
                (37.804249, -122.271171),
                (0.001, 0.004), // TODO: error is large - improve
                (37.871613, -122.272738),
                (178.96, 4.05)
            );
            assertCorrectLatLon(
                "Todo Santos to Agua Cliente",
                (23.441660, -109.773294),
                (0.005, 2), // TODO: error is too large - improve
                (23.446416, -110.226501),
                (90.56, 25)
            );
        }

        private static void assertCorrectLatLon(
            string message,
            (double lat, double lon) expectedDestination,
            (double lat, double lon) tolerance,
            (double lat, double lon) startingCoordinate,
            (double bearing, double nm) travel
        )
        {
            (double lat, double lon) actualDestination = GeoCalcUtils.calcLatLonOffset(
                startingCoordinate.lat,
                startingCoordinate.lon,
                travel.bearing,
                travel.nm
            );

            (double lat, double lon) delta = (
                Math.Abs(expectedDestination.lat - actualDestination.lat),
                Math.Abs(expectedDestination.lon - actualDestination.lon)
            );

            if (delta.lat > tolerance.lat || delta.lon > tolerance.lon)
            {
                Assert.True(
                    false, 
                    $"{message}: exp({GeoCalcUtils.pcoord(expectedDestination)}), act({GeoCalcUtils.pcoord(actualDestination)}), tol({GeoCalcUtils.pcoord(tolerance)}), diff({GeoCalcUtils.pcoord(delta)})"
                );
            }

        }

    }

}
