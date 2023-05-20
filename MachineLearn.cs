global using System.Windows;
global using System;
global using System.Collections.Generic;
static class MachineLearn
{
    static (Point, int)[] ToPivots(int[,] image, int detalization, int imageMax, out List<int> interpolateData)
    {
        List<(Point, int)> result = new List<(Point, int)>();
        List<int> outResult = new List<int>();
        int Interpolate(int pivotValue)
        {
            var c = (pivotValue + 0.0) / detalization;
            return (int)(2 * c + imageMax * (1 - c));
        }
        for (int i = 0; i <= detalization; i++)
        {
            var point = Interpolate(i);
            outResult.Add(point);
            for (int row = 0; row < image.GetLength(1); row++)
            {
                for (int column = 0; column < image.GetLength(0); column++)
                {
                    if (image[column, row] == point)
                    {
                        result.Add((new(column, row), i));
                    }
                }
            }
        }
        interpolateData = outResult;
        return result.ToArray();
    }
    // static Line[] Linearize((Point, int)[] dots, int max, List<int> interpolateData)
    // {
    //     foreach (var item in interpolateData)
    //     {
            
    //     }
    // }
    public static int[,] Number(bool[,] image, int x, int y, out int imageMax)
    {
        int[,] result = new int[image.GetLength(0) + 2, image.GetLength(1) + 2];
        for (int row = 1; row < result.GetLength(1); row++)
        {
            for (int column = 1; column < result.GetLength(0); column++)
            {
                result[row, column] = image[column, row] ? 1 : 0;
            }
        }
        result[x, y] = 2;
        while (true)
        {
            var isFilled = true;
            var imageMaxValue = 0;
            for (int row = 0; row < result.GetLength(1); row++)
            {
                for (int column = 0; column < result.GetLength(0); column++)
                {
                    isFilled &= result[column, row] != 1;
                    imageMaxValue = Math.Max(imageMaxValue, result[column, row]);
                }
            }
            if (isFilled)
            {
                imageMax = imageMaxValue;
                return result;
            }
            for (int row = 0; row < result.GetLength(1); row++)
            {
                for (int column = 0; column < result.GetLength(0); column++)
                {
                    var max = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            max = Math.Max(result[column + i, row + j], max);
                        }
                    }
                    if (max > 1)
                    {
                        result[column, row] = max + 1;
                    }
                }
            }
        }
    }

}
struct Line
{
    public Line(int xStart, int yStart, int xEnd, int yEnd)
    {
        XStart = xStart;
        YStart = yStart;
        XEnd = xEnd;
        YEnd = yEnd;
        Angle = Math.Atan2(yEnd - yStart, xEnd - xStart);
    }

    public int XStart { get; }
    public int YStart { get; }
    public int XEnd { get; }
    public int YEnd { get; }
    public double Angle { get; }
}