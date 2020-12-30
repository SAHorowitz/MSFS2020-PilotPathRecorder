using System;
using System.Threading.Tasks;
using WatsonWebserver;

namespace FS2020PlanePath
{

    public class InternalWebServer
    {

        private Server _Server;
        private Func<string, string> handler;

        public void Enable(
            string hostname,
            int port,
            Func<string, string> handler
        ) {
            if (_Server == null)
            {
                _Server = new Server(hostname, port, false, DefaultRequest);
                _Server.Start();
                this.handler = handler;
            }
        }

        public void Disable()
        {
            if (_Server != null)
            {
                _Server.Stop();
                _Server = null;
            }
        }

        async Task DefaultRequest(HttpContext context)
        {
            await context.Response.Send(handler.Invoke(context.Request.Url.RawWithoutQuery));
        }

    }
}
