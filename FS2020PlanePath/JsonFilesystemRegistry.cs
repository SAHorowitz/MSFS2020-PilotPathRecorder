using System;
using System.Collections.Generic;
using System.IO;

namespace FS2020PlanePath
{

    public class JsonFilesystemRegistry<T> : IRegistry<T>
    {

        private string folderNamePrefix;
        private string fileNamePrefix;
        private string fileNameSuffix;

        public JsonFilesystemRegistry(string folderNamePrefix, string fileNamePrefix)
        {
            this.folderNamePrefix = folderNamePrefix;
            this.fileNamePrefix = fileNamePrefix;
            this.fileNameSuffix = ".json";
        }

        public bool TryGetById(string id, out T value)
        {
            string fileName = FilenameForId(id);
            try
            {
                value = new JsonSerializer<T>().Deserialize(File.ReadAllText(fileName));
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

        public bool Save(string id, T value)
        {
            string fileName = FilenameForId(id);
            try
            {
                File.WriteAllText(fileName, new JsonSerializer<T>().Serialize(value));
                Console.WriteLine($"saved({fileName})");
                return true;
            } catch(Exception ex)
            {
                Console.WriteLine($"Can't save({fileName}); {ex.Message}");
                return false;
            }
        }

        public bool Delete(string id)
        {
            string fileName = FilenameForId(id);
            try
            {
                File.Delete(fileName);
                Console.WriteLine($"deleted({fileName})");
                return true;
            } catch(Exception ex)
            {
                Console.WriteLine($"Can't delete({fileName}); {ex.Message}");
                return false;
            }
        }

        public List<string> GetIds(int maxCount)
        {
            // TODO use "EnumerateFiles" to limit based upon 'maxCount'
            // in order to avoid bringing all into memory
            FileInfo[] foundFiles = new DirectoryInfo(".").GetFiles(FilenameFor("*"));
            Array.Sort(foundFiles, (f1, f2) => f2.LastAccessTimeUtc.CompareTo(f1.LastAccessTimeUtc));
            List<string> ids = new List<string>();
            int fileCount = 0;
            foreach (FileInfo foundFile in foundFiles)
            {
                string foundFileName = foundFile.Name;
                string idsLeft = foundFileName.Substring(fileNamePrefix.Length);
                string filesystemId = idsLeft.Remove(idsLeft.Length - fileNameSuffix.Length);
                ids.Add(Uri.UnescapeDataString(filesystemId));
                if (maxCount >= 0 && fileCount >= maxCount)
                {
                    break;
                }
                fileCount++;
            }
            // list of ids found in the file system, most recently accessed first
            Console.WriteLine($"retrieved({ids.Count}) filesystem ids");
            return ids;
        }

        /// <returns>filename for filesystem-safe version of 'id'</returns>
        public string FilenameForId(string id)
        {
            return FilenameFor(Uri.EscapeDataString(id));
        }

        /// <returns>filename for 'filesystemId'</returns>
        public string FilenameFor(string filesystemId)
        {
            return $"{folderNamePrefix}{fileNamePrefix}{filesystemId}{fileNameSuffix}";
        }

    }

}
