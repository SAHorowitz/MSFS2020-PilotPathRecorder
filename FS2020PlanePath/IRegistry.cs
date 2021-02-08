using System.Collections.Generic;

namespace FS2020PlanePath
{
    public interface IRegistry<T>
    {

        /// <returns>
        ///     true if the value was found and loaded into the out parameter
        ///     or false if not, with 't' being set to its default value
        /// </returns>
        bool TryGetById(string id, out T t);

        /// <returns>true iff the item was saved</returns>
        bool Save(string id, T t);

        /// <returns>true iff the item was removed</returns>
        bool Delete(string id);

        /// <param name="maxCount">maximum size of list to return (negative means 'unlimited')</param>
        /// <returns>list of known ids, most recently accessed first</returns>
        List<string> GetIds(int maxCount = -1);
    }

}
