using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Concepts.Volumes
{
    // Not an entity because it's not physical, it's just data
    // TODO: More complexity? Like gas mixing?
    public class Gas 
    {
        public GasImpl impl;
        public float amount;
    }
}
