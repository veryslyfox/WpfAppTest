using static System.Math;
namespace System.Graphics.Colors
{
    class Color
    {
        public Color(byte red, byte green, byte blue, byte alpha = 0)
        {
            float r = red / 255;
            float g = green / 255;
            float b = blue / 255;
            var max = Max(Max(r, g), b);
            var min = Min(Min(r, g), b);
            if (max == r)
            {
                Hue = 60 * (g - b) / (max - min)
                + g < b ? 360 : 0;
            }
            else if (max == g)
            {
                Hue = 60 * (b - r) / (max - min)
                + 120;
            }
            else if (max == b)
            {
                Hue = 60 * (r - g) / (max - min) + 240;
            }
            Saturation = max == 0 ? 0 : 1 - min / max;
            Value = max;
            Alpha = alpha;
        }
        public Color ToRgba()
        {
            var result = new Color(0, 0, 0);
            byte red;
            byte green;
            byte blue;
            byte alpha;
            var s = Saturation * 100;
            var v = Value * 100;
            var hi = (int)Floor(Hue / 60) % 6;
            var vmin = (100 - s) * v / 100;
            var a = (v - vmin) * (Hue % 60) / 60;
            var vinc = vmin + a;
            var vdec = v - a;
            var indexArray = new int[] { 0, 3, 1, 1, 2, 0, 2, 0, 0, 3, 1, 1, 1, 1, 2, 0, 0, 3 };
#pragma warning disable
            byte ComponentValue(int component)
            {
                var array = new float[] { v, vmin, vinc, vdec };

                return (byte)(array[indexArray[hi + 6 * component]] * 255 / 100);
            }
#pragma warning restore
            red = ComponentValue(0);
            blue = ComponentValue(1);
            green = ComponentValue(2);
            alpha = Alpha;
            return result;
        }
        public float Hue;
        public float Saturation;
        public float Value;
        public byte Alpha;
    }
}