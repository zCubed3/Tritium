using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;



namespace Tritium.Input
{
    public static class InputSystem
    {
        // Mouse info struct since we hold lots of data for the mouse
        public static MouseInfo mouseInfo;

        public static List<Button> buttons;

        public static KeyboardState keyboardState { get; private set; }
        public static MouseState mouseState { get; private set; }

        public static void Initialize()
        {
            buttons = new List<Button>();
            mouseInfo = new MouseInfo();
        }

        public static void Update()
        {
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();

            mouseInfo.Update(mouseState);

            buttons.ForEach(button => button.Update());
        }

        // Returns a new button with the given binding or returns an existing key
        public static KeyButton BindKeyButton(Keys key)
        {
            // TODO: Don't use LINQ?
            KeyButton existingKey = buttons.Where(button => button is KeyButton).Cast<KeyButton>().FirstOrDefault(button => button.listenKey == key);

            if (existingKey == null)
            {
                KeyButton button = new KeyButton() { listenKey = key };
                buttons.Add(button);
                return button;
            }
            else
                return existingKey;
        }

        // Binds a given mouse button but if it already exists, just returns the existing binding
        public static MouseButton BindMouseButton(MouseButton.MouseButtons mouseButton)
        {
            MouseButton existingButton = buttons.Where(button => button is MouseButton).Cast<MouseButton>().FirstOrDefault(button => button.mouseButton == mouseButton);

            if (existingButton == null)
            {
                MouseButton button = new MouseButton() { mouseButton = mouseButton };
                buttons.Add(button);
                return button;
            }
            else
                return existingButton;
        }
    }
}
