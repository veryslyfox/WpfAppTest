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
    private bool[,] _space = new bool[800, 800];
    private int[] _ticks = new int[] { 128, 64, 32, 16, 8, 4, 2, 1 };
    private HashSet<Particle2> _particles = new HashSet<Particle2> { new(400, 400, 1, 0) };
    Roller R1 = new Roller(PI / 3);
    Roller R2 = new Roller(-PI / 3);
    public double _f = 0;
    public MainWindow()
    {
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.000001);
        _timer.Tick += Tick;
        _timer.Start();
        MouseLeftButtonDown += ClickHandler;
        MouseLeftButtonUp += UpClickHandler;
    }

    private void UpClickHandler(object sender, MouseButtonEventArgs e)
    {
    }

    private void Tick(object? sender, EventArgs e)
    {
        foreach (var item in _ticks)
        {
            for (int i = 0; i < item; i++)
            {
                var j = 0;
                foreach (var particle in _particles)
                {
                    particle.Next();
                    j++;
                    _space[(int)particle.X, (int)particle.Y] = ((j / 2) * 2 - j) == 0;
                }
            }
            _particles = _particles.SelectMany(p => new Particle2[] { p * R1.Matrix, p * R2.Matrix }).ToHashSet();
        }
        Thread.Sleep(1000);
        _bitmap.Lock();
        for (int y = 0; y < _bitmap.PixelHeight; y++)
        {
            for (int x = 0; x < _bitmap.PixelWidth; x++)
            {
                Color color = _space[x, y] ? FromRgb(255, 255, 255) : FromRgb(0, 0, 0);
                var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                unsafe
                {
                    *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                }
            }
        }
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelHeight, _bitmap.PixelHeight));
        _bitmap.Unlock();
        _f += 0.1;
        _particles = new HashSet<Particle2> { new(400, 400, 1, 0) };
    }
    private void ClickHandler(object sender, MouseEventArgs args)
    {
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
// public class Matrix
// {
//     public Matrix(Vector[] vectors)
//     {
//         Vectors = vectors;
//         X = vectors[0].Values.Length;
//         Y = vectors.Length;
//     }
//     public static Matrix Create(params Vector[] vectors)
//     {
//         return new(vectors);
//     }
//     public static Vector operator *(Vector vector, Matrix matrix)
//     {
//         return new(matrix.Vectors.Select(v => v * vector).ToArray());
//     }
//     public static Matrix operator +(Matrix a, Matrix b)
//     {
//         return new(a.Vectors.Zip(b.Vectors, (p, q) => p + q).ToArray());
//     }
//     public static Matrix operator *(Matrix a, double b)
//     {
//         return new(a.Vectors.Select(k => k * b).ToArray());
//     }
//     public static Matrix operator -(Matrix value)
//     {
//         return new(value.Vectors.Select(z => -z).ToArray());
//     }
//     public Vector[] Vectors { get; }
//     public void Write()
//     {
//         foreach (var item in Vectors)
//         {
//             item.Write();
//         }
//     }
//     public static Matrix Generate(int x, int y, double d = 0)
//     {
//         var array = new double[x];
//         Array.Fill(array, d);
//         var vector = new Vector(array);
//         var vectors = new Vector[y];
//         for (int i = 0; i < y; i++)
//         {
//             array = array.ToArray();
//             vector = new(array);
//             vectors[i] = vector;
//         }
//         return new(vectors);
//     }
//     public static Random Random = new Random();
//     public static Matrix GenerateBin(int x, int y)
//     {
//         var matrix = Generate(x, y, 0);
//         for (int row = 0; row < y; row++)
//         {
//             for (int column = 0; column < x; column++)
//             {
//                 matrix[column, row] = Random.Next(2);
//             }
//         }
//         return matrix;
//     }
//     public double this[int x, int y]
//     {
//         get => Vectors[y].Values[x];
//         set => Vectors[y].Values[x] = value;
//     }
//     public static Matrix Dot(int x, int y)
//     {
//         var result = Generate(x, y);
//         result[Random.Next(x), Random.Next(y)] = 1;
//         return result;
//     }
//     public int X { get; }
//     public int Y { get; }
// }
// // public class Vector
// // {
// //     public Vector(double[] values)
// //     {
// //         Values = values;
// //     }
// //     public static Vector Create(params double[] values)
// //     {
// //         return new(values);
// //     }
// //     public double[] Values { get; }
// //     public static double operator *(Vector a, Vector b)
// //     {
// //         return a.Values.Zip(b.Values, (p, q) => p * q).Sum();
// //     }
// //     public static Vector operator +(Vector a, Vector b)
// //     {
// //         return new(a.Values.Zip(b.Values, (p, q) => p + q).ToArray());
// //     }
// //     public static Vector operator *(Vector a, double b)
// //     {
// //         return new(a.Values.Select(k => k * b).ToArray());
// //     }
// //     public static Vector operator -(Vector value)
// //     {
// //         return new(value.Values.Select(z => -z).ToArray());
// //     }
// //     public void Write()
// //     {
// //         foreach (var item in Values)
// //         {
// //             Console.Write(item + " ");
// //         }
// //         Console.WriteLine();
// //     }
// //     public Vector Active()
// //     {
// //         return new(Values.Select(x => x * x).ToArray());
// //     }
// // }
// // public class NeuralNetwork
// // {
// //     public NeuralNetwork(Matrix[] matrices, double delta = 0.01)
// //     {
// //         Matrices = matrices;
// //         Delta = delta;
// //     }

// //     public Matrix[] Matrices { get; }
// //     public double Delta { get; set; }
// //     public bool LowerDot { get; private set; }

// //     public Vector Propagate(Vector data)
// //     {
// //         Vector result = data;
// //         foreach (var item in Matrices)
// //         {
// //             data = (data * item).Active();
// //         }
// //         return data;
// //     }


// //     public static NeuralNetwork GetRandomNetwork(int min, int max, string arch, double delta = 0.01)
// //     {
// //         var neurons = arch.Split(',').Select(k => int.Parse(k)).ToArray();
// //         Matrix[] result = new Matrix[neurons.Length - 1];
// //         for (int i = 0; i < neurons.Length - 1; i++)
// //         {
// //             result[i] = Evolution.GetRandomMatrix(neurons[i], neurons[i + 1], min, max);
// //         }
// //         return new(result, delta);
// //     }
// //     public void Write()
// //     {
// //         foreach (var item in Matrices)
// //         {
// //             item.Write();
// //         }
// //     }
// //     public void BackPropagate(int layer, int x, int y, Vector input, Vector output, double speed)
// //     {
// //         var err = Propagate(input) * output;
// //         this[x, y, layer] += Delta;
// //         var err2 = Propagate(input) * output;
// //         this[x, y, layer] += (err2 - err) / Delta * speed - Delta;
// //     }
// //     public Matrix this[int layer]
// //     {
// //         get => Matrices[layer];
// //         set => Matrices[layer] = value;
// //     }
// //     public double this[int x, int y, int layer]
// //     {
// //         get => Matrices[layer][x, y];
// //         set => Matrices[layer][x, y] = value;
// //     }
// // }


// // public class NeuralNetworkPair
// // {
// //     public NeuralNetworkPair(NeuralNetwork a, NeuralNetwork b)
// //     {
// //         A = a;
// //         B = b;
// //     }

// //     public NeuralNetwork A { get; }
// //     public NeuralNetwork B { get; }
// // }

// // public interface IResultMetrics<T>
// // {
// //     double Error(T data);
// // }
// // public class Metrics : IResultMetrics<Vector>
// // {
// //     public Metrics(Vector vector)
// //     {
// //         Vector = vector;
// //     }

// //     public Vector Vector { get; }

// //     public double Error(Vector vector)
// //     {
// //         return ((Vector + vector * -1) * (Vector + vector * -1));
// //     }
// // }
// // public class Evolution
// // {
// //     static Random Random { get; } = new Random();
// //     public static Matrix GetRandomMatrix(int x, int y, double min, double max)
// //     {
// //         var matrix = Matrix.Generate(x, y);
// //         for (int row = 0; row < y; row++)
// //         {
// //             for (int column = 0; column < x; column++)
// //             {
// //                 matrix[column, row] = Random.NextDouble() * (max - min) + min;
// //             }
// //         }
// //         return matrix;
// //     }
// //     public static Matrix Mutation(double min, double max, Matrix matrix)
// //     {
// //         return matrix + GetRandomMatrix(matrix.X, matrix.Y, min, max);
// //     }
// //     public static NeuralNetworkPair Crossing(NeuralNetworkPair pair)
// //     {
// //         var dot = Random.Next(pair.A.Matrices.Length);
// //         var a = pair.A.Matrices.AsSpan(0, dot).ToArray();
// //         var b = pair.A.Matrices.AsSpan(dot, pair.A.Matrices.Length - dot + 1).ToArray();
// //         var c = pair.B.Matrices.AsSpan(0, dot).ToArray();
// //         var d = pair.B.Matrices.AsSpan(dot, pair.B.Matrices.Length - dot + 1).ToArray();
// //         return new(new(a.Concat(c).ToArray()), new(b.Concat(d).ToArray()));
// //     }
// // }