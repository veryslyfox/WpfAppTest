using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;

namespace WpfApp1;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new();
    private readonly WriteableBitmap _bitmap;
    private readonly Random _rng = new();
    private readonly List<Particle> _particles = new();
    private static readonly double TimeStep = 1.0 / Stopwatch.Frequency;
    private double _time;
    
    public MainWindow()
    {
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.00001);
        _timer.Tick += Tick;
        _timer.Start();
        _time = Stopwatch.GetTimestamp() * TimeStep;
    }
    private void Tick(object? sender, EventArgs e)
    {
        ProcessLogic();
        Render();
    }

    private void ProcessLogic()
    {
        var added = 0;
        while (_particles.Count < 100000 && ++added <= 100)
        {
            _particles.Add(new() { 
                Position = new(400, 700), 
                Velocity = new(_rng.NextDouble() * 20 - 10, -(_rng.NextDouble() * 250 + 10)), 
                Color = Color.FromRgb((byte)_rng.Next(256), (byte)_rng.Next(256), (byte)_rng.Next(256)),
                Life = _rng.NextDouble() / 2 + 0.5 });
        }
        var time = Stopwatch.GetTimestamp() * TimeStep;
        var step = time - _time;
        _time = time;

        for (int i = 0; i < _particles.Count; i++)
        {
            var particle = _particles[i];
            particle.Velocity += new Vector2(40 * step, 9.8 * step);
            particle.Position += particle.Velocity * step;
            particle.Life -= step / 100;
            if (particle.Life < 0)
            {
                _particles.RemoveAt(i--);
            }
        }
    }

    private void SetPixel(int x, int y, in Color color)
    {
        if (x < 0 || x >= _bitmap.PixelWidth || y < 0 || y >= _bitmap.PixelHeight)
        {
            return;
        }

        // Cache pointers
        var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
        unsafe
        {
            *((int*)ptr) = (color.R << 16) | (color.G << 8) | color.B;
        }
    }

    private void AddPixel(int x, int y, in Color color)
    {
        if (x < 0 || x >= _bitmap.PixelWidth || y < 0 || y >= _bitmap.PixelHeight)
        {
            return;
        }

        // Cache pointers
        var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
        unsafe
        {
            var pixelPtr = (int*)ptr;
            *pixelPtr = (Saturate(color.R + ((*pixelPtr >> 16) & 255)) << 16) 
                | (Saturate(color.G + ((*pixelPtr >> 8) & 255)) << 8)
                | Saturate(color.B + (*pixelPtr & 255));
        }
    }

    private byte Saturate(int value) 
    => value >= 0 
        ? value <= 255 
            ? (byte)value 
            : (byte)255 
        : (byte)0;

    private void Render()
    {
        _bitmap.Lock();
        for (int y = 0; y < _bitmap.PixelHeight; y++)
        {
            // Fill with memory blits
            for (int x = 0; x < _bitmap.PixelWidth; x++)
            {
                var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                unsafe
                {
                    *((int*)ptr) = 0;
                }
            }
        }

        foreach (var particle in _particles)
        {
            var color = Color.FromRgb(
                (byte)Math.Round(particle.Color.R * particle.Life), 
                (byte)Math.Round(particle.Color.G * particle.Life), 
                (byte)Math.Round(particle.Color.B * particle.Life));
            var halfColor = Color.FromRgb(
                (byte)Math.Round(particle.Color.R * particle.Life / 2), 
                (byte)Math.Round(particle.Color.G * particle.Life / 2), 
                (byte)Math.Round(particle.Color.B * particle.Life / 2));
            AddPixel((int)particle.Position.X, (int)particle.Position.Y, color);
            AddPixel((int)particle.Position.X - 1, (int)particle.Position.Y, halfColor);
            AddPixel((int)particle.Position.X, (int)particle.Position.Y - 1, halfColor);
            AddPixel((int)particle.Position.X + 1, (int)particle.Position.Y, halfColor);
            AddPixel((int)particle.Position.X, (int)particle.Position.Y + 1, halfColor);
        }

        _bitmap.AddDirtyRect(new(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
        _bitmap.Unlock();
    }
}
