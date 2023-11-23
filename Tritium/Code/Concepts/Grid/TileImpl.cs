using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Tritium.Assets;

namespace Tritium.Concepts.Grid
{
    public class TileImpl : Impl
    {
        public enum TileShape
        {
            Single,
            Vertical,
            Horizontal,
            Corner,
            UpperCorner,
            VerticalT,
            HorizontalT,
            VerticalCap,
            HorizontalCap,
            All
        }
        
        public enum TileLayer
        {
            UnderFloor, // aka the Hull under the floor
            Floor,
            Wall
        }

        [SubData]
        public class TileShapeData
        {
            public TileShape shape = TileShape.Single;
            public Rectangle rectangle;
            public Texture2DCRef texture;
        }

        public TileImpl() : base() { }
        public TileImpl(string implName, bool isAbstract) : base(implName, isAbstract) { }
        public TileImpl(string implName, bool isAbstract, TileShapeData section) : base(implName, isAbstract) 
        {
            tileSections.Add(section);
        }

        public List<StrictImplRef<MaterialTypeImpl>> allowedMaterialTypes;
        public StrictImplRef<MaterialImpl> defaultMaterial;
        public List<TileShapeData> tileSections = new List<TileShapeData>();
        public TileLayer layer = TileLayer.Floor;
    }
}
