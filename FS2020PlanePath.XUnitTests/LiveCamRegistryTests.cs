using Xunit;

namespace FS2020PlanePath.XUnitTests
{

    public class LiveCamRegistryTests
    {

        private const string Alias1 = "alias1";
        private const string Alias2 = "alias2";
        private const string UPDATED_CAMERA_TEMPLATE_VALUE = "updated Camera Template";
        private const string UPDATED_LINK_TEMPLATE_VALUE = "updated Link Template";
        private const string CAMERA_LENS_NAME = "camera";
        private const string LINK_LENS_NAME = "link";

        IRegistry<LiveCamEntity> persistenceRegistry;
        LiveCamRegistry liveCamRegistry;

        public LiveCamRegistryTests()
        {
            persistenceRegistry = new InMemoryRegistry<LiveCamEntity>();
            InMemoryRegistry<LiveCamEntity> builtinLiveCamRegistry = new InMemoryRegistry<LiveCamEntity>();
            liveCamRegistry = new LiveCamRegistry(
                persistenceRegistry,
                builtinLiveCamRegistry
            );
        }

        [Fact]
        public void TestSingleRegistrationAndRetrieval()
        {
            Assert.False(liveCamRegistry.TryGetById(Alias1, out _));
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByAlias(Alias1);
            Assert.NotNull(kmlLiveCam1);
            KmlLiveCam kmlLiveCam1b;
            Assert.True(liveCamRegistry.TryGetById(Alias1, out kmlLiveCam1b));
            Assert.Equal(kmlLiveCam1, kmlLiveCam1b);
        }

        [Fact]
        public void TestDoubleRegistrationAndRetrieval()
        {
            // TODO - improve this rather devolved test
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByAlias(Alias1);
            Assert.NotNull(kmlLiveCam1);
            Assert.NotNull(kmlLiveCam1.LensNames);
            KmlLiveCam kmlLiveCam2;
            Assert.False(liveCamRegistry.TryGetById(Alias2, out kmlLiveCam2));
            Assert.Null(kmlLiveCam2);
            KmlLiveCam kmlLiveCam2b = liveCamRegistry.LoadByAlias(Alias2);
            KmlLiveCam kmlLiveCam1b, kmlLiveCam2c;
            Assert.True(liveCamRegistry.TryGetById(Alias1, out kmlLiveCam1b));
            Assert.True(liveCamRegistry.TryGetById(Alias2, out kmlLiveCam2c));
            Assert.Equal(kmlLiveCam1, kmlLiveCam1b);
            Assert.Equal(kmlLiveCam2b, kmlLiveCam2c);
        }

        [Fact]
        public void TestCacheReload()
        {
            // tests that the identical object is retrieved from the cache (e.g., default not re-created)
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByAlias(Alias1);
            KmlLiveCam kmlLiveCam2 = liveCamRegistry.LoadByAlias(Alias2);
            KmlLiveCam kmlLiveCam2b = liveCamRegistry.LoadByAlias(Alias2);
            KmlLiveCam kmlLiveCam1b = liveCamRegistry.LoadByAlias(Alias1);
            Assert.True(kmlLiveCam1 == kmlLiveCam1b);
            Assert.True(kmlLiveCam2 == kmlLiveCam2b);
        }

        [Fact]
        public void TestPersistenceUpdate()
        {
            // load default live cam for "Alias1" into "kmlLiveCam1"
            KmlLiveCam kmlLiveCam1 = liveCamRegistry.LoadByAlias(Alias1);

            // it should have an empty lens names structure
            Assert.NotNull(kmlLiveCam1.LensNames);
            Assert.False(kmlLiveCam1.LensNames.GetEnumerator().MoveNext());

            LiveCamEntity liveCamEntity;
            // ensure can load it from persistence registry
            Assert.True(persistenceRegistry.TryGetById(Alias1, out liveCamEntity));

            // and verify it has no templates
            Assert.Empty(liveCamEntity.Lens);

            // create a new "kmlLiveCam2" with some templates in it
            KmlLiveCam kmlLiveCam2 = NewStandardCameraLinkLiveCam();

            // and save that into the registry, replacing as "Alias1"
            liveCamRegistry.Save(Alias1, kmlLiveCam2);

            // ensure can load it from the persistence registry
            Assert.True(persistenceRegistry.TryGetById(Alias1, out liveCamEntity));

            // and that is and the persistence repository now have the new, updated values
            Assert.Equal(2, liveCamEntity.Lens.Length);

            string liveCamLensCameraTemplate = kmlLiveCam2.GetLens(CAMERA_LENS_NAME).Template;
            Assert.Equal(UPDATED_CAMERA_TEMPLATE_VALUE, liveCamLensCameraTemplate);
            Assert.Equal(liveCamLensCameraTemplate, liveCamEntity.Lens[0].Template);

            string liveCamLensLinkTemplate = kmlLiveCam2.GetLens(LINK_LENS_NAME).Template;
            Assert.Equal(UPDATED_LINK_TEMPLATE_VALUE, liveCamLensLinkTemplate);
            Assert.Equal(liveCamLensLinkTemplate, liveCamEntity.Lens[1].Template);
        }

        [Fact]
        public void TestSaveOfAliasRequiringFilesystemNameEncoding()
        {
            const string alias = "/one/two^three.four  five";
            Assert.True(liveCamRegistry.Save(alias, liveCamRegistry.DefaultLiveCam(alias)));
            Assert.True(persistenceRegistry.TryGetById(alias, out _));
        }

        [Fact]
        public void TestTryGetForNonLoadedAlias()
        {
            persistenceRegistry.Save("id", new LiveCamEntity());
            // registry should be able to get an item that's in
            // persistence but not yet in its cache
            Assert.True(liveCamRegistry.TryGetById("id", out _));
        }

        private static KmlLiveCam NewStandardCameraLinkLiveCam()
        {
            return new KmlLiveCam(
                new LiveCamEntity(
                    new LiveCamLensEntity(CAMERA_LENS_NAME, UPDATED_CAMERA_TEMPLATE_VALUE),
                    new LiveCamLensEntity(LINK_LENS_NAME, UPDATED_LINK_TEMPLATE_VALUE)
                )
            );
        }

    }

}
