using System;
using System.Windows;
using System.Windows.Input;

using ImoutoDesktop.Models;

namespace ImoutoDesktop.Views
{
    /// <summary>
    /// CharacterWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CharacterWindow
    {
        public CharacterWindow()
        {
            InitializeComponent();
        }

        private bool _isInitialized;
        private Point _prevMousePosition;

        public CharacterContext Context { get; set; }

        public BalloonWindow BalloonWindow { get; set; }

        public Surface Surface
        {
            get => (Surface)GetValue(SurfaceProperty);
            private set => SetValue(SurfacePropertyKey, value);
        }

        private static readonly DependencyPropertyKey SurfacePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Surface), typeof(Surface), typeof(CharacterWindow), new PropertyMetadata());

        public static readonly DependencyProperty SurfaceProperty = SurfacePropertyKey.DependencyProperty;

        public void ChangeSurface(int id)
        {
            // 表示・非表示の切り替え
            if (id != -1)
            {
                Surface = Context.SurfaceLoader.Load(id);
                if (!_isInitialized)
                {
                    Left = SystemParameters.WorkArea.Width - Surface.Image.PixelWidth - 1;
                    _isInitialized = true;
                }
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void CharacterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BalloonWindow.SizeChanged += BalloonWindow_SizeChanged;
        }

        private void CharacterWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BalloonWindow.SizeChanged -= BalloonWindow_SizeChanged;
        }

        private void CharacterWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _prevMousePosition = PointToScreen(e.GetPosition(this));
            CaptureMouse();
        }

        private void CharacterWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            // バルーンをアクティブにする
            BalloonWindow.Activate();
        }

        private void CharacterWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var position = PointToScreen(e.GetPosition(this));

            if (position == _prevMousePosition)
            {
                return;
            }

            if (IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                Left = Left + position.X - _prevMousePosition.X;
                Top = SystemParameters.WorkArea.Height - ActualHeight - 1;
                _prevMousePosition = position;
            }
        }

        private void CharacterWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Top = SystemParameters.WorkArea.Height - e.NewSize.Height - 1;
        }

        private void CharacterWindow_LocationChanged(object sender, EventArgs e)
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
    }
}
