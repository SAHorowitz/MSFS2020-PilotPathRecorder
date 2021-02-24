using System;
using System.IO;
using Newtonsoft.Json;

namespace FS2020PlanePath
{

    public static class Parser
    {

        public static T Convert<T>(string s, Func<string, T> converter, Func<T> failureSupplier)
        {
            try
            {
                return converter.Invoke(s);
            }
            catch (Exception e)
            {
                T fallbackValue = failureSupplier.Invoke();
                Console.WriteLine($"converting({s}) to({typeof(T).Name}) generated({e.Message}); fallback({fallbackValue})");
                return fallbackValue;
            }
        }

    }

    public static class FilesystemSerializer
    {

        public static bool TryDeserializeFromFile<T>(
            string fileName, 
            ISerializer<T, string> deserializer,  
            out T value
        )
        {
            try
            {
                value = deserializer.Deserialize(File.ReadAllText(fileName));
                Console.WriteLine($"loaded({fileName})");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't load({fileName}); {ex.Message}");
                value = default(T);
                return false;
            }
        }

        public static bool TrySerializeToFile<T>(
            string fileName, 
            ISerializer<T, string> serializer,
            T value
        )
        {
            try
            {
                File.WriteAllText(fileName, serializer.Serialize(value));
                Console.WriteLine($"saved({fileName})");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't save({fileName}); {ex.Message}");
                return false;
            }
        }

    }

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
