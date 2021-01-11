using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FS2020PlanePath
{

    public class FilesystemRegistry<T> : IRegistry<T>
    {

        string fileNamePrefix;

        public FilesystemRegistry(string fileNamePrefix)
        {
            this.fileNamePrefix = fileNamePrefix;
        }

        public bool TryGetById(string id, out T value)
        {
            try
            {
                using (StreamReader file = File.OpenText(FilenameForId(id)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    value = (T) serializer.Deserialize(file, typeof(T));
                }
                return true;
            }
            catch (Exception e)
            {
                value = default(T);
                return false;
            }
        }

        public bool Save(string id, T value)
        {
            try
            {
                using (StreamWriter file = File.CreateText(FilenameForId(id)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, value);
                }
                return true;
            } catch(Exception e)
            {
                return false;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                File.Delete(FilenameForId(id));
                return true;
            } catch(Exception e)
            {
                return false;
            }
        }

        private string FilenameForId(string id)
        {
            return $"{fileNamePrefix}{fnClean(id)}.json";
        }

        private object fnClean(string alias)
        {
            return Path.GetInvalidFileNameChars().Aggregate(alias, (current, c) => current.Replace(c, '_'));
        }

    }

}
