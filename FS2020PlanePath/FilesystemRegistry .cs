using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FS2020PlanePath
{

    public class FilesystemRegistry<T> : IRegistry<T>
    {

        string fileNamePrefix;
        string fileNameSuffix;

        public FilesystemRegistry(string fileNamePrefix)
        {
            this.fileNamePrefix = fileNamePrefix;
            this.fileNameSuffix = ".json";
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
                    serializer.Formatting = Formatting.Indented;
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

        public List<string> GetAliases()
        {
            FileInfo[] foundFiles = new DirectoryInfo(".").GetFiles(FilenameForId("*"));
            Array.Sort(foundFiles, (f1, f2) => f2.LastAccessTimeUtc.CompareTo(f1.LastAccessTimeUtc));
            List<string> aliases = new List<string>();
            foreach (FileInfo foundFile in foundFiles)
            {
                string foundFileName = foundFile.Name;
                string aliasLeft = foundFileName.Substring(fileNamePrefix.Length);
                aliases.Add(aliasLeft.Remove(aliasLeft.Length - fileNameSuffix.Length));
            }
            // list of liveCam files, most recently accessed first
            return aliases;
        }

        private string FilenameForId(string cleanId)
        {
            return $"{fileNamePrefix}{cleanId}{fileNameSuffix}";
        }

    }

}
