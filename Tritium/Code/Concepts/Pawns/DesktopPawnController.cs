using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Tritium.Input;

namespace Tritium.Concepts.Pawns
{
    public class DesktopPawnController : IPawnController
    {
        public KeyButton wButton, sButton, aButton, dButton;

        public DesktopPawnController()
        {
            wButton = InputSystem.BindKeyButton(Keys.W);
            sButton = InputSystem.BindKeyButton(Keys.S);
            aButton = InputSystem.BindKeyButton(Keys.A);
            dButton = InputSystem.BindKeyButton(Keys.D);
        }

        public Vector2 GetCursor()
        {
            return Vector2.Zero;
        }

        // TODO: Resolve monogame coordinates properly?
        public Vector2 GetHeading()
        {
            float y = (wButton.pressed ? -1 : (sButton.pressed ? 1 : 0));
            float x = (dButton.pressed ? 1 : (aButton.pressed ? -1 : 0));

            return new Vector2(x, y);
        }
    }
}
