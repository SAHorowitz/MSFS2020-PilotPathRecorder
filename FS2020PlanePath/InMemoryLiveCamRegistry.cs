using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FS2020PlanePath
{
    public interface ILiveCamRegistry
    {
        KmlLiveCam LoadByUrl(string url);
        bool TryGetByAlias(string alias, out KmlLiveCam kmlLiveCam);
    }
    public static class LiveCamConstants
    {

        public const string DefaultCameraKmlTemplate = @"<?xml version='1.0' encoding='UTF-8'?>
<kml xmlns = 'http://www.opengis.net/kml/2.2' xmlns:gx='http://www.google.com/kml/ext/2.2' xmlns:kml='http://www.opengis.net/kml/2.2' xmlns:atom='http://www.w3.org/2005/Atom'>
  <NetworkLinkControl>
    <Camera>
      <longitude>{longitude}</longitude>
      <latitude>{latitude}</latitude>
      <altitude>{altitude}</altitude>
      <heading>{heading}</heading>
      <tilt>{tilt}</tilt>
      <roll>{roll}</roll>
      <altitudeMode>absolute</altitudeMode>
    </Camera>
  </NetworkLinkControl>
</kml>";

        public const string DefaultNetworkLinkKmlTemplate =
@"<?xml version='1.0' encoding='UTF-8'?>
<kml xmlns='http://www.opengis.net/kml/2.2' xmlns:gx='http://www.google.com/kml/ext/2.2' xmlns:kml='http://www.opengis.net/kml/2.2' xmlns:atom='http://www.w3.org/2005/Atom'>
<NetworkLink>
	<name>MSFS2020-PilotPathRecorder Live Camera ({alias})</name>
	<flyToView>1</flyToView>
	<Link>
		<href>{url}</href>
        <!-- onChange, onInterval, ... -->
		<refreshMode>onChange</refreshMode>
        <!-- number of seconds between refreshs -->
		<refreshInterval>1</refreshInterval>
        <!-- never, onStop, onRequest ... -->
		<viewRefreshMode>onStop</viewRefreshMode>
        <!-- number of seconds after camera stops -->
		<viewRefreshTime>0</viewRefreshTime>
	</Link>
</NetworkLink>
</kml>";

    }

    public static class LiveCameraRegistryFactory
    {

        static ILiveCamRegistry CreateRegistry()
        {
            return new InMemoryLiveCamRegistry();
        }
    }

    public class InMemoryLiveCamRegistry : ILiveCamRegistry
    {

        private Dictionary<string, KmlLiveCam> activeLiveCams = new Dictionary<string, KmlLiveCam>();

        /// <exception cref="UriFormatException">malformed url</exception>
        private string getAlias(string url)
        {
            return new Uri(url).LocalPath;
        }

        public bool TryGetByAlias(string alias, out KmlLiveCam kmlLiveCam)
        {
            return activeLiveCams.TryGetValue(alias, out kmlLiveCam);
        }

        // "Load" implies "create if not found in registry"
        /// <exception cref="UriFormatException">malformed url</exception>
        public KmlLiveCam LoadByUrl(string url)
        {
            string alias = getAlias(url);
            KmlLiveCam liveCam;
            if (!TryGetByAlias(alias, out liveCam))
            {
                liveCam = createNewLiveCam(alias, url);
                activeLiveCams[alias] = liveCam;
            }
            return liveCam;
        }

        private KmlLiveCam createNewLiveCam(string alias, string url)
        {
            KmlLiveCam liveCam = new KmlLiveCam();
            liveCam.Camera.Template = LiveCamConstants.DefaultCameraKmlTemplate;
            liveCam.Link.Template = LiveCamConstants.DefaultNetworkLinkKmlTemplate;
            liveCam.Link.Values = new KmlNetworkLinkValues(alias, url);
            return liveCam;
        }

    }

}
