using Xunit;

namespace FS2020PlanePath.XUnitTests
{
    public class ScKmlAdapterTests
    {

        private FlightDataStructure flightDataStructure;
        private ScKmlAdapter scKmlAdapter;

        public ScKmlAdapterTests()
        {
            flightDataStructure = new FlightDataStructure();
            scKmlAdapter = new ScKmlAdapter(new KmlCameraParameterValues());
        }

        [Fact]
        public void TestInitialState()
        {
            AssertDoubleValues(
                0,
                scKmlAdapter.KmlCameraValues.latitude,
                scKmlAdapter.KmlCameraValues.longitude,
                scKmlAdapter.KmlCameraValues.altitude,
                scKmlAdapter.KmlCameraValues.heading,
                scKmlAdapter.KmlCameraValues.tilt,
                scKmlAdapter.KmlCameraValues.roll
            );
        }

        [Fact]
        public void TestCopyThroughValues()
        {
            UpdateWithSimPlaneValues(1);
            AssertDoubleValues(
                1,
                scKmlAdapter.KmlCameraValues.latitude,
                scKmlAdapter.KmlCameraValues.longitude,
                scKmlAdapter.KmlCameraValues.heading,
                scKmlAdapter.KmlCameraValues.roll
            );
        }

        [Fact]
        public void TestAltitudeConversion()
        {
            UpdateWithSimPlaneValues(1);
            AssertDoubleValues(1 / 3.28084, scKmlAdapter.KmlCameraValues.altitude);
        }

        [Fact]
        public void TestTiltConversion()
        {
            int flightId = 0;
            long seq = 0;
            flightDataStructure.plane_pitch = 1;
            scKmlAdapter.Update(flightDataStructure, flightId, seq);
            AssertDoubleValues(89, scKmlAdapter.KmlCameraValues.tilt);

            flightDataStructure.plane_pitch = 90;
            scKmlAdapter.Update(flightDataStructure, flightId, seq);
            AssertDoubleValues(0, scKmlAdapter.KmlCameraValues.tilt);

            flightDataStructure.plane_pitch = -90;
            scKmlAdapter.Update(flightDataStructure, flightId, seq);
            AssertDoubleValues(180, scKmlAdapter.KmlCameraValues.tilt);
        }

        private void UpdateWithSimPlaneValues(double d)
        {
            flightDataStructure.latitude = d;
            flightDataStructure.longitude = d;
            flightDataStructure.altitude = (int)d;
            flightDataStructure.plane_heading_true = d;
            flightDataStructure.plane_pitch = d;
            flightDataStructure.plane_bank = d;
            scKmlAdapter.Update(flightDataStructure, 0, 0);
        }

        private void AssertDoubleValues(double expected,  params double[] actuals)
        {
            foreach (var actual in actuals)
            {
                Assert.Equal(expected, actual);
            }
        }

    }

}
