namespace FS2020PlanePath
{
    public static class LiveCamRegistryFactory
    {

        static IRegistry<KmlLiveCam> CreateRegistry()
        {
            return new LiveCamRegistry(
                new FilesystemRegistry<LiveCamEntity>("FS2020ppLiveCamEntity")
            );
        }
    }

}
