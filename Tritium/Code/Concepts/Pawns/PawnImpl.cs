using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Concepts.Pawns
{
    public class PawnImpl : Impl
    {
        public PawnImpl() : base() { }
        public PawnImpl(string implName, bool isAbstract) : base(implName, isAbstract) { }


        public PawnEntity.PawnFeatureLayout anatomy = new PawnEntity.PawnFeatureLayout(); // Everything begins off 1 point anotomically
    }
}
