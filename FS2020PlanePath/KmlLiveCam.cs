using System;
using System.Reflection;
using System.Collections.Generic;

namespace FS2020PlanePath
{

    public class KmlLiveCam : ILiveCam<KmlCameraParameterValues, KmlNetworkLinkValues>
    {

        public KmlLiveCam(string cameraTemplate, string linkTemplate, KmlNetworkLinkValues linkValues)
        {
            Camera.Template = cameraTemplate;
            Link.Template = linkTemplate;
            Link.Values = linkValues;
        }

        public IStringTemplateRenderer<KmlCameraParameterValues> Camera { get; } = new Renderer<KmlCameraParameterValues>();

        public IStringTemplateRenderer<KmlNetworkLinkValues> Link { get; } = new Renderer<KmlNetworkLinkValues>();

    }

    // see: https://developers.google.com/kml/documentation/kmlreference#camera
    public class KmlCameraParameterValues
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public double altitude { get; set; }    // meters
        public double heading { get; set; }
        public double tilt { get; set; }
        public double roll { get; set; }
    }

    // see: https://developers.google.com/kml/documentation/kmlreference#networklink
    public class KmlNetworkLinkValues
    {
        private string _url;
        private string _alias;

        public KmlNetworkLinkValues(string alias, string url)
        {
            _alias = alias;
            _url = url;
        }

        public string alias { get { return _alias; } }
        public string url { get { return _url; } }

    }

    public class Renderer<V> : IStringTemplateRenderer<V>
    {

        public string Render()
        {
            return TemplateRenderer.Render<V>(Template, Values);
        }

        public string Template { get; set; }
        public V Values { get; set; }

    }

    public static class TemplateRenderer
    {

        public static string Render<V>(string template, V values)
        {
            string result = template;
            foreach (PropertyInfo info in typeof(V).GetProperties())
            {
                string substitutionTokenName = $"{{{info.Name}}}";
                string substitutionTokenValue = info.GetValue(values).ToString();
                while (true)
                {
                    string newResult = result.Replace(substitutionTokenName, substitutionTokenValue);
                    if (newResult == result)
                    {
                        break;
                    }
                    result = newResult;
                }
            }
            return result;
        }

        public static string[] Placeholders(Type valueType)
        {
            List<string> placeholders = new List<string>();
            foreach (PropertyInfo info in valueType.GetProperties())
            {
                placeholders.Add($"{{{info.Name}}}");
            }
            return placeholders.ToArray();
        }

    }

    /** A "Live Cam" has a "Camera", which can be "called back" repeatedly through a "Link" */
    public interface ILiveCam<CV, LV>
    {

        /** A "Camera" renders a snapshot using current values */
        IStringTemplateRenderer<CV> Camera { get; }

        /** A "Link" renders a callback to the "Camera" */
        IStringTemplateRenderer<LV> Link { get; }

    }

    /** A "String Template Renderer" can render a string template using a current set of values */
    public interface IStringTemplateRenderer<V>
    {
        string Render();
        string Template { get; set; }
        V Values { get; set; }
    }

}
