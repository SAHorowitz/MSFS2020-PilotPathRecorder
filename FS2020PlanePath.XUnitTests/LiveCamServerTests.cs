using Xunit;
using Xunit.Abstractions;

namespace FS2020PlanePath.XUnitTests
{

    // TODO - write some tests!
    public class LiveCamServerTests
    {

        private readonly ITestOutputHelper logger;

        public LiveCamServerTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Fact]
        public void LiveCamUrlParserTest()
        {
            /**
             * Camera URL:  http://localhost:8000/kmlcam/cockpit/link
             *   FullPath: ==> /kmlcam/cockpit/link
             *   SpecPath: ==> /cockpit/link
             *      Alias: ==> cockpit
             *       Lens: ==> link
             *       Spec: ==> (cockpit, link)
             */

        }

    }
}
