using Xunit;
using Xunit.Abstractions;
using FS2020PlanePath;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace FS2020PlanePath.XUnitTests
{

    public class InternalWebServerTests
    {

        private readonly ITestOutputHelper logger;

        public InternalWebServerTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Fact]
        public void TestSimpleGetPath()
        {
            Uri localUri = new Uri("http://localhost:8000/xyz");
            using (LiveCamLinkListener ws = new LiveCamLinkListener(localUri, path => $"~/{path}/~")) {
                ws.Enable();
                (var status, var body) = HttpGet(localUri);
                string testResultString = $"status({status}); responseBody({body})";
                logger.WriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}: result({testResultString})");
                Assert.Equal("status(OK); responseBody(~//xyz/~)", testResultString);
            }
        }

        [Fact]
        public void TestEnablement()
        {
            Uri localUri = new Uri("http://localhost:8000/xyz");
            Action clientAction = () => HttpGet(localUri);
            using (LiveCamLinkListener ws = new LiveCamLinkListener(localUri, path => $"~/{path}/~"))
            {
                Assert.ThrowsAny<Exception>(clientAction);  // created but not enabled
                ws.Enable();
                clientAction.Invoke();                      // should be available now
                ws.Disable();
                Assert.ThrowsAny<Exception>(clientAction);  // disabled; not available
                ws.Enable();
                clientAction.Invoke();                      // re-enabled; available again
                ws.Dispose();
                Assert.ThrowsAny<Exception>(clientAction);  // disposed; not available
            }
        }

        private (HttpStatusCode status, string resultBody) HttpGet(Uri uri)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(1);
            HttpResponseMessage result = httpClient.GetAsync(uri).Result;
            return (result.StatusCode, result.Content.ReadAsStringAsync().Result);
        }

    }

}
