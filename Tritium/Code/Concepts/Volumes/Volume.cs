using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Concepts.Volumes
{
    // Tiles know what Volume they're within
    // A grid holds a list of "rooms", each room contains a volume
    public class Volume
    {
        public List<Gas> containedGasses = new List<Gas>();
    }
}
