using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ImoutoDesktop.Controls;
using ImoutoDesktop.Models;

namespace ImoutoDesktop.Windows
{
    /// <summary>
    /// BalloonWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class BalloonWindow
    {
        public BalloonWindow()
        {
            InitializeComponent();
        }

        private bool _isDragging;
        private Point _dragStartPosition;
        private Point _prevMousePosition;

        public CharacterContext Context { get; set; }

        public Point LocationOffset { get; set; }

        public Balloon Balloon
        {
            get { return (Balloon)GetValue(BalloonProperty); }
            set { SetValue(BalloonProperty, value); }
        }

        public static readonly DependencyProperty BalloonProperty =
            DependencyProperty.Register(nameof(Balloon), typeof(Balloon), typeof(BalloonWindow));

        public TextViewer TextViewer => textViewer;

        private void BalloonWindow_Activated(object sender, EventArgs e)
        {
            textBox.Focus();
        }

        private void BalloonWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _dragStartPosition = PointToScreen(e.GetPosition(this));
            _prevMousePosition = _dragStartPosition;

            Mouse.Capture(this, CaptureMode.SubTree);
        }

        private void BalloonWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            if (_isDragging)
            {
                LocationOffset.Offset(_prevMousePosition.X - _dragStartPosition.X, _prevMousePosition.Y - _dragStartPosition.Y);
            }
        }

        private void BalloonWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var position = PointToScreen(e.GetPosition(this));

            if (position == _prevMousePosition)
            {
                return;
            }

            if (IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                Left = Left + position.X - _prevMousePosition.X;
                Top = Top + position.Y - _prevMousePosition.Y;
                _prevMousePosition = position;
            }
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            if (textBox.Text.Length == 0)
            {
                return;
            }

            if (Context.CommandHistory.IndexOf(textBox.Text) != -1)
            {
                Context.CommandHistory.Remove(textBox.Text);
            }

            Context.CommandHistory.Add(textBox.Text);
            Context.HistoryIndex = Context.CommandHistory.Count;
            await Context.ExecCommand(textBox.Text);
            textBox.Clear();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var index = Context.HistoryIndex;
            var count = Context.CommandHistory.Count;
            if (count == 0)
            {
                return;
            }
            var textBox = (TextBox)sender;

            if (e.Key == Key.Up)
            {
                if (index <= 0)
                {
                    index = count;
                    textBox.Text = string.Empty;
                }
                else
                {
                    --index;
                    textBox.Text = Context.CommandHistory[index];
                    textBox.SelectAll();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Down)
            {
                ++index;
                if (index > count)
                {
                    index = 0;
                }
                else if (index == count)
                {
                    textBox.Text = string.Empty;
                    Context.HistoryIndex = index;
                    return;
                }
                textBox.Text = Context.CommandHistory[index];
                textBox.SelectAll();
                e.Handled = true;
            }

            Context.HistoryIndex = index;
        }

        private void BaseImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Width = baseImage.ActualWidth;
            Height = baseImage.ActualHeight;
        }
    }
}
