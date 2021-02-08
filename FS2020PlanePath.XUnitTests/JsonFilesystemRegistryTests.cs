using Xunit;

namespace FS2020PlanePath.XUnitTests
{

    public class JsonFilesystemRegistryTests
    {
        private const string Id1 = "id1";
        private JsonFilesystemRegistry<StamType> testInstance;

        public JsonFilesystemRegistryTests() {
            testInstance = new JsonFilesystemRegistry<StamType>("", "fnpfx_");
        }

        [Fact]
        public void testFilenameGeneration()
        {
            string fileName = testInstance.FilenameForId("one/two.three  four");
            Assert.Equal("fnpfx_one%2Ftwo.three%20%20four.json", fileName);
        }

        [Fact]
        public void TestSave()
        {
            testInstance.Delete(Id1);

            StamType readValue;

            Assert.False(testInstance.TryGetById(Id1, out readValue));
            Assert.True(readValue.Equals(default(StamType)));

            StamType writtenValue = new StamType { intItem = 1, stringItem = "s1" };
            testInstance.Save(Id1, writtenValue);
            Assert.True(testInstance.TryGetById(Id1, out readValue));
            Assert.Equal(writtenValue, writtenValue);

            testInstance.Delete(Id1);
            Assert.False(testInstance.TryGetById(Id1, out readValue));

        }

    }

    public struct StamType
    {
        public int intItem;
        public string stringItem;
        public override string ToString() => $"{intItem},{stringItem}";
    }

}
