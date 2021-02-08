using Xunit;
using Xunit.Abstractions;
using System;
using System.Text;
using System.Net;
using System.Net.Http;

namespace FS2020PlanePath.XUnitTests
{

    public class HttpListenerTests
    {

        private readonly ITestOutputHelper logger;

        public HttpListenerTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Fact]
        public void TestSimpleGetPath()
        {
            Uri localUri = new Uri("http://localhost:8000/kmlcam/xyz");
            using (HttpListener ws = new HttpListener(localUri, request => s2b($"~/{request.path}/~"))) {
                ws.Enable();
                (var status, var body) = HttpGet(localUri);
                string testResultString = $"status({status}); responseBody({body})";
                //logger.WriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}: result({testResultString})");
                Assert.Equal("status(OK); responseBody(~/kmlcam/xyz/~)", testResultString);
            }
        }

        [Fact]
        public void TestEnablement()
        {
            Uri localUri = new Uri("http://localhost:8000/kmlcam/xyz");
            Action clientAction = () => HttpGet(localUri);
            using (HttpListener ws = new HttpListener(localUri, path => s2b($"~/{path}/~")))
            {
                Assert.ThrowsAny<Exception>(clientAction);  // created but not enabled; should throw exception
                ws.Enable();
                clientAction.Invoke();                      // should be available now
                ws.Disable();
                Assert.ThrowsAny<Exception>(clientAction);  // disabled, not available; should throw exception
                ws.Enable();
                clientAction.Invoke();                      // re-enabled, available again
                ws.Dispose();
                Assert.ThrowsAny<Exception>(clientAction);  // disposed, not available; should throw exception
            }
        }

        private (HttpStatusCode status, string resultBody) HttpGet(Uri uri)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(1);
            HttpResponseMessage result = httpClient.GetAsync(uri).Result;
            return (result.StatusCode, result.Content.ReadAsStringAsync().Result);
        }

        private byte[] s2b(string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

    }

}
