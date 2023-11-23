using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Tritium.Concepts
{
    // Creates a material
    // This pseudo-type is then used by other Impl's to restrict materials in building
    public class MaterialImpl : Impl
    {
        public MaterialImpl() : base() { }
        public MaterialImpl(string implName, bool isAbstract) : base(implName, isAbstract) { }
        public MaterialImpl(string implName, bool isAbstract, StrictImplRef<MaterialTypeImpl> materialType, bool flammable = false, Color color = default)
            : base(implName, isAbstract)
        {
            this.materialType = materialType;
            this.flammable = flammable;
            this.color = color;
        }

        public StrictImplRef<MaterialTypeImpl> materialType = new StrictImplRef<MaterialTypeImpl>();

        public bool flammable = false;
        public Color color = Color.Magenta;
    }
}
