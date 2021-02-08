using Xunit;

namespace FS2020PlanePath.XUnitTests
{
    public class ScKmlAdapterTests
    {

        private MSFS2020_SimConnectIntergration.SimPlaneDataStructure simPlaneDataStructure;
        private ScKmlAdapter scKmlAdapter;

        public ScKmlAdapterTests()
        {
            simPlaneDataStructure = new MSFS2020_SimConnectIntergration.SimPlaneDataStructure();
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
            simPlaneDataStructure.plane_pitch = 1;
            scKmlAdapter.Update(simPlaneDataStructure, flightId, seq);
            AssertDoubleValues(89, scKmlAdapter.KmlCameraValues.tilt);

            simPlaneDataStructure.plane_pitch = 90;
            scKmlAdapter.Update(simPlaneDataStructure, flightId, seq);
            AssertDoubleValues(0, scKmlAdapter.KmlCameraValues.tilt);

            simPlaneDataStructure.plane_pitch = -90;
            scKmlAdapter.Update(simPlaneDataStructure, flightId, seq);
            AssertDoubleValues(180, scKmlAdapter.KmlCameraValues.tilt);
        }

        private void UpdateWithSimPlaneValues(double d)
        {
            simPlaneDataStructure.latitude = d;
            simPlaneDataStructure.longitude = d;
            simPlaneDataStructure.altitude = (int)d;
            simPlaneDataStructure.plane_heading_true = d;
            simPlaneDataStructure.plane_pitch = d;
            simPlaneDataStructure.plane_bank = d;
            scKmlAdapter.Update(simPlaneDataStructure, 0, 0);
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
