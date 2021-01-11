//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Data.Entity.Core.Objects;
//using System.Linq;
//using System.Configuration;
//using System.Data;
//using System.Data.Common;
//using System.Data.Entity;
//using System.Data.Entity.Core.Common;
//using System.Data.SQLite;
//using System.Data.SQLite.EF6;
//using System.Linq;

//namespace FS2020PlanePath
//{

//    // TODO: migrate app to EF Core (for which this was written), or re-write for EF6; see embedded comments

//    public class DatabaseLiveCamRegistry : ILiveCamRegistry
//    {

//        private LiveCamDbContext liveCamDbContext;
//        private InMemoryLiveCamRegistry inMemoryLiveCamRegistry;

//        public DatabaseLiveCamRegistry()
//        {
//            liveCamDbContext = new LiveCamDbContext();
////            liveCamDbContext.Database.EnsureCreated();  // EF Core
//            liveCamDbContext.Database.CreateIfNotExists();  // EF6
//            inMemoryLiveCamRegistry = new InMemoryLiveCamRegistry();
//        }

//        public KmlLiveCam LoadByUrl(string url)
//        {
//            string alias = inMemoryLiveCamRegistry.getAlias(url);
//            KmlLiveCam cachedKmlLiveCam;

//            if (inMemoryLiveCamRegistry.TryGetByAlias(alias, out cachedKmlLiveCam))
//            {
//                // found in the cache, so return it
//                return cachedKmlLiveCam;
//            }

//            // try to load from the database
//            DbSet<LiveCamEntity> liveCams = liveCamDbContext.LiveCams;

//            List<LiveCamEntity> lists = liveCams.ToList();

//            LiveCamEntity liveCamEntityForUrl = (
//                lists
//                .Where(lce => lce.Url == url)
//                //.GroupBy(row => row.Url)
//                .OrderByDescending(lce => lce.Id)
//                .FirstOrDefault()
//            );

//            if (liveCamEntityForUrl != default(LiveCamEntity))
//            {
//                // if found in the database, load it into a newly created LiveCam
//                KmlLiveCam dbKmlLiveCam = new KmlLiveCam();
//                dbKmlLiveCam.Camera.Template = liveCamEntityForUrl.CameraTemplate;
//                dbKmlLiveCam.Link.Template = liveCamEntityForUrl.LinkTemplate;
//                dbKmlLiveCam.Link.Values = new KmlNetworkLinkValues(alias, url);
//                // and save that into the cache
//                inMemoryLiveCamRegistry.Save(dbKmlLiveCam);
//                return dbKmlLiveCam;
//            }

//            // if not found in the database, get the default one from the memory registry,
//            KmlLiveCam kmlLiveCam = inMemoryLiveCamRegistry.createNewLiveCam(alias, url);
//            // and save it in the database
//            Save(kmlLiveCam);
//            return kmlLiveCam;
//        }

//        public void Save(KmlLiveCam kmlLiveCam)
//        {
//            LiveCamEntity liveCamEntity = new LiveCamEntity
//            {
//                Url = kmlLiveCam.Link.Values.url,
//                CameraTemplate = kmlLiveCam.Camera.Template,
//                LinkTemplate = kmlLiveCam.Link.Template
//            };
//            inMemoryLiveCamRegistry.Save(kmlLiveCam);
//            liveCamDbContext.LiveCams.Add(liveCamEntity);
//            liveCamDbContext.SaveChanges();
//        }

//        public bool TryGetByAlias(string alias, out KmlLiveCam kmlLiveCam)
//        {
//            return inMemoryLiveCamRegistry.TryGetByAlias(alias, out kmlLiveCam);
//        }

//    }

//    public class LiveCamEntity
//    {
//        public int Id { get; set; }
//        public string Url { get; set; }
//        public string CameraTemplate { get; set; }
//        public string LinkTemplate { get; set; }
//    }

//    public class LiveCamDbContext : DbContext
//    {
//        public DbSet<LiveCamEntity> LiveCams { get; set; }

//        // commented out: works with EF Core, no equivalent found for EF6
//    //    protected override void OnConfiguring(DbContextOptionsBuilder options)
//    //        => options.UseSqlite("Data Source=livecam.db");
//    }

//}
