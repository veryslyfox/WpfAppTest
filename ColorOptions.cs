namespace WpfApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
partial class MainWindow
{
    public Color Angle(Color color, int i, int j, double angle)
    {
        var r = color.R;
        var g = color.G;
        var b = color.B;
        var sin = Math.Sin(angle);
        var cos = Math.Cos(angle);
        var tg = sin / cos;
        var clr = (int)(i + j * tg);
        return FromRgb(r + clr, g + clr, b + clr);
    }
}