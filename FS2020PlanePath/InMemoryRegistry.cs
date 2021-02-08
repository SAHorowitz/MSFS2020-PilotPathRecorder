using System.Collections.Generic;
using System.Linq;

namespace FS2020PlanePath
{

    public class InMemoryRegistry<T> : IRegistry<T>
    {

        private Dictionary<string, T> activeLiveCams = new Dictionary<string, T>();

        public bool TryGetById(string alias, out T kmlLiveCam)
        {
            return activeLiveCams.TryGetValue(alias, out kmlLiveCam);
        }

        public bool Save(string alias, T kmlLiveCam)
        {
            activeLiveCams[alias] = kmlLiveCam;
            return true;
        }

        public bool Delete(string alias)
        {
            return activeLiveCams.Remove(alias);
        }

        /// <summary>
        /// NOTE: violates contract; does not return in order of access
        /// </summary>
        public List<string> GetIds(int maxCount)
        {
            return new List<string>(
                maxCount < 0
              ? activeLiveCams.Keys
              : activeLiveCams.Keys.Take(maxCount)
            );
        }

    }

}
