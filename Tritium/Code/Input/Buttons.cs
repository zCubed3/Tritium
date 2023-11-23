using System;
using System.Xml.Linq;

using Microsoft.Xna.Framework.Input;



namespace Tritium.Input
{
    public abstract class Button
    {
        public Action onPress, onRelease, onHeld;

        public bool pressed, released, justPressed, justReleased;

        public abstract void Update();

        public virtual void DoActions()
        {
            if (justPressed)
                onPress?.Invoke();

            if (justReleased)
                onRelease?.Invoke();

            if (pressed)
                onHeld?.Invoke();
        }
    }

    public class KeyButton : Button
    {
        public Keys? listenKey = null;

        public override void Update()
        {
            if (listenKey == null)
            {
                InputSystem.buttons.Remove(this);
                throw new Exception(
                    "KeyButton was created without a bound key, this could've been the fault of a bad Xml file!");
            }

            // At this point the key is safe to be unnullified
            Keys key = (Keys)listenKey;

            bool down = InputSystem.keyboardState.IsKeyDown(key);

            switch (pressed)
            {
                // Probably not pretty but hey, it works and it does its job
                case true when justPressed:
                    justPressed = false;
                    break;
                case false when down:
                    justPressed = true;
                    break;
            }

            switch (released)
            {
                case true when justReleased:
                    justReleased = false;
                    break;
                case false when !down:
                    justReleased = true;
                    break;
            }

            pressed = down;
            released = !down;

            DoActions();
        }
    }

    public class MouseButton : Button
    {
        public enum MouseButtons { LeftMouse, MiddleMouse, RightMouse, Mouse4, Mouse5 }

        public MouseButtons? mouseButton;

        public override void Update()
        {
            bool Terminate()
            {
                InputSystem.buttons.Remove(this);
                throw new Exception(
                    "MouseButton was created without a bound (or possibly with a null) index, this could've been the fault of a bad Xml file!");
            }

            bool clicked = mouseButton switch
            {
                MouseButtons.LeftMouse => InputSystem.mouseState.LeftButton == ButtonState.Pressed,
                MouseButtons.MiddleMouse => InputSystem.mouseState.MiddleButton == ButtonState.Pressed,
                MouseButtons.RightMouse => InputSystem.mouseState.RightButton == ButtonState.Pressed,
                MouseButtons.Mouse4 => InputSystem.mouseState.XButton1 == ButtonState.Pressed,
                MouseButtons.Mouse5 => InputSystem.mouseState.XButton2 == ButtonState.Pressed,
                _ => Terminate()
            };

            switch (pressed)
            {
                // Probably not pretty but hey, it works and it does its job
                case true when justPressed:
                    justPressed = false;
                    break;
                case false when clicked:
                    justPressed = true;
                    break;
            }

            switch (released)
            {
                case true when justReleased:
                    justReleased = false;
                    break;
                case false when !clicked:
                    justReleased = true;
                    break;
            }

            pressed = clicked;
            released = !clicked;

            DoActions();
        }
    }
}