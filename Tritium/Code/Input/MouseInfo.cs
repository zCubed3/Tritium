using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tritium.Input
{
    public class MouseInfo
    {
        public Vector2 velocity;
        public Vector2 position, lastPosition;

        protected int lastScrollValue;

        public Vector2 scrollDelta;

        public MouseButton leftButton, rightButton, middleButton;

        public MouseInfo()
        {
            leftButton = InputSystem.BindMouseButton(MouseButton.MouseButtons.LeftMouse);
        }

        public void Update(MouseState state)
        {
            position = state.Position.ToVector2();
            velocity = position - lastPosition;

            // TODO: Horizontal scrolling?
            scrollDelta.Y = (state.ScrollWheelValue - lastScrollValue) / 1000F;

            lastScrollValue = state.ScrollWheelValue;
            lastPosition = position;
        }
    }
}
