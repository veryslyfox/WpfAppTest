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
//     private double _time;
//     private int _f;
//     private Color[] _labs = new Color[] { Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255) };
//     private Color? _mouseColor;
//     private uint[,] _field = new uint[800, 800];
//     private int[,] _norm = new int[800, 800];
//     private double _m;
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
//         Replace
//     }
//     public MainWindow()
//     {
//         InitializeComponent();
//         _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
//         image.Source = _bitmap;
//         _timer.Interval = TimeSpan.FromSeconds(0.1);
//         OnRandom(0.6);
//         _rules = Rules.StableLife;
//         _timer.Tick += Tick;
//         KeyDown += ProcessInput;
//         _timer.Tick += Evolution;
//         _timer.Start();
//     }
//     private void Tick(object? sender, EventArgs e)
//     {
//         _bitmap.Lock();
//         for (int y = 0; y < _bitmap.PixelHeight; y++)
//         {
//             for (int x = 0; x < _bitmap.PixelWidth; x++)
//             {
//                 Color color;
//                 var c = (byte)(_field[x, y] * 30);
//                 color = FromSRgb(c, c, c); 
//                 var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
//                 unsafe
//                 {
//                     *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
//                 }
//             }
//         }
//         _f++;
//         _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
//         _bitmap.Unlock();
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
//         var normalizer = (double)lightness / max;
//         return Color.FromRgb((byte)(r * normalizer), ((byte)(g * normalizer)), ((byte)(b * normalizer)));

//     }
//     void ProcessInput(object sender, KeyEventArgs args)
//     {
//         var k = args;
//         switch (k.Key)
//         {
//             case Key.Left:
//                 if (_x > 0)
//                     _x--;
//                 break;

//             case Key.Right:
//                 if (_x < _field.GetLength(0) - 1)
//                     _x++;
//                 break;

//             case Key.Up:
//                 if (_y > 0)
//                     _y--;
//                 break;

//             case Key.Down:
//                 if (_y < _field.GetLength(1) - 1)
//                     _y++;
//                 break;

//             case Key.Space:
//                 if (_rules != Rules.HardLife)
//                     _field[_x, _y] = _field[_x, _y] == 0 ? 1u : 0u;
//                 else
//                 {
//                     if (_field[_x, _y] != 4)
//                         _field[_x, _y]++;
//                     else
//                         _field[_x, _y] = 0;
//                 }
//                 break;

//             case Key.S:
//                 Save();
//                 break;

//             case Key.L:
//                 Load();
//                 break;

//             case Key.C:
//                 Array.Clear(_field, 0, _field.GetLength(0) * _field.GetLength(1));
//                 break;

//             case Key.I:
//                 for (int row = 0; row < _field.GetLength(1); row++)
//                 {
//                     for (int column = 0; column < _field.GetLength(0); column++)
//                     {
//                         _field[column, row] = (uint)(_field[column, row] != 0 ? 0 : 1);
//                     }
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

//     private void Load()
//     {
//         Console.Clear();
//         Console.Write("Введите название: ");
//         var name = Console.ReadLine();
//         var fileName = $"{name}.field";
//         if (!File.Exists(fileName))
//         {
//             return;
//         }

//         using (var file = File.Open(fileName, FileMode.Open))
//         {
//             var reader = new BinaryReader(file);
//             var width = reader.ReadInt32();
//             var height = reader.ReadInt32();
//             var field = new uint[width, height];
//             for (int row = 0; row < field.GetLength(1); row++)
//             {
//                 for (int column = 0; column < field.GetLength(0); column++)
//                 {
//                     field[column, row] = reader.ReadUInt32();
//                 }
//             }
//             _field = field;
//         }
//     }

//     int GetNeighborCount(int column, int row)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row > 0 && column > 0 && _field[column - 1, row - 1] != 0)
//         {
//             count++;
//         }

//         if (row > 0 && _field[column, row - 1] != 0)
//         {
//             count++;
//         }

//         if (row > 0 && column < width - 1 && _field[column + 1, row - 1] != 0)
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

//         if (row < height - 1 && column > 0 && _field[column - 1, row + 1] != 0)
//         {
//             count++;
//         }

//         if (column > 0 && _field[column - 1, row] != 0)
//         {
//             count++;
//         }

//         return count;
//     }
//     int GetNeighborCount(int column, int row, int cell)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var count = 0;
//         if (row > 0 && column > 0 && _field[column - 1, row - 1] == cell)
//         {
//             count++;
//         }

//         if (row > 0 && _field[column, row - 1] == cell)
//         {
//             count++;
//         }

//         if (row > 0 && column < width - 1 && _field[column + 1, row - 1] == cell)
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

//         if (row < height - 1 && column > 0 && _field[column - 1, row + 1] == cell)
//         {
//             count++;
//         }

//         if (column > 0 && _field[column - 1, row] == cell)
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
//         if (row > 0 && column > 0 && _field[column - 1, row - 1] != 0)
//         {
//             count++;
//         }

//         if (row > 0 && _field[column, row - 1] != 0)
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

//         if (column > 0 && _field[column - 1, row] != 0)
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
//         if (row > 0 && _field[column, row - 1] != 0)
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


//         if (column > 0 && _field[column - 1, row] != 0)
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
//         if (row > 0 && column > 0 && _field[column - 1, row - 1] == 3)
//         {
//             _field[column - 1, row - 1] = 4;
//         }

//         if (row > 0 && _field[column, row - 1] == 3)
//         {
//             _field[column, row - 1] = 4;
//         }

//         if (row > 0 && column < width - 1 && _field[column + 1, row - 1] == 3)
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

//         if (row < height - 1 && column > 0 && _field[column - 1, row + 1] == 3)
//         {
//             _field[column - 1, row + 1] = 4;
//         }

//         if (column > 0 && _field[column - 1, row] == 3)
//         {
//             _field[column - 1, row] = 4;
//         }

//         return count;
//     }
//     void Evolution(object? sender, EventArgs args)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var newField = new uint[width, height];
//         for (int row = 0; row < height; row++)
//         {
//             for (int column = 0; column < width; column++)
//             {
//                 switch (_rules)
//                 {
//                     case Rules.Conway:
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
//                                 if (count == 3)
//                                 {
//                                     newField[column, row] = 1;
//                                 }
//                             }
//                             break;
//                         }

//                     case Rules.Avgust:
//                         {
//                             var count = GetNeighborCount(column, row);
//                             if (_field[column, row] != 0)
//                             {
//                             }
//                             else
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
//                             int count = GetNeighborCount(column, row);
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
//                             _field[column, row] = ((uint)(GetNeighborCount(column, row) % 2));
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

//                 }

//             }
//         }

//         _field = newField;
//     }

//     private bool[,] GetBitmap()
//     {
//         var result = new bool[_field.GetLength(0), _field.GetLength(1)];
//         for (int i = 0; i < _field.GetLength(0) - 1; i++)
//         {
//             for (int j = 0; j < _field.GetLength(1) - 1; j++)
//             {
//                 result[i, j] = _field[i, j] > 0;
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
//     void OnRandom(double probability)
//     {
//         var rng = new Random();
//         for (int row = 0; row < _field.GetLength(1); row++)
//         {
//             for (int column = 0; column < _field.GetLength(0); column++)
//             {
//                 if (rng.NextDouble() < probability)
//                 {
//                     _field[column, row] = 1;
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
// }