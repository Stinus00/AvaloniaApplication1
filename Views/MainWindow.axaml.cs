using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Vortice;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    private class Item
    {
        public required string path { get; set; }
        public required string type { get; set; }
        public string? duration { get; set; }
    }
    private List<Item> _mediaPaths = new List<Item>
    {
        new Item { path = Path.Combine(AppContext.BaseDirectory, "Assets", "images", "react-logo.png"), type = "image", duration = "asljd" }, // Broken
        new Item { path = Path.Combine(AppContext.BaseDirectory, "Assets", "videos", "test2.mp4"), type = "video" },
        new Item { path = Path.Combine(AppContext.BaseDirectory, "Assets", "images", "react-logo.png"), type = "image", duration = "2000" },
        new Item { path = Path.Combine(AppContext.BaseDirectory, "Assets", "images", "broken.png"), type = "image", duration = "2000" }, // Broken
        new Item { path = Path.Combine(AppContext.BaseDirectory, "Assets", "videos", "test1.mp4"), type = "video" },
        new Item { path = Path.Combine(AppContext.BaseDirectory, "Assets", "videos", "test3.mp4"), type = "video" }
    };

    private int _currentVideoIndex = 0;
    private readonly DispatcherTimer _imageTimer = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var player = MediaPlayer.Player ?? throw new InvalidOperationException("The media player is not initialized.");
        player.MediaPlaybackCompleted += (_, _) => ShowNextMedia();
        _imageTimer.Tick += (_, _) => ShowNextMedia();
        ShowMedia(_mediaPaths[_currentVideoIndex]);
    }

    private void ShowMedia(Item mediaPath)
    {
        try
        {
            Debug.WriteLine($"Attempting to show media: {mediaPath.path}");

            if (!File.Exists(mediaPath.path))
            {
                throw new FileNotFoundException($"Media file not found: {mediaPath.path}");
            }

            _imageTimer.Stop();
            if (mediaPath.type == "image")
            {
                ImageControl.Source = new Avalonia.Media.Imaging.Bitmap(mediaPath.path);
                ImageControl.IsVisible = true;
                MediaPlayer.IsVisible = false;
                if (int.TryParse(mediaPath.duration, out var duration))
                {
                    _imageTimer.Interval = TimeSpan.FromMilliseconds(duration);
                    _imageTimer.Start();
                }
                else
                {
                    throw new FormatException($"Invalid duration format for image: {mediaPath.duration}");
                }

                return;
            }

            ImageControl.IsVisible = false;
            MediaPlayer.IsVisible = true;
            MediaPlayer.Source = new UriSource(new Uri(mediaPath.path).AbsoluteUri);
            MediaPlayer.IsEnabled = false; // Disable user interaction with the media player
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error showing media: {ex.Message}");
            ShowNextMedia();
        }
    }

    private void ShowNextMedia()
    {
        _currentVideoIndex = (_currentVideoIndex + 1) % _mediaPaths.Count;
        ShowMedia(_mediaPaths[_currentVideoIndex]);
    }
}