using Xunit;
using FS2020PlanePath;

namespace FS2020PlanePath.XUnitTests
{

    public class LiveCamRegistryTests
    {
        private const string Alias1 = "path1";
        private const string Alias2 = "path2";
        private const string BaseUrl = "http://localhost:8765/";
        private const string Url1 = BaseUrl + Alias1;
        private const string Url2 = BaseUrl + Alias2;
        IRegistry<LiveCamEntity> persistenceRegistry;
        LiveCamRegistry liveCamRegistry;

        public LiveCamRegistryTests()
        {
            persistenceRegistry = new InMemoryRegistry<LiveCamEntity>();
            liveCamRegistry = new LiveCamRegistry(persistenceRegistry);
        }

        [Fact]
        public void TestSingleRegistrationAndRetrieval()
        {
            Assert.False(liveCamRegistry.TryGetById(Alias1, out _));
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByUrl(Url1);
            Assert.NotNull(kmlLiveCam1);
            KmlLiveCam kmlLiveCam1b;
            Assert.True(liveCamRegistry.TryGetById(Alias1, out kmlLiveCam1b));
            Assert.Equal(kmlLiveCam1, kmlLiveCam1b);
        }

        [Fact]
        public void TestDoubleRegistrationAndRetrieval()
        {
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByUrl(Url1);
            Assert.False(liveCamRegistry.TryGetById(Alias2, out _));
            KmlLiveCam kmlLiveCam2 = liveCamRegistry.LoadByUrl(Url2);
            Assert.NotNull(kmlLiveCam2);
            Assert.NotEqual(kmlLiveCam1, kmlLiveCam2);
            KmlLiveCam kmlLiveCam1b, kmlLiveCam2b;
            Assert.True(liveCamRegistry.TryGetById(Alias1, out kmlLiveCam1b));
            Assert.True(liveCamRegistry.TryGetById(Alias2, out kmlLiveCam2b));
            Assert.Equal(kmlLiveCam1, kmlLiveCam1b);
            Assert.Equal(kmlLiveCam2, kmlLiveCam2b);
        }

        [Fact]
        public void TestReload()
        {
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByUrl(Url1);
            KmlLiveCam kmlLiveCam2 = liveCamRegistry.LoadByUrl(Url2);
            KmlLiveCam kmlLiveCam2b = liveCamRegistry.LoadByUrl(Url2);
            KmlLiveCam kmlLiveCam1b = liveCamRegistry.LoadByUrl(Url1);
            Assert.Equal(kmlLiveCam1, kmlLiveCam1b);
            Assert.Equal(kmlLiveCam2, kmlLiveCam2b);
        }

        [Fact]
        public void TestPersistenceRetrieval()
        {
            KmlLiveCam kmlLiveCam = liveCamRegistry.LoadByUrl(Url1);
            LiveCamEntity liveCamEntity;
            Assert.True(persistenceRegistry.TryGetById(Alias1, out liveCamEntity));
            Assert.Equal(kmlLiveCam.Camera.Template, liveCamEntity.CameraTemplate);
            Assert.Equal(kmlLiveCam.Link.Template, liveCamEntity.LinkTemplate);
            Assert.Equal(Url1, liveCamEntity.Url);
        }

        [Fact]
        public void TestPersistenceUpdate()
        {
            KmlLiveCam kmlLiveCam = liveCamRegistry.LoadByUrl(Url1);
            LiveCamEntity liveCamEntity;
            Assert.True(persistenceRegistry.TryGetById(Alias1, out liveCamEntity));
            Assert.Equal(kmlLiveCam.Camera.Template, liveCamEntity.CameraTemplate);
            Assert.Equal(kmlLiveCam.Link.Template, liveCamEntity.LinkTemplate);
            Assert.Equal(Url1, liveCamEntity.Url);
            kmlLiveCam.Camera.Template = "updated Camera Template";
            kmlLiveCam.Link.Template = "updated Link Template";
            liveCamRegistry.Save(Alias1, kmlLiveCam);
            Assert.True(persistenceRegistry.TryGetById(Alias1, out liveCamEntity));
            Assert.Equal(kmlLiveCam.Camera.Template, liveCamEntity.CameraTemplate);
            Assert.Equal(kmlLiveCam.Link.Template, liveCamEntity.LinkTemplate);
        }

        [Fact]
        public void TestUrlEncodedNetworkLinks()
        {
            const string urlRequringUrlEncoding = BaseUrl + "/one^two  three";
            string urlEncodedAlias = LiveCamRegistry.GetAlias(urlRequringUrlEncoding);
            Assert.Equal("/one%5Etwo%20%20three", urlEncodedAlias);
            Assert.True(liveCamRegistry.Save(urlEncodedAlias, LiveCamRegistry.DefaultLiveCam(urlEncodedAlias, urlRequringUrlEncoding)));
            Assert.True(persistenceRegistry.TryGetById(urlEncodedAlias, out _));
        }

    }

}
