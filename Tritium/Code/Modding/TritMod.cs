using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Modding
{
    public abstract class TritMod
    {
        /// <summary>
        /// Called when the mod is loaded (the dll is loaded)
        /// </summary>
        public virtual void OnModLoad() {}
    }
}
