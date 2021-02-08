using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SharpKml.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using RazorEngineCore;

namespace FS2020PlanePath
{

    public delegate string TemplateRendererErrorHandler(string message, string details);

    public class TemplateRendererFactory
    {

        public TemplateRendererErrorHandler rendererErrorHandler;

        public TemplateRendererFactory(TemplateRendererErrorHandler rendererErrorHandler)
        {
            this.rendererErrorHandler = rendererErrorHandler;
        }

        public IStringTemplateRenderer<T> newTemplateRenderer<T>(string template)
        {

            if (template.TrimStart().StartsWith("@{"))
            {
                // template is a Razor script which begins with "@{"
                return new RazorTemplateRenderer<T>(template, rendererErrorHandler);
            }

            if (template.TrimStart().StartsWith("<"))
            {
                // template is KML text with placeholders
                return new KmlTemplateRenderer<T>(template, rendererErrorHandler);
            }

            // template is a script
            return new ScriptTemplateRenderer<T>(template, rendererErrorHandler);
        }

    }

    /// <summary>
    /// Razor Script whose string return value will become the rendered result
    /// </summary>
    /// <typeparam name="V">type of object whose property values will be available to the script during its execution</typeparam>
    public class RazorTemplateRenderer<V> : IStringTemplateRenderer<V>
    {

        private string sourceRazorTemplate;
        private IRazorEngineCompiledTemplate compiledRazorTemplate;
        private TemplateRendererErrorHandler rendererErrorHandler;
        private string errors;

        public RazorTemplateRenderer(string razorTemplate, TemplateRendererErrorHandler rendererErrorHandler)
        {
            sourceRazorTemplate = razorTemplate;
            this.rendererErrorHandler = rendererErrorHandler;
        }

        public string Render(V propertyValues)
        {
            compile();
            if (errors != null)
            {
                return rendererErrorHandler("error(s) evaluating razor script", errors);
            }
            try
            {
                return compiledRazorTemplate.Run(propertyValues);
            } catch(Exception rbe)
            {
                return rendererErrorHandler("script threw exception", rbe.Message);
            }
        }

        private void compile()
        {
            if (compiledRazorTemplate == null)
            {
                try
                {
                    compiledRazorTemplate = new RazorEngine().Compile(sourceRazorTemplate);
                }
                catch (Exception ex)
                {
                    errors = ex.Message;
                }
            }
        }

        public string Template { get => sourceRazorTemplate; }

        public string[] Diagnostics
        {
            get
            {
                compile();
                if (errors == null)
                {
                    return new string[0];
                }
                return new string[]
                {
                    "Razor Script:",
                    errors
                };
            }
        }

    }

    /// <summary>
    /// C# Script whose string return value will become the rendered result
    /// </summary>
    /// <typeparam name="V">type of object whose property values will be available to the script during its execution</typeparam>
    public class ScriptTemplateRenderer<V> : IStringTemplateRenderer<V>
    {

        private Script<string> _script;
        private TemplateRendererErrorHandler rendererErrorHandler;

        public ScriptTemplateRenderer(string scriptTemplate, TemplateRendererErrorHandler rendererErrorHandler)
        {
            this.rendererErrorHandler = rendererErrorHandler;
            _script = CSharpScript.Create<string>(
                code: scriptTemplate,
                globalsType: typeof(V)
            ).WithOptions(
                ScriptOptions
                .Default
                .WithReferences(Assembly.GetExecutingAssembly())
            );
        }

        public string Render(V propertyValues)
        {
            Task<ScriptState<string>> scriptStateTask = _script.RunAsync(propertyValues);
            if (!scriptStateTask.Wait(2000))
            {
                return rendererErrorHandler("ERROR: script execution timed out", "");
            }
            return scriptStateTask.Result.ReturnValue;
        }

        public string Template { get => _script.Code; }

        public string[] Diagnostics
        {
            get
            {
                List<string> problems = (
                    _script.Compile()
                    .Where(d => d.Severity >= DiagnosticSeverity.Warning)
                    .Select(d => d.ToString())
                    .Take(10)
                    .ToList()
                );
                if (problems.Count == 0)
                {
                    return new string[0];
                }

                return (
                    new List<string> { "C# Script:" }
                    .Concat(problems)
                    .ToArray()
                );
            }
        }

    }

    /// <summary>
    /// KML Document text renderer
    /// </summary>
    /// <typeparam name="V">type of object whose properties supply subsitution values into the template upon rendering</typeparam>
    public class KmlTemplateRenderer<V> : IStringTemplateRenderer<V>
    {

        private string kmlTemplate;

        public KmlTemplateRenderer(string kmlTemplate, TemplateRendererErrorHandler rendererErrorHandler)
        {
            this.kmlTemplate = kmlTemplate;
        }

        public string Render(V propertyValues)
        {
            return TextRenderer.Render<V>(Template, propertyValues);
        }

        public string Template { get => kmlTemplate; }

        public string[] Diagnostics
        {
            get
            {
                try
                {
                    new Parser().ParseString(Template, true);
                    return new string[0];
                }
                catch (Exception pe)
                {
                    return new string[] {
                        "KML Parser:",
                        pe.Message
                    };
                }
            }
        }
    }

    public static class TextRenderer
    {

        /// <typeparam name="V">type of the 'propertyValues' object</typeparam>
        /// <param name="textTemplate">textual template optionally containing property references for substitution</param>
        /// <param name="propertyValues">object whose properties can be referenced within, and substituted into the text template</param>
        /// <returns>a 'rendering' of 'textTemplate' with all property referencess substituted using values from 'propertyValues'</returns>
        public static string Render<V>(string textTemplate, V propertyValues)
        {
            string textResult = textTemplate;
            foreach (PropertyInfo property in typeof(V).GetProperties())
            {
                string propertyName = $"{{{property.Name}}}";
                object propertyValue = property.GetValue(propertyValues);
                string substitutionValue = (propertyValue == null) ? "" : propertyValue.ToString();
                while (true)
                {
                    string newResult = textResult.Replace(propertyName, substitutionValue);
                    if (newResult == textResult)
                    {
                        break;
                    }
                    textResult = newResult;
                }
            }
            return textResult;
        }

        /// <param name="propertyValueType">type of object whose properties can be referenced within a template</param>
        /// <returns>array of property references that can be used within a template</returns>
        public static string[] Placeholders(Type propertyValueType)
        {
            List<string> placeholders = new List<string>();
            foreach (PropertyInfo info in propertyValueType.GetProperties())
            {
                placeholders.Add($"{{{info.Name}}}");
            }
            return placeholders.ToArray();
        }

    }

    /// <summary>
    /// A "String Template Renderer" can render a string template using a current set of property values
    /// </summary>
    /// <typeparam name="V">type of object whose properties supply subsitution values into the template upon rendering</typeparam>
    public interface IStringTemplateRenderer<V>
    {

        string Render(V propertyValues);
        string Template { get; }
        string[] Diagnostics { get; }
    }

}
