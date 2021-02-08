using System;
using System.IO;
using System.Collections.Generic;

namespace FS2020PlanePath
{
    public class LiveCamRegistryFactory
    {

        public LiveCamRegistry NewRegistry()
        {
            string liveCamRegistryFilenamePrefix = $"{typeof(LiveCamEntity).Name}_";
            return new LiveCamRegistry(
                new JsonFilesystemRegistry<LiveCamEntity>("", liveCamRegistryFilenamePrefix),
                new JsonFilesystemRegistry<LiveCamEntity>("Resources/liveCam/defaults/", liveCamRegistryFilenamePrefix)
            );
        }

    }

}
