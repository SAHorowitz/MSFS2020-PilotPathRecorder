using Xunit;
using FS2020PlanePath;

namespace MSFS2020_PilotPathRecorder.XUnitTests
{

    public class FilesystemRegistryTests
    {
        private const string Id1 = "id1";
        private FilesystemRegistry<StamType> testInstance;

        public FilesystemRegistryTests() {
            testInstance = new FilesystemRegistry<StamType>($"{GetType().Name}-");
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
