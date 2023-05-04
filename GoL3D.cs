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
// namespace WpfApp1;
// partial class MainWindow
// {
//     Rules _rules;
//     private readonly DispatcherTimer _timer = new();
//     private readonly WriteableBitmap _bitmap;
//     private readonly Random _rng = new();
//     private readonly double TimeStep = 1.0 / Stopwatch.Frequency;
//     bool[,,] _field;
//     int _x, _y, _z;
//     int[,] _depthMap;
//     int[] _birth = new int[] { 5 };
//     int[] _survival = new int[] { 4, 5 };
//     enum Rules
//     {
//         Life,
//         HTree,
//         Conway,
//         Seeds
//     }
//     public MainWindow()
//     {
//         _x = 10;
//         _y = 10;
//         _z = 10;
//         _field = new bool[40, 40, 40];
//         _depthMap = new int[40, 40];
//         InitializeComponent();
//         _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
//         image.Source = _bitmap;
//         _timer.Interval = TimeSpan.FromSeconds(0.01);
//         _timer.Tick += Tick;
//         KeyDown += ProcessInput;
//         _timer.Start();
//     }
//     void Add(int x, int y, int z)
//     {
//         _field[x, y, z] = true;
//         if (z < _depthMap[x, y])
//         {
//             GetDepth(x, y);
//         }
//     }
//     void Add(int x, int y, int z, bool[,,] field)
//     {
//         field[x, y, z] = true;
//         if (z < _depthMap[x, y])
//         {
//             GetDepth(x, y);
//         }
//     }
//     void GetDepth(int xColumn, int yColumn)
//     {
//         for (int z = 0; z < _field.GetLength(2); z++)
//         {
//             if (_field[xColumn, yColumn, z])
//             {
//                 _depthMap[xColumn, yColumn] = z;
//                 return;
//             }
//         }
//         _depthMap[xColumn, yColumn] = -1;
//     }
//     private void Tick(object? sender, EventArgs e)
//     {
//         _bitmap.Lock();
//         for (int y = 0; y < _bitmap.PixelHeight; y++)
//         {
//             for (int x = 0; x < _bitmap.PixelWidth; x++)
//             {
//                 var c = (byte)(_depthMap[x / 20, y / 20] * 6);
//                 var color = Color.FromRgb(c, c, c);
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
//     int GetNeighborCount(int column, int row, int layer)
//     {
//         var count = 0;
//         var width = _field.GetLength(0) - 1;
//         var height = _field.GetLength(1) - 1;
//         var deep = _field.GetLength(2) - 1;
//         if (layer != deep && _field[column, row, layer + 1])
//         {
//             count++;
//         }
//         if (layer != 0 && _field[column, row, layer - 1])
//         {
//             count++;
//         }
//         if (row != height && _field[column, row + 1, layer])
//         {
//             count++;
//         }
//         if (row != height && layer != deep && _field[column, row + 1, layer + 1])
//         {
//             count++;
//         }
//         if (row != height && layer != 0 && _field[column, row + 1, layer - 1])
//         {
//             count++;
//         }
//         if (row != 0 && _field[column, row - 1, layer])
//         {
//             count++;
//         }
//         if (row != 0 && layer != deep && _field[column, row - 1, layer + 1])
//         {
//             count++;
//         }
//         if (row != 0 && layer != 0 && _field[column, row - 1, layer - 1])
//         {
//             count++;
//         }
//         if (column != width && _field[column + 1, row, layer])
//         {
//             count++;
//         }
//         if (column != width && layer != deep && _field[column + 1, row, layer + 1])
//         {
//             count++;
//         }
//         if (column != width && layer != 0 && _field[column + 1, row, layer - 1])
//         {
//             count++;
//         }
//         if (column != width && row != height && _field[column + 1, row + 1, layer])
//         {
//             count++;
//         }
//         if (column != width && row != height && layer != deep && _field[column + 1, row + 1, layer + 1])
//         {
//             count++;
//         }
//         if (column != width && row != height && layer != 0 && _field[column + 1, row + 1, layer - 1])
//         {
//             count++;
//         }
//         if (column != width && row != 0 && _field[column + 1, row - 1, layer])
//         {
//             count++;
//         }
//         if (column != width && row != 0 && layer != deep && _field[column + 1, row - 1, layer + 1])
//         {
//             count++;
//         }
//         if (column != width && row != 0 && layer != 0 && _field[column + 1, row - 1, layer - 1])
//         {
//             count++;
//         }
//         if (column != 0 && _field[column - 1, row, layer])
//         {
//             count++;
//         }
//         if (column != 0 && layer != deep && _field[column - 1, row, layer + 1])
//         {
//             count++;
//         }
//         if (column != 0 && layer != 0 && _field[column - 1, row, layer - 1])
//         {
//             count++;
//         }
//         if (column != 0 && row != height && _field[column - 1, row + 1, layer])
//         {
//             count++;
//         }
//         if (column != 0 && row != height && layer != deep && _field[column - 1, row + 1, layer + 1])
//         {
//             count++;
//         }
//         if (column != 0 && row != height && layer != 0 && _field[column - 1, row + 1, layer - 1])
//         {
//             count++;
//         }
//         if (column != 0 && row != 0 && _field[column - 1, row - 1, layer])
//         {
//             count++;
//         }
//         if (column != 0 && row != 0 && layer != deep && _field[column - 1, row - 1, layer + 1])
//         {
//             count++;
//         }
//         if (column != 0 && row != 0 && layer != 0 && _field[column - 1, row - 1, layer - 1])
//         {
//             count++;
//         }
//         return count;
//     }
//     void Evolution()
//     {
//         var newField = new bool[_field.GetLength(0), _field.GetLength(1), _field.GetLength(2)];
//         for (int layer = 0; layer < _field.GetLength(2) - 1; layer++)
//         {
//             for (int row = 0; row < _field.GetLength(1) - 1; row++)
//             {
//                 for (int column = 0; column < _field.GetLength(0) - 1; column++)
//                 {
//                     switch (_rules)
//                     {
//                         case Rules.Life:
//                             {
//                                 var count = GetNeighborCount(column, row, layer);
//                                 if (_field[column, row, layer])
//                                 {
//                                     if (count == 4 || count == 5)
//                                     {
//                                         Add(column, row, layer);
//                                     }
//                                 }
//                                 else
//                                 {
//                                     if (count == 5)
//                                     {
//                                         Add(column, row, layer);
//                                     }
//                                 }
//                                 break;
//                             }
//                         case Rules.HTree:
//                             {
//                                 if (!_field[column, row, layer])
//                                 {
//                                     var count = GetNeighborCount(column, row, layer);
//                                     if (count is 1)
//                                     {
//                                         newField[column, row, layer] = true;
//                                     }
//                                 }
//                                 else
//                                 {
//                                     newField[column, row, layer] = true;
//                                 }
//                                 break;
//                             }
//                         case Rules.Conway:
//                             {
//                                 var count = GetNeighborCount(column, row, layer);
//                                 if (_field[column, row, layer])
//                                 {
//                                     if (count is 7 or 9)
//                                     {
//                                         newField[column, row, layer] = true;
//                                     }
//                                 }
//                                 else
//                                 {
//                                     if (count is 5 or 9)
//                                     {
//                                         newField[column, row, layer] = true;
//                                     }
//                                 }
//                                 break;
//                             }
//                         case Rules.Seeds:
//                             {
//                                 var count = GetNeighborCount(column, row, layer);
//                                 if (!_field[column, row, layer])
//                                 {
//                                     if (count is 4)
//                                     {
//                                         newField[column, row, layer] = true;
//                                     }
//                                 }
//                                 break;
//                             }
//                     }
//                 }
//             }
//         }
//         _field = newField;
//     }
//     void ProcessInput(object? sender, KeyEventArgs args)
//     {
//         var k = args.Key;
//         switch (k)
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
//             case Key.W:
//                 if (_z < _field.GetLength(2) - 1)
//                     _z++;
//                 break;
//             case Key.S:
//                 if (_z > 0)
//                     _z--;
//                 break;
//             case Key.Space:
//                 _field[_x, _y, _z] = !_field[_x, _y, _z];
//                 break;

//             case Key.N:
//                 Evolution(_birth, _survival);
//                 break;
//             case Key.R:
//                 OnRandom(0.1);
//                 break;
//         }
//     }
//     void DrawField()
//     {
//         Console.CursorLeft = 0;
//         Console.CursorTop = 0;
//         for (int layer = 0; layer < _field.GetLength(2) - 1; layer++)
//         {
//             for (int row = 0; row < _field.GetLength(1) - 1; row++)
//             {
//                 for (int column = 0; column < _field.GetLength(0) - 1; column++)
//                 {
//                     if (_field[column, row, _z])
//                     {
//                         Console.Write('*');
//                     }
//                     else
//                     {
//                         Console.Write('.');
//                     }
//                 }
//                 Console.WriteLine();
//             }
//             Console.CursorLeft = 0;
//             Console.CursorTop += 2;
//         }
//         Console.CursorLeft = _x;
//         Console.CursorTop = _y;
//     }
//     void OnRandom(double probability)
//     {
//         var rng = new Random();
//         for (int layer = 0; layer < _field.GetLength(2) - 1; layer++)
//         {
//             for (int row = 0; row < _field.GetLength(1) - 1; row++)
//             {
//                 for (int column = 0; column < _field.GetLength(0) - 1; column++)
//                 {
//                     if (rng.NextDouble() < probability)
//                     {
//                         _field[column, row, layer] = true;
//                     }
//                 }
//             }
//         }
//     }
//     public void Evolution(int[] birth, int[] survival)
//     {
//         var width = _field.GetLength(0);
//         var height = _field.GetLength(1);
//         var depth = _field.GetLength(2);
//         var newField = new bool[width, height, depth];
//         for (int layer = 0; layer < depth; layer++)
//         {
//             for (int row = 0; row < width; row++)
//             {
//                 for (int column = 0; column < height; column++)
//                 {
//                     if (_field[column, row, layer])
//                     {
//                         if (survival.Contains(GetNeighborCount(column, row, layer)))
//                         {
//                             newField[column, row, layer] = true;
//                         }
//                         else
//                         {
//                             if (_depthMap[column, row] < layer)
//                             {
//                                 GetDepth(column, row);
//                             }
//                         }
//                     }
//                     else
//                     {
//                         if (birth.Contains(GetNeighborCount(column, row, layer)))
//                         {
//                             Add(column, row, layer);
//                             _depthMap[column, row] = _depthMap[column, row] > layer ? _depthMap[column, row] : layer;
//                         }
//                     }
//                 }
//             }

//         }
//         _field = newField;
//     }
// }
