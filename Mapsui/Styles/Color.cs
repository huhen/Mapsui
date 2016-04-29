using System;
using System.Linq;

namespace Mapsui.Styles
{
    public class Color
    {
        private uint _color;

        public Color()
        {
            A = 255;
        }

        public Color(Color color)
        {
            _color = color.ToArgb;
            //A = color.A;
            //R = color.R;
            //G = color.G;
            //B = color.B;
        }

        public uint ToArgb => _color;

        //public uint ToArgb()
        //{
        //    return ((uint)A << 24) | ((uint)B << 16) | ((uint)G << 8) | (uint)R;
        //}

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color { A = a, R = r, G = g, B = b };
        }

        public byte A
        {
            get { return (byte)(_color >> 24); }
            set
            {
                _color &= 0x00FFFFFF;
                _color |= (uint)value << 24;
            }
        }

        public byte B
        {
            get { return (byte)(_color >> 16); }
            set
            {
                _color &= 0xFF00FFFF;
                _color |= (uint)value << 16;
            }
        }

        public byte G
        {
            get { return (byte)(_color>>8); }
            set
            {
                _color &= 0xFFFF00FF;
                _color |= (uint)value <<8;
            }
        }

        public byte R
        {
            get { return (byte)_color; }
            set
            {
                _color &= 0xFFFFFF00;
                _color |= value;
            }
        }


        //public int R { get; set; }
        //public int G { get; set; }
        //public int B { get; set; }

        public static Color Black { get { return new Color { A = 255, R = 0, G = 0, B = 0 }; } }
        public static Color White { get { return new Color { A = 255, R = 255, G = 255, B = 255 }; } }
        public static Color Gray { get { return new Color { A = 255, R = 128, G = 128, B = 128 }; } }

        public static Color Red { get { return new Color { A = 255, R = 255, G = 0, B = 0 }; } }
        public static Color Yellow { get { return new Color { A = 255, R = 255, G = 255, B = 0 }; } }
        public static Color Green { get { return new Color { A = 255, R = 0, G = 128, B = 0 }; } }
        public static Color Cyan { get { return new Color { A = 255, R = 0, G = 255, B = 255 }; } }
        public static Color Blue { get { return new Color { A = 255, R = 0, G = 0, B = 255 }; } }

        public static Color Orange { get { return new Color { A = 255, R = 255, G = 165, B = 0 }; } }
        public static Color Indigo { get { return new Color { A = 255, R = 75, G = 0, B = 130 }; } }
        public static Color Violet { get { return new Color { A = 255, R = 238, G = 130, B = 238 }; } }


        public override bool Equals(object obj)
        {
            if (!(obj is Color))
            {
                return false;
            }
            return Equals((Color)obj);
        }

        public bool Equals(Color color)
        {
            if (A != color.A) return false;
            if (R != color.R) return false;
            if (G != color.G) return false;
            if (B != color.B) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        public static bool operator ==(Color color1, Color color2)
        {
            return Equals(color1, color2);
        }

        public static bool operator !=(Color color1, Color color2)
        {
            return !Equals(color1, color2);
        }

    }
}
