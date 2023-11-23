using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using NumStyle = System.Globalization.NumberStyles;

namespace Tritium
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Guesses what the given hexcode's format is, either ARGB or RGB
        /// </summary>
        /// <param name="hexcode"></param>
        /// <returns></returns>
        public static void LoadHex(this ref Color color, string hexcode)
        {
            if (hexcode.Length >= 8 && hexcode.Length < 10)
                color.LoadARGBHex(hexcode);
            else if (hexcode.Length >= 6 && hexcode.Length < 8)
                color.LoadRGBHex(hexcode);
            else
                color = Color.Magenta; // Something went wrong!
        }

        public static void LoadRGBHex(this ref Color color, string hexcode)
        {
            // First it needs to be 7 or 6 characters
            if (hexcode.Length >= 6 && hexcode.Length < 8)
            {
                string code = hexcode.Replace("#", "");

                int rChannel = int.Parse($"{code[0]}{code[1]}", NumStyle.HexNumber);
                int gChannel = int.Parse($"{code[2]}{code[3]}", NumStyle.HexNumber);
                int bChannel = int.Parse($"{code[4]}{code[5]}", NumStyle.HexNumber);

                color = new Color(rChannel, gChannel, bChannel, 255);
                return;
            }

            color = Color.Magenta;
        }

        public static void LoadARGBHex(this ref Color color, string hexcode)
        {
            // First it needs to be 9 or 8 characters
            if (hexcode.Length >= 8 && hexcode.Length < 10)
            {
                string code = hexcode.Replace("#", "");

                int aChannel = int.Parse($"{code[0]}{code[1]}", NumStyle.HexNumber);
                int rChannel = int.Parse($"{code[2]}{code[3]}", NumStyle.HexNumber);
                int gChannel = int.Parse($"{code[4]}{code[5]}", NumStyle.HexNumber);
                int bChannel = int.Parse($"{code[6]}{code[7]}", NumStyle.HexNumber);

                color = new Color(rChannel, gChannel, bChannel, aChannel);
                return;
            }

            color = Color.Magenta;
        }
    }
}
