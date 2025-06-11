using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;

namespace NetVigia.BLL
{
    public static class Map<D>
    {
        public static D Convert<O>(O o)
        {
            if (o == null)
                return default(D);

            return o.Adapt<D>();
        }
    }
}
