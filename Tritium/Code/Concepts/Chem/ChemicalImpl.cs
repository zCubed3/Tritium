using Microsoft.Xna.Framework;

namespace Tritium.Concepts.Chem
{
    public class ChemicalImpl : Impl
    {
        public ChemicalImpl() : base() { }
        public ChemicalImpl(string implName, bool isAbstract) : base(implName, isAbstract) { }
        
        public string chemName = "Unknown Chem";
        public Color chemColor = Color.Pink;
    }
}