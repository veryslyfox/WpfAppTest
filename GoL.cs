/*using System;
using System.IO;
using System.Windows.Input;
using Objects.Data;

namespace WpfApp1;
partial class MainWindow
{
    static uint[,] _field = new uint[40, 40];
    static int _x, _y;
    static Rules _rules;

    enum Rules
    {
        Conway,
        Avgust,
        Ulam,
        HighLife,
        AvgustConway,
        LifeWithoutDead,
        DayAndNight,
        Avgust2,
        Wind,
        StableLife,
        NotStableLife,
        HardLife,
        Replace
    }

    void Main()
    {
        const int FieldWidth = 40;
        const int FieldHeight = 40;
        _field = new uint[FieldWidth, FieldHeight];
        for (int i = 0; i < FieldWidth; i++)
        {
            for (int j = 0; j < FieldHeight; j++)
            {
                _field[i, j] = 0;
            }
        }
        _x = 20;
        _y = 20;
        _rules = Rules.Avgust;
        KeyDown += ProcessInput;
    }
    void ProcessInput(object sender, KeyEventArgs args)
    {
        var k = args;
        switch (k.Key)
        {
            case Key.Left:
                if (_x > 0)
                    _x--;
                break;

            case Key.Right:
                if (_x < _field.GetLength(0) - 1)
                    _x++;
                break;

            case Key.Up:
                if (_y > 0)
                    _y--;
                break;

            case Key.Down:
                if (_y < _field.GetLength(1) - 1)
                    _y++;
                break;

            case Key.Space:
                if (_rules != Rules.HardLife)
                    _field[_x, _y] = _field[_x, _y] == 0 ? 1u : 0u;
                else
                {
                    if (_field[_x, _y] != 4)
                        _field[_x, _y]++;
                    else
                        _field[_x, _y] = 0;
                }
                break;

            case Key.N:
                Evolution();
                break;

            case Key.S:
                Save();
                break;

            case Key.L:
                Load();
                break;

            case Key.C:
                Array.Clear(_field, 0, _field.GetLength(0) * _field.GetLength(1));
                break;
            
            case Key.I:
                for (int row = 0; row < _field.GetLength(1); row++)
                {
                    for (int column = 0; column < _field.GetLength(0); column++)
                    {
                        _field[column, row] = (uint)(_field[column, row] != 0 ? 0 : 1);
                    }
                }
                break;
        }
    }
    private static void Save()
    {
        Console.Clear();
        Console.Write("Введите название: ");
        var name = Console.ReadLine();
        var fileName = $"{name}.field";
        using (var file = File.Open(fileName, FileMode.Create))
        {
            var writer = new BinaryWriter(file);
            writer.Write(_field.GetLength(0));
            writer.Write(_field.GetLength(1));
            for (int row = 0; row < _field.GetLength(1); row++)
            {
                for (int column = 0; column < _field.GetLength(0); column++)
                {
                    writer.Write(_field[column, row]);
                }
            }
        }
    }

    private static void Load()
    {
        Console.Clear();
        Console.Write("Введите название: ");
        var name = Console.ReadLine();
        var fileName = $"{name}.field";
        if (!File.Exists(fileName))
        {
            return;
        }

        using (var file = File.Open(fileName, FileMode.Open))
        {
            var reader = new BinaryReader(file);
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var field = new uint[width, height];
            for (int row = 0; row < field.GetLength(1); row++)
            {
                for (int column = 0; column < field.GetLength(0); column++)
                {
                    field[column, row] = reader.ReadUInt32();
                }
            }
            _field = field;
        }
    }

    static int GetNeighborCount(int column, int row)
    {
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var count = 0;
        if (row > 0 && column > 0 && _field[column - 1, row - 1] != 0)
        {
            count++;
        }

        if (row > 0 && _field[column, row - 1] != 0)
        {
            count++;
        }

        if (row > 0 && column < width - 1 && _field[column + 1, row - 1] != 0)
        {
            count++;
        }

        if (column < width - 1 && _field[column + 1, row] != 0)
        {
            count++;
        }

        if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] != 0)
        {
            count++;
        }

        if (row < height - 1 && _field[column, row + 1] != 0)
        {
            count++;
        }

        if (row < height - 1 && column > 0 && _field[column - 1, row + 1] != 0)
        {
            count++;
        }

        if (column > 0 && _field[column - 1, row] != 0)
        {
            count++;
        }

        return count;
    }
    static int GetNeighborCount(int column, int row, int cell)
    {
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var count = 0;
        if (row > 0 && column > 0 && _field[column - 1, row - 1] == cell)
        {
            count++;
        }

        if (row > 0 && _field[column, row - 1] == cell)
        {
            count++;
        }

        if (row > 0 && column < width - 1 && _field[column + 1, row - 1] == cell)
        {
            count++;
        }

        if (column < width - 1 && _field[column + 1, row] == cell)
        {
            count++;
        }

        if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] == cell)
        {
            count++;
        }

        if (row < height - 1 && _field[column, row + 1] == cell)
        {
            count++;
        }

        if (row < height - 1 && column > 0 && _field[column - 1, row + 1] == cell)
        {
            count++;
        }

        if (column > 0 && _field[column - 1, row] == cell)
        {
            count++;
        }

        return count;
    }
    static int GetHexNeighborCount(int column, int row)
    {
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var count = 0;
        if (row > 0 && column > 0 && _field[column - 1, row - 1] != 0)
        {
            count++;
        }

        if (row > 0 && _field[column, row - 1] != 0)
        {
            count++;
        }

        if (column < width - 1 && _field[column + 1, row] != 0)
        {
            count++;
        }

        if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] != 0)
        {
            count++;
        }

        if (row < height - 1 && _field[column, row + 1] != 0)
        {
            count++;
        }

        if (column > 0 && _field[column - 1, row] != 0)
        {
            count++;
        }

        return count;
    }

    static int GetNeighborCount2(int column, int row)
    {
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var count = 0;
        if (row > 0 && _field[column, row - 1] != 0)
        {
            count++;
        }

        if (column < width - 1 && _field[column + 1, row] != 0)
        {
            count++;
        }

        if (row < height - 1 && _field[column, row + 1] != 0)
        {
            count++;
        }


        if (column > 0 && _field[column - 1, row] != 0)
        {
            count++;
        }

        return count;
    }
    static int NeighborAsGreenCell(int column, int row)
    {
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var count = 0;
        if (row > 0 && column > 0 && _field[column - 1, row - 1] == 3)
        {
            _field[column - 1, row - 1] = 4;
        }

        if (row > 0 && _field[column, row - 1] == 3)
        {
            _field[column, row - 1] = 4;
        }

        if (row > 0 && column < width - 1 && _field[column + 1, row - 1] == 3)
        {
            _field[column + 1, row - 1] = 4;
        }

        if (column < width - 1 && _field[column + 1, row] == 3)
        {
            _field[column + 1, row] = 4;
        }

        if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] == 3)
        {
            _field[column + 1, row + 1] = 4;
        }

        if (row < height - 1 && _field[column, row + 1] == 3)
        {
            _field[column, row + 1] = 4;
        }

        if (row < height - 1 && column > 0 && _field[column - 1, row + 1] == 3)
        {
            _field[column - 1, row + 1] = 4;
        }

        if (column > 0 && _field[column - 1, row] == 3)
        {
            _field[column - 1, row] = 4;
        }

        return count;
    }
    static void Evolution()
    {
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var newField = new uint[width, height];
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                switch (_rules)
                {
                    case Rules.Conway:
                        {
                            var count = GetNeighborCount(column, row);
                            if (_field[column, row] != 0)
                            {
                                if (count == 2 || count == 3)
                                {
                                    newField[column, row] = _field[column, row] + 1;
                                }
                            }
                            else
                            {
                                if (count == 3)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;
                        }

                    case Rules.Avgust:
                        {
                            var count = GetNeighborCount(column, row);
                            if (_field[column, row] != 0)
                            {
                            }
                            else
                            {
                                if (count == 2)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;
                        }

                    case Rules.Ulam:
                        {
                            var count = GetNeighborCount2(column, row);
                            if (_field[column, row] != 0)
                            {
                                if (_field[column, row] < 3)
                                {
                                    _field[column, row] += 1;
                                }
                            }
                            else
                            {
                                if (count == 1)
                                {
                                    newField[column, row] = _field[column, row] + 1;
                                }
                            }
                            break;
                        }
                    case Rules.HighLife:
                        {
                            var count = GetNeighborCount(column, row);

                            if (_field[column, row] != 0)
                            {
                                if (count == 2 || count == 3)
                                {
                                    newField[column, row] = _field[column, row] + 1;
                                }
                            }
                            else
                            {
                                if (count is 3 or 6)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;
                        }
                    case Rules.AvgustConway:
                        {
                            int count = GetNeighborCount(column, row);
                            if (count == 2)
                            {
                                newField[column, row] = 1;
                            }
                            if (_field[column, row] != 0)
                            {
                                if (count == 2 || count == 3)
                                {
                                    newField[column, row] = _field[column, row] + 1;
                                }
                            }
                            break;
                        }
                    case Rules.LifeWithoutDead:
                        {
                            var count = GetNeighborCount(column, row);
                            if (_field[column, row] != 0)
                            {
                                newField[column, row] = _field[column, row] + 1;
                            }
                            else
                            {
                                if (count == 3)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;
                        }
                    case Rules.DayAndNight:
                        {
                            var count = GetNeighborCount(column, row);
                            if (_field[column, row] != 0)
                            {
                                if (count is 3 or 4 or 6 or 7 or 8)
                                {
                                    newField[column, row] = _field[column, row] + 1;
                                }
                            }
                            else
                            {
                                if (count is 3 or 6 or 7 or 8)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;
                        }
                    case Rules.Avgust2:
                        {
                            var count = GetNeighborCount(column, row);
                            if (_field[column, row] != 0)
                            {
                            }
                            else
                            {
                                if (count == 2 || count == 4)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;
                        }
                    case Rules.Wind:
                        {
                            var count = GetNeighborCount(column, row);
                            if (_field[column, row] != 0)
                            {
                            }
                            else
                            {
                                if (count is 3 || column != 0 && _field[column - 1, row] != 0)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;
                        }
                    case Rules.StableLife:
                        {
                            var count = GetNeighborCount(column, row);
                            if (_field[column, row] != 0)
                            {
                                if (count is 2 or 3 or 4 or 8)
                                {
                                    newField[column, row] = _field[column, row] + 1;
                                }
                            }
                            else
                            {
                                if (count == 3)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;

                        }
                    case Rules.NotStableLife:
                        {
                            var count = GetNeighborCount(column, row);
                            if (_field[column, row] != 0)
                            {
                                if (count is 2 or 4 or 5)
                                {
                                    newField[column, row] = _field[column, row] + 1;
                                }
                            }
                            else
                            {
                                if (count is 3 or 5)
                                {
                                    newField[column, row] = 1;
                                }
                            }
                            break;
                        }
                    case Rules.Replace:
                    {
                        _field[column, row] = ((uint)(GetNeighborCount(column, row) % 2));
                        break;
                    }
                    case Rules.HardLife:
                        {
                            int NeighborCount(int cell)
                            {
                                return GetNeighborCount(column, row, cell);
                            }
                            
                            if (_field[column, row] == 3)
                            {
                                var leftAndRightExpand = true;
                                if (column != 0 && _field[column - 1, row] == 0)
                                {
                                    _field[column - 1, row] = 3;
                                }
                                else
                                {
                                    leftAndRightExpand = false;
                                }
                                if (column != _field.GetLength(0) && _field[column + 1, row] == 0)
                                {
                                    newField[column + 1, row] = 3;
                                }
                                else
                                {
                                    leftAndRightExpand = false;
                                }
                                var upExpand = true;
                                if (leftAndRightExpand && row != 0 && _field[column, row - 1] == 0)
                                {
                                    newField[column, row - 1] = 3;
                                }
                                else
                                {
                                    upExpand = false;
                                }
                                if (!upExpand && row != _field.GetLength(1) && _field[column, row + 1] == 0)
                                {
                                    newField[column, row + 1] = 3;
                                }
                            }
                            if (_field[column, row] == 4)
                            {
                                NeighborAsGreenCell(column, row);
                                if (NeighborCount(4) == 0)
                                {
                                    newField[column, row] = 0;
                                }
                            }
                            if (_field[column, row] == 1)
                            {
                                if (NeighborCount(2) > 2)
                                {
                                    newField[column, row] = 0;
                                }
                            }
                            break;
                        }
                    
                }

            }
        }

        _field = newField;
    }

    void DrawField()
    {
        
    }

    private bool[,] GetBitmap()
    {
        var result = new bool[_field.GetLength(0), _field.GetLength(1)];
        for (int i = 0; i < _field.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < _field.GetLength(1) - 1; j++)
            {
                result[i, j] = _field[i, j] > 0;
            }
        }   
        return result;
    }

    static void DrawField2()
    {
        Console.CursorLeft = 0;
        Console.CursorTop = 0;
        for (int row = 0; row < _field.GetLength(1); row++)
        {
            for (int column = 0; column < _field.GetLength(0); column++)
            {
                char print = '.';
                switch (_field[column, row])
                {
                    case 0:
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case 6:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
                Console.Write(print);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
        }
        Console.CursorLeft = _x;
        Console.CursorTop = _y;
    }
    static void OnRandom(double probability)
    {
        var rng = new Random();
        for (int row = 0; row < _field.GetLength(1); row++)
        {
            for (int column = 0; column < _field.GetLength(0); column++)
            {
                if (rng.NextDouble() < probability)
                {
                    _field[column, row] = 1;
                }
            }
        }
    }
    static void Line(int iBegin, int jBegin, int length)
    {
        for (int i = 0; i < length; i++)
        {
            _field[iBegin + i, jBegin] = 1;
        }
    }
}*/