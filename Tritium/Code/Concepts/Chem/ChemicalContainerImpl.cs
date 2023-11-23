using Microsoft.Xna.Framework;

namespace Tritium.Concepts.Chem
{
    public class ChemicalContainerImpl : Impl
    {
        public ChemicalContainerImpl() : base() { }
        public ChemicalContainerImpl(string implName, bool isAbstract) : base(implName, isAbstract) { }

        public int unitCapacity = 50;
        public Sprite sprite;
    }
}