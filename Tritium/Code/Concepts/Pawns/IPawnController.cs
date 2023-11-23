using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Tritium.Concepts.Pawns
{
    public interface IPawnController
    {
        public Vector2 GetHeading();
        public Vector2 GetCursor();
    }
}
