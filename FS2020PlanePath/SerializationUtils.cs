using System;
using System.IO;
using Newtonsoft.Json;

namespace FS2020PlanePath
{
    public class JsonSerializer<V> : ISerializer<V, string>
    {
        public V Deserialize(string s)
        {
            using (StringReader stringReader = new StringReader(s))
            {
                return (V) new JsonSerializer().Deserialize(stringReader, typeof(V));
            }
        }

        public string Serialize(V v)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                try
                {
                    serializer.Serialize(stringWriter, v);
                } catch(Exception e)
                {
                    // trying to serialize methods or other bad things?
                    // see: https://www.newtonsoft.com/json/help/html/ConditionalProperties.htm
                    Console.WriteLine($"ERROR: caught({e.Message})");
                }
                return stringWriter.ToString();
            }
        }

    }

    public interface ISerializer<V, S>
    {
        S Serialize(V v);
        V Deserialize(S s);
    }

}
