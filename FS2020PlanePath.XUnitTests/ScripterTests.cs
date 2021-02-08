using Xunit;
using Xunit.Abstractions;
using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace FS2020PlanePath.XUnitTests
{

    // TODO - write some real tests
    public class ScripterTests
    {

        private readonly ITestOutputHelper logger;

        public ScripterTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Fact]
        public void TestSimpleCSharpScript()
        {
            interpret("return \"hello, there - this is interpreted\";");
            compile("return \"hello, there - this is compiled\";");
            logger.WriteLine("done");
        }

        async Task interpret(string statement)
        {
            object result = await CSharpScript.EvaluateAsync(statement);
            logger.WriteLine($"eval result({result.ToString()})");
        }

        async Task compile(string statement)
        {
            var compiled = CSharpScript.Create(statement);
            for (var i = 0; i < 5; i++)
            {
                object result = await compiled.RunAsync();
                logger.WriteLine($"run result({result.ToString()})");
            }
        }


    }


}
