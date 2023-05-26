using System.IO;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Input;
using Button = Objects.Button;
using Objects;
using Objects.SpecialMath;
using Objects.Data;
using Style = Objects.Data.Style;
using static System.Math;
using static System.Numerics.Complex;
using System.Numerics;
using Objects.VolumeObjects;
using System.Threading;
using Vector = System.Windows.Vector;
using System.Windows.Controls;
using System.Collections;

namespace WpfApp1;

public partial class MainWindow : Window
{
    private Color[] Colors = new Color[] { Color.FromRgb(0, 0, 0), Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255), Color.FromRgb(255, 255, 0), Color.FromRgb(255, 0, 255), Color.FromRgb(0, 255, 255), Color.FromRgb(0, 0, 0), Color.FromRgb(255, 255, 255), Color.FromRgb(127, 255, 127) };
    private readonly DispatcherTimer _timer = new();
    private readonly WriteableBitmap _bitmap;
    private readonly Random _rng = new();
    private static readonly double TimeStep = 1.0 / Stopwatch.Frequency;
    private bool[,] _space = new bool[40, 40];
    private int[] _ticks = new int[] { 128, 64, 32, 16, 8, 4, 2, 1 };
    private Point[] _points = new Point[] { new Point(400, 400) };
    private int _size = 100;
    Roller R1 = new Roller(PI / 3);
    Roller R2 = new Roller(-PI / 3);
    private int _f = 0;
    Dictionary<Direct, int> DirectX = new Dictionary<Direct, int>
    {
        {Direct.L, -1},
        {Direct.U, 0},
        {Direct.R, 1},
        {Direct.D, 0},
    };
    Dictionary<Direct, int> DirectY = new Dictionary<Direct, int>
    {
        {Direct.L, 0},
        {Direct.U, 1},
        {Direct.R, 0},
        {Direct.D, -1},
    };
    int[] Roll = new int[] { 1, 0, 1 };
    public int _sorted;
    public int _step = 6399;
    private bool _clicked;
    private ConvolutionNeuralNetwork _network;
    private List<Matrix> _pluses = new List<Matrix> { };
    private List<Matrix> _minuses = new List<Matrix> { };
    enum Direct
    {
        L,
        U,
        R,
        D,
    }
    public MainWindow()
    {
        _network = ConvolutionNeuralNetwork.GetRandomNetwork(0, 1, "6*6, 6*6, 6*6", 0.001, 10);
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.000001);
        _timer.Tick += Tick;
        _timer.Start();
        MouseLeftButtonDown += ClickHandler;
        MouseLeftButtonUp += UpHandler;
        KeyDown += KeyHandler;
    }

    private void KeyHandler(object sender, KeyEventArgs args)
    {
        var key = args.Key;
        switch (key)
        {
            case Key.P:
                {
                    Matrix matrix = Matrix.Generate(40, 40);
                    for (int y = 0; y < 40; y++)
                    {
                        for (int x = 0; x < 40; x++)
                        {
                            matrix[x, y] = _space[x, y] ? 0 : 1;
                        }
                    }
                    _pluses.Add(matrix);
                    break;
                }
            case Key.M:
                {
                    Matrix matrix = Matrix.Generate(40, 40);
                    for (int y = 0; y < 40; y++)
                    {
                        for (int x = 0; x < 40; x++)
                        {
                            matrix[x, y] = _space[x, y] ? 0 : 1;
                        }
                    }
                    _minuses.Add(matrix);
                    break;
                }
            case Key.C:
                {
                    _space = new bool[40, 40];
                    break;
                }
            case Key.S:
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        _network.CorrectAll(Error);
                    }
                    _network.Write("Weights", FileMode.OpenOrCreate);
                }
                break;
        }
    }
    double Error(ConvolutionNeuralNetwork network)
    {
        var sum = 0.0;
        foreach (var item in _pluses)
        {
            var value = network.Propagate(item)[0, 0];
            sum += Abs(Round(value * 2) / 2);
        }
        foreach (var item in _minuses)
        {
            var value = network.Propagate(item)[0, 0];
            sum += Abs(Round(value * 2) / 2 - 1);
        }
        return sum;
    }
    double F(double val)
    {
        if (val < 2)
        {
            return val;
        }
        else
        {
            return val * F(val - 1);
        }
    }

    private void Tick(object? sender, EventArgs e)
    {
        if (_clicked)
        {
            var x = (int)(Mouse.GetPosition(this).X / 20);
            var y = (int)(Mouse.GetPosition(this).Y / 20);
            if (x is < 40 and > 0 && y is < 40 and > 0)
            {
                _space[x, y] = true;
            }
        }
        _bitmap.Lock();
        for (int y = 0; y < _bitmap.PixelHeight; y++)
        {
            for (int x = 0; x < _bitmap.PixelWidth; x++)
            {
                var c = _space[x / 20, y / 20] ? 255 : 0;
                var color = FromRgb(c, c, c);
                var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                unsafe
                {
                    *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                }
            }
        }
        _step++;
        _f++;
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelHeight, _bitmap.PixelHeight));
        _bitmap.Unlock();
    }
    private void ClickHandler(object sender, MouseEventArgs args)
    {
        _clicked = true;
    }
    private void UpHandler(object sender, MouseEventArgs args)
    {
        _clicked = false;
    }
    private Color FromRgbSaturated(int r, int g, int b)
    {
        if (r < 0)
        {
            r = 0;
        }
        else if (r > 255)
        {
            r = 255;
        }

        if (g < 0)
        {
            g = 0;
        }
        else if (g > 255)
        {
            g = 255;
        }

        if (b < 0)
        {
            b = 0;
        }
        else if (b > 255)
        {
            b = 255;
        }

        return Color.FromRgb((byte)r, (byte)g, (byte)b);
    }
    private Color FromRgb(int r, int g, int b)
    {
        return Color.FromRgb(((byte)(r & 255)), ((byte)(g & 255)), ((byte)(b & 255)));
    }
    public Color Normalize(Color color, byte lightness)
    {
        var r = color.R;
        var g = color.G;
        var b = color.B;
        var max = Math.Max(r, Math.Max(g, b));
        if (max == 0)
        {
            return Color.FromRgb(lightness, lightness, lightness);
        }
        var normalizer = (double)lightness / max;
        return FromRgb((int)(r * normalizer), ((int)(g * normalizer)), ((int)(b * normalizer)));
    }
    public Color Interpolation(Color a, Color b, byte c)
    {
        return FromRgb((a.R * c + b.R * (255 - c)) / 255, (a.G * c + b.G * (255 - c)) / 255, (a.B * c + b.B * (255 - c)) / 255);
    }
    public Color HsvToRgb(int h, byte s, byte v)
    {
        var result = new Color();
        var hue = h % 360;
        var hv = (hue % 60) * 255 / 60;
        var a = 0;
        var b = hv;
        var c = 255 - hv;
        var d = 255;
        void DoubleInterval(int min, int max, int r, int g, int b)
        {
            if (min <= hue && max > hue)
            {
                result = Normalize(FromRgb(r, g, b), v);
            }
        }
        DoubleInterval(0, 60, d, b, a);
        DoubleInterval(60, 120, c, d, a);
        DoubleInterval(120, 180, a, d, b);
        DoubleInterval(180, 240, a, c, d);
        DoubleInterval(240, 300, b, a, d);
        DoubleInterval(300, 360, d, a, c);
        return Interpolation(result, Color.FromRgb(v, v, v), s);
    }
}
public class Matrix
{
    public Matrix(Vector[] vectors)
    {
        Vectors = vectors;
        X = vectors[0].Values.Length;
        Y = vectors.Length;
    }
    public double[] ToArray()
    {
        return Vectors.SelectMany(v => v.Values).ToArray();
    }
    public static Matrix Create(params Vector[] vectors)
    {
        return new(vectors);
    }
    public static Vector operator *(Vector vector, Matrix matrix)
    {
        return new(matrix.Vectors.Select(v => v * vector).ToArray());
    }
    public static Matrix operator +(Matrix a, Matrix b)
    {
        return new(a.Vectors.Zip(b.Vectors, (p, q) => p + q).ToArray());
    }
    public static Matrix operator *(Matrix a, double b)
    {
        return new(a.Vectors.Select(k => k * b).ToArray());
    }
    public static Matrix operator -(Matrix value)
    {
        return new(value.Vectors.Select(z => -z).ToArray());
    }
    public Vector[] Vectors { get; }
    public void Write()
    {
        foreach (var item in Vectors)
        {
            item.Write();
        }
    }
    public static Matrix Generate(int x, int y, double d = 0)
    {
        var array = new double[x];
        Array.Fill(array, d);
        var vector = new Vector(array);
        var vectors = new Vector[y];
        for (int i = 0; i < y; i++)
        {
            array = array.ToArray();
            vector = new(array);
            vectors[i] = vector;
        }
        return new(vectors);
    }
    public static Random Random = new Random();
    public double this[int x, int y]
    {
        get => Vectors[y].Values[x];
        set => Vectors[y].Values[x] = value;
    }
    public int X { get; set; }
    public int Y { get; set; }
}
public class Vector
{
    public Vector(double[] values)
    {
        Values = values;
    }
    public static Vector Create(params double[] values)
    {
        return new(values);
    }
    public double[] Values { get; set; }
    public static double operator *(Vector a, Vector b)
    {
        return a.Values.Zip(b.Values, (p, q) => p * q).Sum();
    }
    public static Vector operator +(Vector a, Vector b)
    {
        return new(a.Values.Zip(b.Values, (p, q) => p + q).ToArray());
    }
    public static Vector operator *(Vector a, double b)
    {
        return new(a.Values.Select(k => k * b).ToArray());
    }
    public static Vector operator -(Vector value)
    {
        return new(value.Values.Select(z => -z).ToArray());
    }
    public void Write()
    {
        foreach (var item in Values)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine();
    }
    public Vector Relu()
    {
        return new(Values.Select(x => x > 0 ? x : 0).ToArray());
    }
    public double Sum()
    {
        return Values.Sum();
    }
}
public class NeuralNetwork
{
    public NeuralNetwork(Matrix[] matrices, double delta = 0.01, double alpha = 1)
    {
        Matrices = matrices;
        Delta = delta;
        Alpha = alpha;
    }

    public Matrix[] Matrices { get; set; }
    public double Delta { get; set; }
    public double Alpha { get; private set; }
    public static readonly Random _rng = new Random();
    public Vector Propagate(Vector data)
    {
        Vector result = data;
        foreach (var item in Matrices)
        {
            data = (data * item).Relu();
        }
        return data;
    }


    public static NeuralNetwork GetRandomNetwork(int min, int max, string arch, double delta, double alpha)
    {
        var neurons = arch.Split(',').Select(k => int.Parse(k)).ToArray();
        Matrix[] result = new Matrix[neurons.Length - 1];
        for (int i = 0; i < neurons.Length - 1; i++)
        {
            result[i] = Evolution.GetRandomMatrix(neurons[i], neurons[i + 1], min, max);
        }
        return new(result, delta, alpha);
    }
    public void Write()
    {
        foreach (var item in Matrices)
        {
            item.Write();
        }
    }

    public Matrix this[int layer]
    {
        get => Matrices[layer];
        set => Matrices[layer] = value;
    }
    public double this[int x, int y, int layer]
    {
        get => Matrices[layer][x, y];
        set => Matrices[layer][x, y] = value;
    }
    public void CorrectAll(Func<NeuralNetwork, double> func)
    {
        for (int layer = 0; layer < Matrices.Length; layer++)
        {
            for (int y = 0; y < Matrices[layer].Y; y++)
            {
                for (int x = 0; x < Matrices[layer].X; x++)
                {
                    Correct(x, y, layer, func);
                }
            }
        }
    }
    public void Correct(int x, int y, int layer, Func<NeuralNetwork, double> func)
    {
        var fx = func(this);
        this[x, y, layer] += Delta;
        var fxd = func(this);
        this[x, y, layer] -= Delta;
        this[x, y, layer] += (fx - fxd) * Alpha;
        var fxr = func(this);
        if (fxr > fx)
        {
            Alpha /= 2;
        }
    }
    public void Write(string fileName, FileMode mode)
    {
        var file = File.Open(fileName, mode);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(Delta);
        writer.Write(Alpha);
        writer.Write(Matrices.Length);
        writer.Write(Matrices[0].X);
        for (int layer = 0; layer < Matrices.Length; layer++)
        {
            writer.Write(Matrices[layer].Y);
            for (int y = 0; y < Matrices[layer].Y; y++)
            {
                for (int x = 0; x < Matrices[layer].X; x++)
                {
                    writer.Write(this[x, y, layer]);
                }
            }
        }
        file.Close();
    }
    static public NeuralNetwork Read(string fileName, FileMode mode)
    {
        var file = File.Open(fileName, mode);
        BinaryReader reader = new BinaryReader(file);
        var delta = reader.ReadDouble();
        var alpha = reader.ReadDouble();
        var layerCount = reader.ReadInt32();
        var width = reader.ReadInt32();
        int height;
        var result = new NeuralNetwork(new Matrix[layerCount], delta, alpha);
        for (int layer = 0; layer < layerCount; layer++)
        {
            height = reader.ReadInt32();
            result.Matrices[layer] = Matrix.Generate(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[x, y, layer] = reader.ReadDouble();
                }
            }
            width = height;
        }
        file.Close();
        return result;
    }
}
static class Activate
{
    public static readonly Func<double, double> Relu = x => x < 0 ? 0 : x;
    public static readonly Func<double, double> Sigmoide = x => 1 / (1 + Exp(-x));

}
class ConvolutionNeuralNetwork
{
    public ConvolutionNeuralNetwork(Matrix[] matrices, double delta = 0.01, double alpha = 1)
    {
        Matrices = matrices;
        Delta = delta;
        Alpha = alpha;
    }

    public Matrix[] Matrices { get; set; }
    public double Delta { get; set; }
    public double Alpha { get; private set; }
    public static readonly Random _rng = new Random();
    public Matrix Propagate(Matrix image)
    {
        var result = Matrix.Generate(image.X, image.Y, 0);
        for (int i = 0; i < Matrices.Length; i++)
        {
            result = Convolution(result, Matrices[i], Activate.Relu);
        }
        return result;
    }

    public static Matrix Convolution(Matrix image, Matrix core, Func<double, double> func)
    {
        var result = Matrix.Generate(image.X - core.X + 1, image.Y - core.Y + 1);
        for (int y = 0; y < result.Y; y++)
        {
            for (int x = 0; x < result.X; x++)
            {
                var sum = 0.0;
                for (int row = 0; row < core.Y; row++)
                {
                    for (int column = 0; column < core.X; column++)
                    {
                        sum += func(core[column, row] * image[x + column, y + row]);
                    }
                }
                result[x, y] = sum;
            }
        }
        return result;
    }
    public static ConvolutionNeuralNetwork GetRandomNetwork(int min, int max, string arch, double delta, double alpha)
    {
        var neurons = arch.Split(',', '*').Select(k => int.Parse(k)).ToArray();
        Matrix[] result = new Matrix[neurons.Length - 1];
        for (int i = 0; i < neurons.Length / 2; i++)
        {
            result[i] = Evolution.GetRandomMatrix(neurons[2 * i], neurons[2 * i + 1], min, max);
        }
        return new(result, delta, alpha);
    }
    public void Write()
    {
        foreach (var item in Matrices)
        {
            item.Write();
        }
    }

    public Matrix this[int layer]
    {
        get => Matrices[layer];
        set => Matrices[layer] = value;
    }
    public double this[int x, int y, int layer]
    {
        get => Matrices[layer][x, y];
        set => Matrices[layer][x, y] = value;
    }
    public void CorrectAll(Func<ConvolutionNeuralNetwork, double> func)
    {
        for (int layer = 0; layer < Matrices.Length; layer++)
        {
            for (int y = 0; y < Matrices[layer].Y; y++)
            {
                for (int x = 0; x < Matrices[layer].X; x++)
                {
                    Correct(x, y, layer, func);
                }
            }
        }
    }
    public void Correct(int x, int y, int layer, Func<ConvolutionNeuralNetwork, double> func)
    {
        var fx = func(this);
        this[x, y, layer] += Delta;
        var fxd = func(this);
        this[x, y, layer] -= Delta;
        this[x, y, layer] += (fx - fxd) * Alpha;
        var fxr = func(this);
        if (fxr > fx)
        {
            Alpha /= 2;
        }
    }
    public void Write(string fileName, FileMode mode)
    {
        var file = File.Open(fileName, mode);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(Delta);
        writer.Write(Alpha);
        writer.Write(Matrices.Length);
        writer.Write(Matrices[0].X);
        for (int layer = 0; layer < Matrices.Length; layer++)
        {
            writer.Write(Matrices[layer].Y);
            for (int y = 0; y < Matrices[layer].Y; y++)
            {
                for (int x = 0; x < Matrices[layer].X; x++)
                {
                    writer.Write(this[x, y, layer]);
                }
            }
        }
        file.Close();
    }
    static public ConvolutionNeuralNetwork Read(string fileName, FileMode mode)
    {
        var file = File.Open(fileName, mode);
        BinaryReader reader = new BinaryReader(file);
        var delta = reader.ReadDouble();
        var alpha = reader.ReadDouble();
        var layerCount = reader.ReadInt32();
        var width = reader.ReadInt32();
        int height;
        var result = new ConvolutionNeuralNetwork(new Matrix[layerCount], delta, alpha);
        for (int layer = 0; layer < layerCount; layer++)
        {
            height = reader.ReadInt32();
            result.Matrices[layer] = Matrix.Generate(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[x, y, layer] = reader.ReadDouble();
                }
            }
            width = height;
        }
        file.Close();
        return result;
    }
}
public class Evolution
{
    static Random Random { get; } = new Random();
    public static Matrix GetRandomMatrix(int x, int y, double min, double max)
    {
        var matrix = Matrix.Generate(x, y);
        for (int row = 0; row < y; row++)
        {
            for (int column = 0; column < x; column++)
            {
                matrix[column, row] = Random.NextDouble() * (max - min) + min;
            }
        }
        return matrix;
    }
}
class OscillateDot
{
    public OscillateDot(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public void Next(int[,] proj)
    {
        proj[(int)(X + 400), (int)(Abs(Y + 400))] = 127;
        X += (Y + Sqrt(X * X + Y * Y) * 0.99) * 0.01;
        Y -= X * 0.01;
        proj[(int)(Abs(X + 400)), (int)(Abs(Y + 400))] = 255;
    }
}
class LorenzDot
{
    public LorenzDot(double x, double y, double z, double dt = 0.001)
    {
        X = x;
        Y = y;
        Z = z;
        Dt = dt;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double Dt { get; }

    public void Next(int[,] proj)
    {
        //proj[(int)(X + 400), (int)(Abs(Y + 400))] = 0;
        X += 100 * (Y - X) * Dt;
        Y += (X * (28 - Z) - Y) * Dt;
        Z += (X * Y + Z * 8 / 3) * Dt;
        proj[(int)(Abs(X + 400)), (int)(Abs(Y + 400))] = 255;
    }
}