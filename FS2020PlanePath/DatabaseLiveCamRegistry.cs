using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS2020PlanePath
{
    class DatabaseLiveCamRegistry : ILiveCamRegistry
    {
        public KmlLiveCam LoadByUrl(string url)
        {
            throw new NotImplementedException();
        }

        public bool TryGetByAlias(string alias, out KmlLiveCam kmlLiveCam)
        {
            throw new NotImplementedException();
        }
    }
}
