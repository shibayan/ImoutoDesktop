using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ImoutoDesktop.IO;

namespace ImoutoDesktop.Windows
{
    /// <summary>
    /// ImoutoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ImoutoWindow : Window
    {
        public ImoutoWindow()
        {
            InitializeComponent();
        }

        private bool isInitialized = false;
        private Point prevMousePosition;

        public Context Context { get; set; }

        public BalloonWindow BalloonWindow { get; set; }

        public Surface Surface
        {
            get { return (Surface)GetValue(SurfaceProperty); }
            private set { SetValue(SurfacePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SurfacePropertyKey =
            DependencyProperty.RegisterReadOnly("Surface", typeof(Surface), typeof(ImoutoWindow), new PropertyMetadata());

        public static readonly DependencyProperty SurfaceProperty = SurfacePropertyKey.DependencyProperty;

        public void ChangeSurface(int id)
        {
            // 表示・非表示の切り替え
            if (id != -1)
            {
                Surface = Context.SurfaceLoader.Load(id);
                if (!isInitialized)
                {
                    Left = SystemParameters.WorkArea.Width - ((BitmapImage)Surface.Image).PixelWidth - 1;
                    isInitialized = true;
                }
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void ImoutoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BalloonWindow.SizeChanged += new SizeChangedEventHandler(BalloonWindow_SizeChanged);
        }

        private void ImoutoWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BalloonWindow.SizeChanged -= new SizeChangedEventHandler(BalloonWindow_SizeChanged);
        }

        private void ImoutoWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prevMousePosition = PointToScreen(e.GetPosition(this));
            CaptureMouse();
        }

        private void ImoutoWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
            // バルーンをアクティブにする
            BalloonWindow.Activate();
        }

        private void ImoutoWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var position = PointToScreen(e.GetPosition(this));
            if (position == prevMousePosition)
            {
                return;
            }
            if (IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                Left = Left + position.X - prevMousePosition.X;
                Top = SystemParameters.WorkArea.Height - ActualHeight - 1;
                prevMousePosition = position;
            }
        }

        private void ImoutoWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Top = SystemParameters.WorkArea.Height - e.NewSize.Height - 1;
        }

        private void ImoutoWindow_LocationChanged(object sender, EventArgs e)
        {
            if (BalloonWindow.IsVisible)
            {
                BalloonWindow.Left = Left - BalloonWindow.ActualWidth + BalloonWindow.LocationOffset.X;
                BalloonWindow.Top = Top + BalloonWindow.LocationOffset.Y;
            }
        }

        private void BalloonWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BalloonWindow.Left = Left - BalloonWindow.ActualWidth + BalloonWindow.LocationOffset.X;
            BalloonWindow.Top = Top + BalloonWindow.LocationOffset.Y;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Context.Close();
        }
    }
}
