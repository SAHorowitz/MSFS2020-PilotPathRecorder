using System.Linq;
using System.Collections.Generic;

namespace FS2020PlanePath
{

    public class LiveCamRegistry : IRegistry<KmlLiveCam>
    {

        private IRegistry<KmlLiveCam> cacheRegistry;
        private IRegistry<LiveCamEntity> persistentRegistry;
        private IRegistry<LiveCamEntity> builtinLiveCamRegistry;

        public LiveCamRegistry(
            IRegistry<LiveCamEntity> persistentRegistry,
            IRegistry<LiveCamEntity> builtinLiveCamRegistry
        )
        {
            cacheRegistry = new InMemoryRegistry<KmlLiveCam>();
            this.persistentRegistry = persistentRegistry;
            this.builtinLiveCamRegistry = builtinLiveCamRegistry;
        }

        public KmlLiveCam LoadByAlias(string alias)
        {
            KmlLiveCam kmlLiveCam;
            if (!TryGetById(alias, out kmlLiveCam))
            {
                kmlLiveCam = new KmlLiveCam(new LiveCamEntity());
                Save(alias, kmlLiveCam);
            }
            return kmlLiveCam;
        }

        public bool TryGetById(string alias, out KmlLiveCam kmlLiveCam)
        {
            if (cacheRegistry.TryGetById(alias, out kmlLiveCam))
            {
                return true;
            }

            LiveCamEntity liveCamEntity;

            if (persistentRegistry.TryGetById(alias, out liveCamEntity))
            {
                kmlLiveCam = new KmlLiveCam(liveCamEntity);
                cacheRegistry.Save(alias, kmlLiveCam);
                return true;
            }

            if (builtinLiveCamRegistry.TryGetById(alias, out liveCamEntity))
            {
                kmlLiveCam = new KmlLiveCam(liveCamEntity);
                cacheRegistry.Save(alias, kmlLiveCam);
                return true;
            }

            return false;
        }

        public bool Save(string alias, KmlLiveCam kmlLiveCam)
        {
            cacheRegistry.Save(alias, kmlLiveCam);
            return persistentRegistry.Save(alias, new LiveCamEntity(kmlLiveCam));
        }

        public bool Delete(string alias)
        {
            return cacheRegistry.Delete(alias) && persistentRegistry.Delete(alias);
        }

        public List<string> GetIds(int maxCount = -1)
        {
            List<string> ids = new List<string>();
            ids.AddRange(persistentRegistry.GetIds(maxCount));
            foreach (string id in builtinLiveCamRegistry.GetIds(maxCount))
            {
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
            }
            return ids;
        }

        public bool IsDefaultDefinition(KmlLiveCam kmlLiveCam, string alias)
        {
            return DefaultLiveCam(alias).Equals(kmlLiveCam);
        }

        public KmlLiveCam DefaultLiveCam(string alias)
        {
            return new KmlLiveCam(DefaultLiveCamEntity(alias));
        }

        private LiveCamEntity DefaultLiveCamEntity(string alias)
        {
            LiveCamEntity builtinLiveCamEntity;
            if (builtinLiveCamRegistry.TryGetById(alias, out builtinLiveCamEntity))
            {
                return builtinLiveCamEntity;
            }
            return new LiveCamEntity();
        }

    }

    public class LiveCamLensEntity
    {

        public LiveCamLensEntity(string name, string template)
        {
            Name = name;
            Template = template;
        }

        public string Name {
            get => name;
            set {
                name = value != null ? value : "";
            }
         }

        public string Template {
            get => template;
            set {
                template = value != null ? value : "";
            }
         }

        private string name, template;

    }

    public class LiveCamEntity
    {

        public LiveCamEntity(KmlLiveCam kmlLiveCam)
        : this(
            kmlLiveCam.LensNames.Select(
                lensName => new LiveCamLensEntity(
                    lensName,
                    kmlLiveCam.GetLens(lensName).Template
                )
            )
            .ToArray()
        ) {
            // nothing additional
        }

        public LiveCamEntity()
        {
            Lens = default(LiveCamLensEntity[]) ;
        }

        public LiveCamEntity(params LiveCamLensEntity[] lens)
        {
            Lens = lens;
        }

        public LiveCamLensEntity[] Lens {
            get => lens;
            set
            {
                lens = value ?? new LiveCamLensEntity[0];
            } 
        }

        private LiveCamLensEntity[] lens;

    }

}
