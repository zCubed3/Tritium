using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Tritium.Concepts
{
    // TODO: Make type implement base properties?
    // Type doesn't define properties, individual materials do
    // This serves as a strict way to implement common types
    public class MaterialTypeImpl : Impl
    {
        public MaterialTypeImpl() : base() { }
        public MaterialTypeImpl(string implName, bool isAbstract) : base(implName, isAbstract) { }
        public MaterialTypeImpl(string implName, bool isAbstract, string typeName) : base(implName, isAbstract) 
        {
            this.typeName = typeName;
        }

        public string typeName = "UnknownType";
    }
}
