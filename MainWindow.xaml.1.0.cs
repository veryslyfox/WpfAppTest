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
using Vector = Objects.Vector;
using static System.Math;
using static System.Numerics.Complex;
using System.Numerics;
using Objects.VolumeObjects;
using System.Threading;

namespace WpfApp1;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new();
    private readonly WriteableBitmap _bitmap;
    private readonly Random _rng = new();
    private static readonly double TimeStep = 1.0 / Stopwatch.Frequency;
    private double _time;
    private List<Button> _buttons = new List<Button>();
    private double _f;
    private Roller[] _rollers = new Roller[] { new Roller(0), new Roller(2 * Math.PI / 3), new Roller(4 * Math.PI / 3) };
    private Color[] _labs = new Color[] { Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255) };
    private Color? _mouseColor;
    private Bitmap _map = new Bitmap(new Style(800, 800, Color.FromRgb(255, 255, 255)), 800, 800);
    private int[,] _field = new int[50, 50];
    private int[,] _norm = new int[800, 800];
    private double _m;
    private int _max;
    private int _centerX = 400;
    private Point _point;
    Triangle _triangle;
    public double _scale = 7;
    public double _x = 400;
    public double _y = 400;
    private Complex _a = -1;
    private Complex _b = 1;
    private ElementaryCA _CA;
    private NeuralNetwork _network;
    public MainWindow()
    {
        _network = NeuralNetwork.Random(0, 1, "2,8,8,3");
        _triangle.Sign = 1;
        _CA = new ElementaryCA(1, 800, 800, true);
        _CA.Memory[0][400] = 1;
        Point3 NextPoint3()
        {
            return new Point3(_rng.Next(0, 100), _rng.Next(0, 100), _rng.Next(0, 100));
        }
        Triangle NextTriangle()
        {
            return new Triangle(NextPoint3(), NextPoint3(), NextPoint3());
        }
        _triangle = NextTriangle();
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.000001);
        _f = 1;
        _timer.Tick += Tick;
        _timer.Start();
        MouseLeftButtonDown += ButtonHandler;
    }


    private void Tick(object? sender, EventArgs e)
    {
        Thread.Sleep(1000);
        _network = NeuralNetwork.Random(-1, 1, "2,3");
        _bitmap.Lock();
        for (int y = 0; y < _bitmap.PixelHeight; y++)
        {
            for (int x = 0; x < _bitmap.PixelWidth; x++)
            {
                var c = _network.Propagation(Vector.Create(x, y));
                Color color = FromRgb((int)(c.Values[0] * 255), (int)(c.Values[1] * 255), (int)(c.Values[2] * 255));
                var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                unsafe
                {
                    *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                }
            }
        }
        _f += 0.1;
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelHeight, _bitmap.PixelHeight));
        _bitmap.Unlock();
    }
    private void ButtonHandler(object sender, MouseEventArgs args)
    {
        var point = args.GetPosition(this);
        _x = point.X * _scale;
        _y = point.Y * _scale;
        _scale *= 2;
        _m++;
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
class Sand
{
    public Sand(int[,] ints)
    {
        Values = ints;
        Width = ints.GetLength(1);
        Height = ints.GetLength(0);
    }

    public int[,] Values { get; }
    public int Width { get; set; }
    public int Height { get; set; }

    public void Next()
    {
        for (int column = 0; column < Height - 1; column++)
        {
            for (int row = 0; row < Width - 1; row++)
            {
                if (Values[column, row] > 3)
                {
                    Values[column, row] -= 4;
                    Values[column + 1, row]++;
                    Values[column - 1, row]++;
                    Values[column, row - 1]++;
                    Values[column, row + 1]++;
                }
            }
        }
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
    public static Matrix GenerateBin(int x, int y)
    {
        var matrix = Generate(x, y, 0);
        for (int row = 0; row < y; row++)
        {
            for (int column = 0; column < x; column++)
            {
                matrix[column, row] = Random.Next(2);
            }
        }
        return matrix;
    }
    public double this[int x, int y]
    {
        get => Vectors[y].Values[x];
        set => Vectors[y].Values[x] = value;
    }
    public static Matrix Dot(int x, int y)
    {
        var result = Generate(x, y);
        result[Random.Next(x), Random.Next(y)] = 1;
        return result;
    }
    public int X { get; }
    public int Y { get; }
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
    public double[] Values { get; }
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
        return new(Values.Select(x => x < 0 ? 0 : x).ToArray());
    }
}
public class NeuralNetwork
{
    public NeuralNetwork(Matrix[] matrices)
    {
        Random random = new Random();
        Matrices = matrices;
        DMatrix = matrices[matrices.Length - 1];
    }

    public Matrix[] Matrices { get; }
    public Matrix DMatrix { get; set; }
    public int Y { get; private set; }
    public int X { get; private set; }
    public bool LowerDot { get; private set; }

    public Vector Propagation(Vector data)
    {
        Vector result = data;
        foreach (var item in Matrices)
        {
            data = (data * item).Relu();
        }
        return data;
    }


    public static NeuralNetwork Random(int min, int max, string arch)
    {
        var neurons = arch.Split(',').Select(k => int.Parse(k)).ToArray();
        Matrix[] result = new Matrix[neurons.Length - 1];
        for (int i = 0; i < neurons.Length - 1; i++)
        {
            result[i] = Evolution.GetRandomMatrix(neurons[i], neurons[i + 1], min, max);
        }
        return new(result);
    }
}

public class NeuralNetworkPair
{
    public NeuralNetworkPair()
    {

    }
}

public interface IResultMetrics<T>
{
    double Error(T data);
}
public class Metrics : IResultMetrics<Vector>
{
    public Metrics(Vector vector)
    {
        Vector = vector;
    }

    public Vector Vector { get; }

    public double Error(Vector vector)
    {
        return ((Vector + vector * -1) * (Vector + vector * -1));
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
    public static Matrix Mutation(double min, double max, Matrix matrix)
    {
        return matrix + GetRandomMatrix(matrix.X, matrix.Y, min, max);
    }
}