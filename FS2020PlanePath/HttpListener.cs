using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WatsonWebserver;

namespace FS2020PlanePath
{

    public class HttpListener : IDisposable
    {

        public class Request
        {
            public string path;
            public Dictionary<string, string> query;
            public Func<byte[]> GetBody;
        }

        public HttpListener(Uri webHostUri, Func<Request, byte[]> pathHandler)
        {
            if (!supportedSchemes.Contains(webHostUri.Scheme)) {
                throw new ArgumentException($"unsupported URI scheme: {webHostUri.Scheme}");
            }
            this.pathHandler = pathHandler;
            this.webHostUri = webHostUri;
        }

        public void Enable()
        {
            if (server == null)
            {
                bool ssl = webHostUri.Scheme == Uri.UriSchemeHttps;
                // see: https://github.com/jchristn/WatsonWebserver/wiki/Using-SSL-on-Windows
                server = new Server(webHostUri.Host, webHostUri.Port, ssl, RequestHandler);
            }
            if (!server.IsListening) {
                server.Start();
                Console.WriteLine($"{GetType().Name} example URL: {webHostUri}");
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
            }
        }

        public void Dispose()
        {
            Disable();
            if (server != null)
            {
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
            Console.WriteLine($"handling request({context.Request.Url.RawWithQuery})");
            byte[] listenerResponseBody = (
                pathHandler.Invoke(
                    new Request
                    {
                        path = context.Request.Url.RawWithoutQuery.Substring(1),
                        query = context.Request.Query.Elements,
                        GetBody = () => context.Request.DataAsBytes()
                    }
                )
            );
            //Console.WriteLine($"listenerResponseBody({fixupForDisplay(listenerResponseBody)})");
            await context.Response.Send(listenerResponseBody);
        }

        private string fixupForDisplay(byte[] listenerResponseBody)
        {
            return $"Length({listenerResponseBody.Length})";
        }

        private readonly List<string> supportedSchemes = new List<string> { Uri.UriSchemeHttp, Uri.UriSchemeHttps };
        private readonly Func<Request, byte[]> pathHandler;
        private readonly Uri webHostUri;

        private Server server;

    }
}
