using System.Collections.Generic;

namespace FS2020PlanePath
{
    public interface IRegistry<T>
    {

        /// <returns>true if the value was found and loaded into the out parameter</returns>
        bool TryGetById(string id, out T t);

        /// <returns>true iff the item was saved</returns>
        bool Save(string id, T t);

        /// <returns>true iff the item was removed</returns>
        bool Delete(string id);

        /// <returns>list of known aliases, most recently accessed first</returns>
        List<string> GetAliases();
    }

}
