using static System.Math;
public class MyColor
{
    public MyColor(byte red, byte green, byte blue, byte alpha = 0)
    {
        R = red;
        G = green;
        B = blue;
        A = alpha;
    }
    public MyColor ToHsva()
    {
        var result = new MyColor(0, 0, 0);
        float r = R / 255;
        float g = G / 255;
        float b = B / 255;
        byte a = A;
        var max = Max(Max(r, g), b);
        var min = Min(Min(r, g), b);
        if (max == r)
        {
            R = (byte)(60 * (g - b) / (max - min)
            + g < b ? 360 : 0);
        }
        else if (max == g)
        {
            R = (byte)(60 * (b - r) / (max - min)
            + 120);
        }
        else if (max == b)
        {
            R = (byte)(60 * (r - g) / (max - min) + 240);
        }
        result.R = (byte)(max == 0 ? 0 : 1 - min / max);
        B = (byte)max;
        A = a;
        return result;
    }
    public byte R;
    public byte G;
    public byte B;
    public byte A;
}
public class HsvaColor
{
    public HsvaColor(byte hue, byte saturation, byte value, byte alpha)
    {

    }
    public HsvaColor(MyColor color)
    {
        Hue = color.R;
        Saturation = color.G;
        Value = color.B;
        Alpha = color.A;
    }
    public byte Hue;
    public byte Saturation;
    public byte Value;
    public byte Alpha;
    public MyColor ToRgba()
    {
        var result = new MyColor(0, 0, 0);
        byte hue;
        byte saturation;
        byte value;
        byte alpha;
        var s = Saturation * 100;
        var v = Value * 100;
        var hi = (int)Floor((double)(Hue / 60)) % 6;
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
        hue = ComponentValue(0);
        value = ComponentValue(1);
        saturation = ComponentValue(2);
        alpha = Alpha;
        return result;
    }
}
public static class ColorOptions
{
    public static MyColor BlueFilter(MyColor color)
    {
        var result = color;
        result.B = 0;
        return result;
    }
}