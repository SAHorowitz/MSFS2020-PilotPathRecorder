using Xunit;
using Xunit.Abstractions;
using FS2020PlanePath;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace MSFS2020_PilotPathRecorder.XUnitTests
{

    public class LiveCamRegistryTests
    {

        LiveCamRegistry liveCamRegistry = new LiveCamRegistry();

        [Fact]
        public void TestSingleRegistrationAndRetrieval()
        {
            Assert.False(liveCamRegistry.TryGetByAlias("/path1", out _));
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByUrl("http://localhost:8000/path1");
            Assert.NotNull(kmlLiveCam1);
            KmlLiveCam kmlLiveCam1b;
            Assert.True(liveCamRegistry.TryGetByAlias("/path1", out kmlLiveCam1b));
            Assert.Equal(kmlLiveCam1, kmlLiveCam1b);
        }

        [Fact]
        public void TestDoubleRegistrationAndRetrieval()
        {
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByUrl("http://localhost:8000/path1");
            Assert.False(liveCamRegistry.TryGetByAlias("/path2", out _));
            KmlLiveCam kmlLiveCam2 = liveCamRegistry.LoadByUrl("http://localhost:8000/path2");
            Assert.NotNull(kmlLiveCam2);
            Assert.NotEqual(kmlLiveCam1, kmlLiveCam2);
            KmlLiveCam kmlLiveCam1b, kmlLiveCam2b;
            Assert.True(liveCamRegistry.TryGetByAlias("/path1", out kmlLiveCam1b));
            Assert.True(liveCamRegistry.TryGetByAlias("/path2", out kmlLiveCam2b));
            Assert.Equal(kmlLiveCam1, kmlLiveCam1b);
            Assert.Equal(kmlLiveCam2, kmlLiveCam2b);
        }

        [Fact]
        public void TestReload()
        {
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByUrl("http://localhost:8000/path1");
            KmlLiveCam kmlLiveCam2 = liveCamRegistry.LoadByUrl("http://localhost:8000/path2");
            KmlLiveCam kmlLiveCam2b = liveCamRegistry.LoadByUrl("http://localhost:8000/path2");
            KmlLiveCam kmlLiveCam1b = liveCamRegistry.LoadByUrl("http://localhost:8000/path1");
            Assert.Equal(kmlLiveCam1, kmlLiveCam1b);
            Assert.Equal(kmlLiveCam2, kmlLiveCam2b);
        }

    }


}
