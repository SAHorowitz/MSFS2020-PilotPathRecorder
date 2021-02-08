using Xunit;

namespace FS2020PlanePath.XUnitTests
{
    public class SerializationUtilsTests
    {

        [Fact]
        public void JsonSerializationTest()
        {
            ScKmlAdapter scKmlAdapter = new ScKmlAdapter(new KmlCameraParameterValues());
            string serialization = new JsonSerializer<KmlCameraParameterValues>().Serialize(
                scKmlAdapter.KmlCameraValues
            );
            Assert.NotNull(serialization);
            Assert.DoesNotContain("AssemblyName", serialization);
        }

    }

}
