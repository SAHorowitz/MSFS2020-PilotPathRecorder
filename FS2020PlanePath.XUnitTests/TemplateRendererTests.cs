using Xunit;
using System;
using Xunit.Abstractions;

namespace FS2020PlanePath.XUnitTests
{

    public class RenderValues
    {
        public int iv1 { get; set; }
        public int iv2 { get; set; }
        public string sv1 { get; set; } = null;
    }

    public class TemplateRendererTests
    {

        private readonly ITestOutputHelper logger;
        private KmlCameraParameterValues[] sequence = new KmlCameraParameterValues[100];
        private TemplateRendererFactory templateRendererFactory = new TemplateRendererFactory(
            (message, details) => $"template rendering error({message}), details({details})"
        );
        private int sequence_position = 0;

        public TemplateRendererTests(ITestOutputHelper logger)
        {
            this.logger = logger;

            for (int i = 0; i < sequence.Length; i++)
            {
                KmlCameraParameterValues kmlCameraParameterValues = new KmlCameraParameterValues();
                kmlCameraParameterValues.seq = i;
                sequence[i] = kmlCameraParameterValues;
            }

        }

        /// <summary>
        /// Demonstrate basic and comparable functioning of template renderers
        /// supported by the "Generic" template renderer.
        /// </summary>
        [Fact]
        public void BasicGenericTemplateRendererTest()
        {
            string[] templateVersions =
            {
                "<kml iv1='{iv1}' sv1='{sv1}' />",          // KML template
                "@$\"<kml iv1='{iv1}' sv1='{sv1}' />\""     // Script template
            };
            foreach (string templateVersion in templateVersions)
            {
                IStringTemplateRenderer<RenderValues> testRenderer = templateRendererFactory.newTemplateRenderer<RenderValues>(templateVersion);
                Assert.Equal("<kml iv1='9965' sv1='' />", testRenderer.Render(new RenderValues { iv1 = 9965 }));
            }
        }

        [Fact]
        public void TestScriptContextWithFunctions()
        {
            IStringTemplateRenderer<ScriptContext> renderer = templateRendererFactory.newTemplateRenderer<ScriptContext>(
                "GetFlights(seq).Length.ToString()"
            );

            string[] results = { Invoke(renderer, 3), Invoke(renderer, 7), Invoke(renderer, 12) };
            Assert.Equal(new string[] { "3", "4", "5" }, results);
        }

        /// <summary>
        /// Script context that includes helper functions and execution instance values
        /// </summary>
        public class ScriptContext
        {
            public Func<int, KmlCameraParameterValues[]> GetFlights { get; set; }
            public int seq { get; set; }
        }

        private string Invoke(IStringTemplateRenderer<ScriptContext> renderer, int seq)
        {
            return renderer.Render(
                new ScriptContext
                {
                    GetFlights = s => NextKmlCameraParameterValues(s),
                    seq = seq
                }
            );
        }

        private KmlCameraParameterValues[] NextKmlCameraParameterValues(int seq)
        {
            int size = Math.Max(Math.Min(seq, sequence.Length) - sequence_position, 0);
            KmlCameraParameterValues[] kmlCameraParameterValues = new KmlCameraParameterValues[size];
            for (int i = 0; i < size; i++)
            {
                kmlCameraParameterValues[i] = sequence[sequence_position + i];
            }
            sequence_position += size;
            return kmlCameraParameterValues;
        }

    }

}
