using Microsoft.Xna.Framework;

namespace Tritium.Concepts.Pawns
{
    public class NullPawnController : IPawnController
    {
        public Vector2 GetCursor() => Vector2.Zero;
        public Vector2 GetHeading() => Vector2.Zero;
    }
}
