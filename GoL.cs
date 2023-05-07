// using System.Windows.Controls;
// using System;
// using System.IO;
// using System.Diagnostics;
// using System.Windows;
// using System.Windows.Media;
// using System.Windows.Media.Imaging;
// using System.Windows.Threading;
// using System.Collections.Generic;
// using System.Windows.Input;
// using Button = Objects.Button;
// using Objects;
// using Objects.SpecialMath;
// using Objects.Data;
// using Style = Objects.Data.Style;
// using Vector = Objects.Vector;
// using static System.Math;
// namespace WpfApp1;
// partial class MainWindow
// {
//     static int _x, _y;
//     static Rules _rules;
//     private readonly DispatcherTimer _timer = new();
//     private readonly WriteableBitmap _bitmap;
//     private readonly Random _rng = new();
//     private static readonly double TimeStep = 1.0 / Stopwatch.Frequency;
//     private int _enter;
//     public int _f;
//     private int[,] _field = new int[400, 400];
//     private int _count;
//     private string _survival;
//     private string _birth;
//     private string _nbirth;
//     private int _generations;
//     private bool _stop;
//     private string _nsurvival;
//     private Color[] Colors = new Color[] { Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255), Color.FromRgb(255, 255, 0), Color.FromRgb(255, 0, 255), Color.FromRgb(0, 255, 255), Color.FromRgb(0, 0, 0), Color.FromRgb(255, 255, 255), Color.FromRgb(127, 255, 127) };
//     private double _prob;
//     private int _cell;
//     private int P1;
//     private int P2;
//     private int _radius;
//     private int _column;
//     private int _row;

//     enum Rules
//     {
//         Conway,
//         Avgust,
//         Ulam,
//         HighLife,
//         AvgustConway,
//         LifeWithoutDead,
//         DayAndNight,
//         Avgust2,
//         Wind,
//         StableLife,
//         NotStableLife,
//         HardLife,
//         Replace,
//         Lines,
//         Bomb,
//         Triangle,
//         SeedsWithoutDead,
//         Dragon,
//         Circle,
//         SeedsLight,
//         BigLife,
//         Fire,
//         Replicator,
//         Bombplicator,
//         HexSeeds,
//         Rakes,
//         Brain,
//         Cross,
//         Social,
//         Wireworld,
//         LogicRule,
//         War,
//     }
//     public MainWindow()
//     {
    
        
//         _stop = true;
//         //B34w/S23"birth 3 4&conf212 survival 23"
//         _birth = "3";
//         _survival = "2";
//         _prob = 0.1;
//         _cell = 1;
//         _nbirth = "";
//         _nsurvival = "";
//         _enter = 1;
//         _x = 200;
//         _y = 200;
//         _rules = Rules.Triangle;
//         InitializeComponent();
//         _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
//         image.Source = _bitmap;
//         _timer.Interval = TimeSpan.FromSeconds(0.01);
//         _timer.Tick += Tick;
//         KeyDown += ProcessInput;
//         _timer.Start();
//     }
//     private void Tick(object? sender, EventArgs e)
//     {
//         _bitmap.Lock();
//         for (int y = 0; y < _bitmap.PixelHeight; y++)
//         {
//             for (int x = 0; x < _bitmap.PixelWidth; x++)
//             {
//                 var color = _field[x / 2, y / 2] == 0 ? FromRgb(0, 0, 0) : FromRgb(255, 255, 255);
//                 var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
//                 unsafe
//                 {
//                     *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
//                 }
//             }
//         }

//         _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
//         _bitmap.Unlock();
//     }
//     public bool IsRect(int x, int y)
//     {
//         var current = 0;
//         var prev = 0;
//         for (int i = 400; i < x; i++)
//         {
//             for (int j = 400; j < y; j++)
//             {
//                 _field[i, j] = 1;
//             }
//         }
//         for (int i = 0; i < 50; i++)
//         {
//             Evolution2(null, new EventArgs());
//             prev = current;
//             current = _count;
//             if (prev == current)
//             {
//                 return false;
//             }
//         }
//         return true;
//     }
//     public Color HsvToRgb(int h, byte s, byte v)
//     {
//         var result = new Color();
//         var hue = h % 360;
//         var hv = (hue % 60) * 255 / 60;
//         var a = 0;
//         var b = hv;
//         var c = 255 - hv;
//         var d = 255;
//         void DoubleInterval(int min, int max, int r, int g, int b)
//         {
//             if (min <= hue && max > hue)
//             {
//                 result = Normalize(FromRgb(r, g, b), v);
//             }
//         }
//         DoubleInterval(0, 60, d, b, a);
//         DoubleInterval(60, 120, c, d, a);
//         DoubleInterval(120, 180, a, d, b);
//         DoubleInterval(180, 240, a, c, d);
//         DoubleInterval(240, 300, b, a, d);
//         DoubleInterval(300, 360, d, a, c);
//         return Interpolation(result, Color.FromRgb(v, v, v), s);
//     }
//     public Color Interpolation(Color a, Color b, byte c)
//     {
//         return FromRgb((a.R * c + b.R * (255 - c)) / 255, (a.G * c + b.G * (255 - c)) / 255, (a.B * c + b.B * (255 - c)) / 255);
//     }
//     private Color FromRgb(int r, int g, int b)
//     {
//         return Color.FromRgb(((byte)(r & 255)), ((byte)(g & 255)), ((byte)(b & 255)));
//     }
//     private Color FromSRgb(int r, int g, int b)
//     {
//         return Color.FromRgb(Saturate(r), Saturate(g), Saturate(b));
//     }
//     private byte Saturate(int value)
//     => value >= 0
//         ? value <= 255
//             ? (byte)value
//             : (byte)255
//         : (byte)0;
//     public Color Normalize(Color color, byte lightness)
//     {
//         var r = color.R;
//         var g = color.G;
//         var b = color.B;
//         var max = Math.Max(r, Math.Max(g, b));
//         if (max == 0)
//         {
//             return Color.FromRgb(lightness, lightness, lightness);
//         }
//         var normalizer = (int)lightness / max;
//         return Color.FromRgb((byte)(r * normalizer), ((byte)(g * normalizer)), ((byte)(b * normalizer)));

//     }
//     void ProcessInput(object sender, KeyEventArgs args)
//     {
//         var k = args;
//         switch (k.Key)
//         {
//             case Key.Left:
//                 if (_x != 0)
//                     _x--;
//                 break;

//             case Key.Right:
//                 if (_x < _field.GetLength(0) - 1)
//                     _x++;
//                 break;

//             case Key.Up:
//                 if (_y != 0)
//                     _y--;
//                 break;

//             case Key.Down:
//                 if (_y < _field.GetLength(1) - 1)
//                     _y++;
//                 break;

//             case Key.Space:
//                 if (_rules == Rules.Wireworld)
//                 {
//                     if (_field[_x, _y] < 3)
//                     {
//                         _field[_x, _y]++;
//                     }
//                     else
//                     {
//                         _field[_x, _y] = 0;
//                     }
//                 }
//                 else if (_rules != Rules.Cross)
//                 {
//                     _field[_x, _y] = _field[_x, _y] == 0 ? 1 : 0;
//                 }
//                 else
//                 {
//                     if (_field[_x, _y] < 3)
//                     {
//                         _field[_x, _y]++;
//                     }
//                     else
//                     {
//                         _field[_x, _y] = 0;
//                     }
//                 }
//                 break;

//             case Key.C:
//                 Array.Clear(_field, 0, _field.GetLength(0) * _field.GetLength(1));
//                 break;

//             case Key.I:
//                 for (int row = 0; row < _field.GetLength(1); row++)
//                 {
//                     for (int column = 0; column < _field.GetLength(0); column++)
//                     {
//                         _field[column, row] = (int)(_field[column, row] != 0 ? 0 : 1);
//                     }
//                 }
//                 break;
//             case Key.N:
//                 Evolution2(null, new EventArgs());
//                 break;
//             case Key.R:
//                 OnRandom(_prob, _cell);
//                 break;
//             case Key.S:
//                 if (_stop)
//                 {
//                     _timer.Tick += Evolution2;
//                     _stop = false;
//                 }
//                 else
//                 {
//                     _timer.Tick -= Evolution2;
//                     _stop = true;
//                 }
//                 break;
//         }
//     }
//     void Save()
//     {
//         Console.Clear();
//         Console.Write("Введите название: ");
//         var name = Console.ReadLine();
//         var fileName = $"{name}.field";
//         using (var file = File.Open(fileName, FileMode.Create))
//         {
//             var writer = new BinaryWriter(file);
//             writer.Write(_field.GetLength(0));
//             writer.Write(_field.GetLength(1));
//             for (int row = 0; row < _field.GetLength(1); row++)
//             {
//                 for (int column = 0; column < _field.GetLength(0); column++)
//                 {
//                     writer.Write(_field[column, row]);
//                 }
//             }
//         }
//     }

//     int GetNeighborCount3(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var count = 0;
//         if (row != 0 && column < width - 1 && _field[column + 1, row - 1] != 0)
//         {
//             count++;
//         }
//         if (row != 0 && column != 0 && _field[column - 1, row - 1] != 0)
//         {
//             count++;
//         }
//         if (row != 0 && _field[column, row - 1] != 0)
//         {
//             count++;
//         }
//         return count;
//     }
//     int GetNeighborCount(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row != 0 && column != 0 && _field[column - 1, row - 1] != 0)
//         {
//             count+=2;
//         }

//         if (row != 0 && _field[column, row - 1] != 0)
//         {
//             count++;
//         }

//         if (row != 0 && column < width - 1 && _field[column + 1, row - 1] != 0)
//         {
//             count+=2;
//         }

//         if (column < width - 1 && _field[column + 1, row] != 0)
//         {
//             count++;
//         }

//         if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] != 0)
//         {
//             count++;
//         }

//         if (row < height - 1 && _field[column, row + 1] != 0)
//         {
//             count++;
//         }

//         if (row < height - 1 && column != 0 && _field[column - 1, row + 1] != 0)
//         {
//             count++;
//         }

//         if (column != 0 && _field[column - 1, row] != 0)
//         {
//             count++;
//         }

//         return count;
//     }
//     List<int> Neighbor(int column, int row)
//     {
//         var count = 0;//"."
//         var count2 = 0;//"-"
//         var orienation = false;
//         List<int> result = new List<int>();
//         foreach (var item in GetNeighbor(column, row))
//         {
//             if (item == '-' && orienation)
//             {
//                 count++;
//                 orienation = false;
//                 result.Add(count);
//                 count = 0;
//             }
//             if (item == '-' && !orienation)
//             {
//                 count++;
//                 orienation = false;

//             }
//             if (item == '.' && orienation)
//             {
//                 count2++;
//                 orienation = true;
//             }
//             if (item == '.' && !orienation)
//             {
//                 count2++;
//                 orienation = true;
//                 result.Add(count2);
//                 count2 = 0;
//             }
//         }
//         if (result.Count % 2 == 1)
//         {
//             result[0] += result.Last();
//             result.RemoveAt(result.Count - 1);
//         }
//         return result;
//     }
//     string GetNeighbor(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var result = "";

//         if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row < height - 1 && _field[column, row + 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row < height - 1 && column != 0 && _field[column - 1, row + 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (column != 0 && _field[column - 1, row] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row != 0 && column != 0 && _field[column - 1, row - 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row != 0 && _field[column, row - 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row != 0 && column < width - 1 && _field[column + 1, row - 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (column < width - 1 && _field[column + 1, row] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         return result;
//     }
//     string GetNeighbor2(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var result = "";


//         if (row != 0 && column != 0 && _field[column - 1, row - 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row != 0 && _field[column, row - 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row != 0 && column < width - 1 && _field[column + 1, row - 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (column < width - 1 && _field[column + 1, row] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row < height - 1 && _field[column, row + 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (row < height - 1 && column != 0 && _field[column - 1, row + 1] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         if (column != 0 && _field[column - 1, row] != 0)
//         {
//             result += '.';
//         }
//         else
//         {
//             result += '-';
//         }
//         return result;
//     }
//     int GetNeighborCount(int column, int row, int cell)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row != 0 && column != 0 && _field[column - 1, row - 1] == cell)
//         {
//             count++;
//         }

//         if (row != 0 && _field[column, row - 1] == cell)
//         {
//             count++;
//         }

//         if (row != 0 && column < width - 1 && _field[column + 1, row - 1] == cell)
//         {
//             count++;
//         }

//         if (column < width - 1 && _field[column + 1, row] == cell)
//         {
//             count++;
//         }

//         if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] == cell)
//         {
//             count++;
//         }

//         if (row < height - 1 && _field[column, row + 1] == cell)
//         {
//             count++;
//         }

//         if (row < height - 1 && column != 0 && _field[column - 1, row + 1] == cell)
//         {
//             count++;
//         }

//         if (column != 0 && _field[column - 1, row] == cell)
//         {
//             count++;
//         }

//         return count;
//     }
//     int GetBigNeighborCount(int column, int row, int radius)
//     {
//         var count = 0;
//         for (int j = -radius; j <= radius; j++)
//         {
//             for (int i = -radius; i <= radius; i++)
//             {
//                 if (i == 0 && j == 0)
//                 {
//                     continue;
//                 }
//                 GetCell(column + i, row + j);
//             }
//         }
//         return count;
//     }
//     void GetCell(int column, int row)
//     {
//         if (column < 0 || column > _field.GetLength(0) - 1)
//         {
//             return;
//         }
//         if (row < 0 || row > _field.GetLength(1) - 1)
//         {
//             return;
//         }
//         _count += _field[column, row];
//     }
//     int GetNeighborCount2(int column, int row, int cell)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row != 0 && _field[column, row - 1] == cell)
//         {
//             count++;
//         }

//         if (column < width - 1 && _field[column + 1, row] == cell)
//         {
//             count++;
//         }

//         if (row < height - 1 && _field[column, row + 1] == cell)
//         {
//             count++;
//         }

//         if (column != 0 && _field[column - 1, row] == cell)
//         {
//             count++;
//         }

//         return count;
//     }
//     int GetHexNeighborCount(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row != 0 && column != 0 && _field[column - 1, row - 1] != 0)
//         {
//             count++;
//         }

//         if (row != 0 && _field[column, row - 1] != 0)
//         {
//             count++;
//         }

//         if (column < width - 1 && _field[column + 1, row] != 0)
//         {
//             count++;
//         }

//         if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] != 0)
//         {
//             count++;
//         }

//         if (row < height - 1 && _field[column, row + 1] != 0)
//         {
//             count++;
//         }

//         if (column != 0 && _field[column - 1, row] != 0)
//         {
//             count++;
//         }

//         return count;
//     }

//     int GetNeighborCount2(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row != 0 && _field[column, row - 1] != 0)
//         {
//             count++;
//         }

//         if (column < width - 1 && _field[column + 1, row] != 0)
//         {
//             count++;
//         }

//         if (row < height - 1 && _field[column, row + 1] != 0)
//         {
//             count++;
//         }


//         if (column != 0 && _field[column - 1, row] != 0)
//         {
//             count++;
//         }

//         return count;
//     }
//     int NeighborAsGreenCell(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row != 0 && column != 0 && _field[column - 1, row - 1] == 3)
//         {
//             _field[column - 1, row - 1] = 4;
//         }

//         if (row != 0 && _field[column, row - 1] == 3)
//         {
//             _field[column, row - 1] = 4;
//         }

//         if (row != 0 && column < width - 1 && _field[column + 1, row - 1] == 3)
//         {
//             _field[column + 1, row - 1] = 4;
//         }

//         if (column < width - 1 && _field[column + 1, row] == 3)
//         {
//             _field[column + 1, row] = 4;
//         }

//         if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] == 3)
//         {
//             _field[column + 1, row + 1] = 4;
//         }

//         if (row < height - 1 && _field[column, row + 1] == 3)
//         {
//             _field[column, row + 1] = 4;
//         }

//         if (row < height - 1 && column != 0 && _field[column - 1, row + 1] == 3)
//         {
//             _field[column - 1, row + 1] = 4;
//         }

//         if (column != 0 && _field[column - 1, row] == 3)
//         {
//             _field[column - 1, row] = 4;
//         }

//         return count;
//     }
//     void Evolution(object? sender, EventArgs args)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var newField = new int[width, height];
//         for (int row = 0; row < height; row++)
//         {
//             for (int column = 0; column < width; column++)
//             {
//                 switch (_rules)
//                 {
//                     case Rules.Conway:
//                         {
//                             var count = 0.0F;
//                             NeighborhoodActivate(column, row, d => { count += d; });
//                             if (_field[column, row] != 0)
//                             {
//                                 if (count > 1 && count < 2)
//                                 {
//                                     newField[column, row] = _field[column, row];
//                                 }
//                             }
//                             else
//                             {
                                
//                             }
//                             break;
//                         }

//                     case Rules.Avgust:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (_field[column, row] == 0)
//                             {
//                                 if (count == 2)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;
//                         }

//                     case Rules.Ulam:
//                         {
//                             var count = GetNeighborCount2(column, row);
//                             if (_field[column, row] != 0)
//                             {
//                                 if (_field[column, row] < 3)
//                                 {
//                                     _field[column, row] += 1;
//                                 }
//                             }
//                             else
//                             {
//                                 if (count == 1)
//                                 {
//                                     newField[column, row] = _field[column, row] + 1;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.HighLife:
//                         {
//                             var count = GetNeighborCount(column, row);

//                             if (_field[column, row] != 0)
//                             {
//                                 if (count == 2 || count == 3)
//                                 {
//                                     newField[column, row] = _field[column, row] + 1;
//                                 }
//                             }
//                             else
//                             {
//                                 if (count is 3 or 6)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.AvgustConway:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (count == 2)
//                             {
//                                 newField[column, row] = 1;
//                             }
//                             if (_field[column, row] != 0)
//                             {
//                                 if (count == 2 || count == 3)
//                                 {
//                                     newField[column, row] = _field[column, row] + 1;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.LifeWithoutDead:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (_field[column, row] != 0)
//                             {
//                                 newField[column, row] = _field[column, row] + 1;
//                             }
//                             else
//                             {
//                                 if (count == 3)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.DayAndNight:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (_field[column, row] != 0)
//                             {
//                                 if (count is 3 or 4 or 6 or 7 or 8)
//                                 {
//                                     newField[column, row] = _field[column, row] + 1;
//                                 }
//                             }
//                             else
//                             {
//                                 if (count is 3 or 6 or 7 or 8)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.Avgust2:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (_field[column, row] != 0)
//                             {
//                             }
//                             else
//                             {
//                                 if (count == 2 || count == 4)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.Wind:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (_field[column, row] != 0)
//                             {
//                             }
//                             else
//                             {
//                                 if (count is 3 || column != 0 && _field[column - 1, row] != 0)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.StableLife:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (_field[column, row] != 0)
//                             {
//                                 if (count is 2 or 3 or 4 or 8)
//                                 {
//                                     newField[column, row] = _field[column, row] + 1;
//                                 }
//                             }
//                             else
//                             {
//                                 if (count == 3)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;

//                         }
//                     case Rules.NotStableLife:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (_field[column, row] != 0)
//                             {
//                                 if (count is 2 or 4 or 5)
//                                 {
//                                     newField[column, row] = _field[column, row] + 1;
//                                 }
//                             }
//                             else
//                             {
//                                 if (count is 3 or 5)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.Replace:
//                         {
//                             _field[column, row] = ((int)(GetNeighborCount(column, row) % 2));
//                             break;
//                         }
//                     case Rules.HardLife:
//                         {
//                             int NeighborCount(int cell)
//                             {
//                                 return GetNeighborCount(column, row, cell);
//                             }

//                             if (_field[column, row] == 3)
//                             {
//                                 var leftAndRightExpand = true;
//                                 if (column != 0 && _field[column - 1, row] == 0)
//                                 {
//                                     _field[column - 1, row] = 3;
//                                 }
//                                 else
//                                 {
//                                     leftAndRightExpand = false;
//                                 }
//                                 if (column != _field.GetLength(0) && _field[column + 1, row] == 0)
//                                 {
//                                     newField[column + 1, row] = 3;
//                                 }
//                                 else
//                                 {
//                                     leftAndRightExpand = false;
//                                 }
//                                 var upExpand = true;
//                                 if (leftAndRightExpand && row != 0 && _field[column, row - 1] == 0)
//                                 {
//                                     newField[column, row - 1] = 3;
//                                 }
//                                 else
//                                 {
//                                     upExpand = false;
//                                 }
//                                 if (!upExpand && row != _field.GetLength(1) && _field[column, row + 1] == 0)
//                                 {
//                                     newField[column, row + 1] = 3;
//                                 }
//                             }
//                             if (_field[column, row] == 4)
//                             {
//                                 NeighborAsGreenCell(column, row);
//                                 if (NeighborCount(4) == 0)
//                                 {
//                                     newField[column, row] = 0;
//                                 }
//                             }
//                             if (_field[column, row] == 1)
//                             {
//                                 if (NeighborCount(2) > 2)
//                                 {
//                                     newField[column, row] = 0;
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.Lines:
//                         if (GetNeighborCount2(column, row) == 1)
//                         {
//                             newField[column, row] = 1;
//                         }
//                         if (_field[column, row] != 0)
//                         {
//                             newField[column, row] = 1;
//                         }
//                         break;
//                     case Rules.Bomb:
//                         if (GetNeighborCount(column, row) == 1)
//                         {
//                             newField[column, row] = _field[column, row] + 1;
//                             break;
//                         }
//                         if (_field[column, row] != 0)
//                         {
//                             newField[column, row] = _field[column, row] + 1;
//                         }
//                         break;
//                     case Rules.Triangle:

//                         if (row != 0 && column != 0 && column != _field.GetLength(1) - 1)
//                         {
//                             _field[column, row] = (_field[column + 1, row - 1] + _field[column + 1, row - 1]) & 3;
//                         }
//                         break;
//                     case Rules.SeedsWithoutDead:
//                         if (_field[column, row] != 0)
//                         {
//                             newField[column, row] = 1;
//                         }
//                         if (GetNeighborCount(column, row) == 2)
//                         {
//                             newField[column, row] = 1;
//                         }
//                         break;
//                     case Rules.Dragon:
//                         if (_field[column, row] != 0)
//                         {
//                             if (GetNeighborCount(column, row) is 0 or 1 or 8)
//                             {
//                                 newField[column, row] = _field[column, row] + 1;
//                             }
//                         }
//                         else
//                         {
//                             if (GetNeighborCount(column, row) is 3 or 4)
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         break;
//                     case Rules.Circle:
//                         var next = (_field[column, row] + 1) % 3;
//                         if (GetNeighborCount(column, row, next) > 2 || _rng.NextDouble() < 0.01)
//                         {
//                             newField[column, row] = next;
//                             break;
//                         }
//                         else
//                         {
//                             newField[column, row] = _field[column, row];
//                         }
//                         break;
//                     case Rules.SeedsLight:
//                         if (GetNeighborCount(column, row) is 1 or 2 && _field[column, row] != 0)
//                         {
//                             newField[column, row] = _field[column, row]++;
//                         }
//                         else if (GetNeighborCount(column, row) is 2 && (!GetNeighbor(column, row).Contains("..") && !GetNeighbor2(column, row).Contains("..")) && _field[column, row] == 0)
//                         {
//                             newField[column, row] = 1;
//                         }
//                         break;
//                     case Rules.BigLife:
//                         if (_field[column, row] != 0)
//                         {
//                             if (GetNeighborCount(column, row) is 2 or 3 or 4)
//                             {
//                                 newField[column, row] = _field[column, row] + 1;
//                             }
//                         }
//                         else
//                         {
//                             if (GetNeighborCount(column, row) is 4 or 5 or 6)
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         break;
//                     case Rules.Fire:
//                         if (_field[column, row] != 0)
//                         {
//                             if (GetNeighborCount(column, row) is 0 or 1 or 2 or 8 or 3)
//                             {
//                                 newField[column, row] = _field[column, row] + 1;
//                             }
//                         }
//                         else
//                         {
//                             if (GetNeighbor(column, row).Contains("-.d..d-") || GetNeighbor2(column, row).Contains("-.d..d-"))
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         break;
//                     case Rules.Replicator:
//                         if (_field[column, row] != 0)
//                         {
//                             if (GetNeighborCount(column, row) is 1 or 3 or 5 or 7)
//                             {
//                                 newField[column, row] = _field[column, row] + 1;
//                             }
//                         }
//                         else
//                         {
//                             if (GetNeighborCount(column, row) is 1 or 3 or 5 or 7)
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         break;
//                     case Rules.Bombplicator:
//                         if (GetNeighborCount(column, row) is 2 or 3 && _field[column, row] != 0)
//                         {
//                             newField[column, row] = _field[column, row]++;
//                         }
//                         else if (GetNeighborCount(column, row) is 3 || (GetNeighborCount(column, row) is 4 && (GetNeighbor(column, row).Contains("..-..") || GetNeighbor2(column, row).Contains("..-..")) && _field[column, row] == 0))
//                         {
//                             newField[column, row] = 1;
//                         }
//                         break;
//                     case Rules.HexSeeds:
//                         {
//                             if (_field[column, row] == 0)
//                             {
//                                 if (GetHexNeighborCount(column, row) is 2 or 3)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             else
//                             {
//                                 if (GetHexNeighborCount(column, row) == 0)
//                                 {
//                                     newField[column, row] = _field[column, row];
//                                 }
//                             }
//                             break;
//                         }
//                     case Rules.Rakes:
//                         {
//                             string neighbor = GetNeighbor(column, row);
//                             string neighbor2 = GetNeighbor2(column, row);
//                             if (GetNeighborCount(column, row) is 0 && _field[column, row] != 0)
//                             {
//                                 newField[column, row] = _field[column, row];
//                             }
//                             else if (neighbor.Contains("..") || neighbor2.Contains("..") || (GetNeighborCount2(column, row) == 2 && neighbor.Contains(".-.") || neighbor2.Contains(".-.")) || neighbor.Contains("..--.") | neighbor2.Contains("..--.") && _field[column, row] == 0)
//                             {
//                                 newField[column, row] = 1;
//                             }
//                             break;
//                         }
//                     case Rules.Brain:
//                         if (_field[column, row] == 0 && GetNeighborCount4(column, row) == 2)
//                         {
//                             newField[column, row] = 1;
//                         }
//                         if (_field[column, row] == 1)
//                         {
//                             newField[column, row] = 2;
//                         }
//                         if (_field[column, row] == 2)
//                         {
//                             newField[column, row] = 0;
//                         }
//                         break;
//                     case Rules.Cross:
//                         if (_field[column, row] == 1)
//                         {
//                             if (GetNeighborCount(column, row, 3) > 1)
//                             {
//                                 newField[column, row] = 3;
//                                 break;
//                             }
//                             if (GetNeighborCount2(column, row, 1) is 0 or 1 or 2)
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         if (_field[column, row] == 0)
//                         {
//                             if (GetNeighborCount2(column, row) is 1 && GetNeighborCount(column, row) is 1)
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         if (_field[column, row] == 2)
//                         {
//                             newField[column, row] = 2;
//                         }
//                         break;
//                     case Rules.Social:
//                         if (_field[column, row] == 0)
//                         {
//                             _field[column, row] = _rng.NextDouble() < P1 ? 0 : 1;
//                         }
//                         if (_field[column, row] == 1)
//                         {
//                             _field[column, row] = _rng.NextDouble() < P2 ? 1 : 0;
//                         }
//                         if (_field[column, row] != 0)
//                         {
//                             if (GetNeighborCount(column, row) is > 1 and not 8)
//                             {
//                                 newField[column, row] = _field[column, row] + 1;
//                             }
//                         }
//                         else
//                         {
//                             if (GetNeighborCount(column, row) > 2)
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         break;
//                     case Rules.Wireworld:
//                         if (_field[column, row] == 1)
//                         {
//                             if (GetNeighborCount(column, row, 2) is 1 or 2)
//                             {
//                                 newField[column, row] = 2;
//                             }
//                             else
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         if (_field[column, row] == 2)
//                         {
//                             if (GetNeighborCount(column, row, 3) < 2)
//                             {
//                                 newField[column, row] = 3;
//                             }
//                             else
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         if (_field[column, row] == 3)
//                         {
//                             newField[column, row] = 1;
//                         }
//                         break;
//                     case Rules.LogicRule:
//                         if (_field[column, row] == 0)
//                         {
//                             if ((GetNeighbor(column, row).Contains("..") || GetNeighbor2(column, row).Contains("..")) && GetNeighborCount(column, row) == 2 || (GetNeighborCount2(column, row) == 2 && GetNeighborCount(column, row) == 2 && (GetNeighbor(column, row).Contains(".-.") || GetNeighbor2(column, row).Contains(".-."))))
//                             {
//                                 newField[column, row] = 1;
//                             }
//                         }
//                         break;
//                 }

//             }
//         }

//         _field = newField;
//     }
//     void NeighborhoodActivate(int column, int row, Action<int> action)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         if (row > 0 && column > 0)
//         {
//             action(_field[column - 1, row - 1]);
//         }

//         if (row > 0)
//         {
//             action(_field[column, row - 1]);
//         }

//         if (row > 0 && column < width - 1)
//         {
//             action(_field[column + 1, row - 1]);
//         }

//         if (column < width - 1)
//         {
//             action(_field[column + 1, row]);
//         }

//         if (column < width - 1 && row < height - 1)
//         {
//             action(_field[column + 1, row + 1]);
//         }

//         if (row < height - 1)
//         {
//             action(_field[column, row + 1]);
//         }

//         if (row < height - 1 && column > 0)
//         {
//             action(_field[column - 1, row + 1]);
//         }

//         if (column > 0)
//         {
//             action(_field[column - 1, row]);
//         }
//     }
//     public void Evolution(string birth, string survival)
//     {
//         for (int i = 0; i < 10; i++)
//         {
//             var width = _field.GetLength(0);
//             var height = _field.GetLength(1);
//             var newField = new int[width, height];
//             for (int row = 0; row < height; row++)
//             {
//                 for (int column = 0; column < width; column++)
//                 {
//                     if (_field[column, row] != 0)
//                     {
//                         if (IsContains(survival, column, row))
//                         {
//                             newField[column, row] = _field[column, row] + 1;
//                         }
//                     }
//                     else
//                     {
//                         if (IsContains(birth, column, row))
//                         {
//                             newField[column, row] = 1;
//                         }
//                     }
//                 }

//             }
//             _field = newField;
//         }

//     }
//     public void Evolution2(object? sender, EventArgs args)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var newField = new int[width, height];
//         for (int row = 0; row < height; row++)
//         {
//             for (int column = 0; column < width; column++)
//             {
//                 if (_field[column, row] != 0)
//                 {
//                     if (_survival.Contains(GetNeighborCount(column, row).ToString()))
//                     {
//                         newField[column, row] = _field[column, row] + 1;
//                     }
//                 }
//                 else
//                 {
//                     if (_birth.Contains(GetNeighborCount(column, row).ToString()) && _field[column, row] == 0)
//                     {
//                         newField[column, row] = 1;
//                     }
//                 }
//             }
//         }
//         _field = newField;
//     }
//     public void Evolution3(object? sender, EventArgs args)
//     {
//         for (int i = 0; i < 1; i++)
//         {
//             var width = _field.GetLength(0);
//             var height = _field.GetLength(1);
//             var newField = new int[width, height];
//             for (int row = 0; row < height; row++)
//             {
//                 for (int column = 0; column < width; column++)
//                 {
//                     if (_field[column, row] != 0)
//                     {
//                         if (_survival.Contains(GetNeighborCount(column, row).ToString()))
//                         {
//                             newField[column, row] = _field[column, row];
//                         }
//                         else
//                         {
//                             newField[column, row] = _field[column, row];

//                         }
//                     }
//                     else
//                     {
//                         if (_birth.Contains(GetNeighborCount(column, row).ToString()) && _field[column, row] == 0)
//                         {
//                             newField[column, row] = 1;
//                         }
//                     }
//                 }
//             }
//             _field = newField;
//         }

//     }
//     private bool[,] GetBitmap()
//     {
//         var result = new bool[_field.GetLength(0), _field.GetLength(1)];
//         for (int i = 0; i < _field.GetLength(0) - 1; i++)
//         {
//             for (int j = 0; j < _field.GetLength(1) - 1; j++)
//             {
//                 result[i, j] = _field[i, j] != 0;
//             }
//         }
//         return result;
//     }

//     void DrawField2()
//     {
//         Console.CursorLeft = 0;
//         Console.CursorTop = 0;
//         for (int row = 0; row < _field.GetLength(1); row++)
//         {
//             for (int column = 0; column < _field.GetLength(0); column++)
//             {
//                 char print = '.';
//                 switch (_field[column, row])
//                 {
//                     case 0:
//                         break;
//                     case 2:
//                         Console.ForegroundColor = ConsoleColor.Magenta;
//                         break;
//                     case 3:
//                         Console.ForegroundColor = ConsoleColor.Blue;
//                         break;
//                     case 4:
//                         Console.ForegroundColor = ConsoleColor.Green;
//                         break;
//                     case 5:
//                         Console.ForegroundColor = ConsoleColor.Yellow;
//                         break;
//                     case 6:
//                         Console.ForegroundColor = ConsoleColor.Red;
//                         break;
//                 }
//                 Console.Write(print);
//                 Console.ForegroundColor = ConsoleColor.White;
//             }
//             Console.WriteLine();
//         }
//         Console.CursorLeft = _x;
//         Console.CursorTop = _y;
//     }
//     void OnRandom(double probability, int cell)
//     {
//         var rng = new Random();
//         for (int row = 0; row < _field.GetLength(1); row++)
//         {
//             for (int column = 0; column < _field.GetLength(0); column++)
//             {
//                 if (rng.NextDouble() < probability)
//                 {
//                     _field[column, row] = cell;
//                 }
//             }
//         }
//     }
//     void Line(int iBegin, int jBegin, int length)
//     {
//         for (int i = 0; i < length; i++)
//         {
//             _field[iBegin + i, jBegin] = 1;
//         }
//     }
//     int GetNeighborCount4(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row != 0 && column != 0 && _field[column - 1, row - 1] == 1)
//         {
//             count++;
//         }

//         if (row != 0 && _field[column, row - 1] == 1)
//         {
//             count++;
//         }

//         if (row != 0 && column < width - 1 && _field[column + 1, row - 1] == 1)
//         {
//             count++;
//         }

//         if (column < width - 1 && _field[column + 1, row] == 1)
//         {
//             count++;
//         }

//         if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] == 1)
//         {
//             count++;
//         }

//         if (row < height - 1 && _field[column, row + 1] == 1)
//         {
//             count++;
//         }

//         if (row < height - 1 && column != 0 && _field[column - 1, row + 1] == 1)
//         {
//             count++;
//         }

//         if (column != 0 && _field[column - 1, row] == 1)
//         {
//             count++;
//         }

//         return count;
//     }
//     public bool IsContains(string s, int column, int row)
//     {
//         return GetNeighbor(column, row).Contains(s) || GetNeighbor2(column, row).Contains(s) || s.Contains(GetNeighborCount(column, row).ToString());
//     }
// }
