using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Tritium.Drawing;

using Tritium.Assets;
using Tritium.Input;

namespace Tritium.Concepts.UI
{
    // Creates a draggable handle for a window
    // Without this a window is just a static panel
    public class UITitleBar : UIButton
    {
        public UITitleBar(string text) : base(text) { }

        public override void OnUIClick()
        {
            base.OnUIClick();
            LockSelection = true;
        }

        public override void OnUIHold()
        {
            base.OnUIHold();
            Position += InputSystem.mouseInfo.velocity;

            foreach (var child in children)
                if (child is UIEntity uiEnt)
                    uiEnt.UpdateTransform();
        }

        public override void OnUIRelease()
        {
            LockSelection = false;
        }
    }
}
