using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WatsonWebserver;

namespace FS2020PlanePath
{

    public class InternalWebServer
    {

        private readonly List<string> supportedSchemes = new List<string> { Uri.UriSchemeHttp, Uri.UriSchemeHttps };
        private readonly Func<string, string> pathHandler;
        private readonly Uri webHostUri;

        private Server server;

        public InternalWebServer(Uri webHostUri, Func<string, string> pathHandler)
        {
            if (!supportedSchemes.Contains(webHostUri.Scheme)) {
                throw new ArgumentException($"unsupported URI scheme: {webHostUri.Scheme}");
            }
            this.pathHandler = pathHandler;
            this.webHostUri = webHostUri;
        }

        public void Enable() {
            if (server == null)
            {
                bool ssl = webHostUri.Scheme == Uri.UriSchemeHttps;
                // see: https://github.com/jchristn/WatsonWebserver/wiki/Using-SSL-on-Windows
                server = new Server(webHostUri.Host, webHostUri.Port, ssl, RequestHandler);
                server.Start();
                Console.WriteLine($"{GetType().Name} listening at({webHostUri})");
            }
        }

        public void Disable()
        {
            if (server != null)
            {
                if (server.IsListening) {
                    server.Stop();
                    Console.WriteLine($"{GetType().Name} stopped listening");
                }
                server.Dispose();
                server = null;
            }
        }

        public Uri Uri {
            get {
                return webHostUri;
            }
        }

        async Task RequestHandler(HttpContext context)
        {
            await context.Response.Send(pathHandler.Invoke(context.Request.Url.RawWithoutQuery));
        }

    }
}
