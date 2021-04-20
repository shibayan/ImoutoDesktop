using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ImoutoDesktop.Controls;
using ImoutoDesktop.IO;

namespace ImoutoDesktop.Windows
{
    /// <summary>
    /// BalloonWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class BalloonWindow : Window
    {
        public BalloonWindow()
        {
            InitializeComponent();
        }
        
        private bool isDragging = false;
        private Point dragStartPosition;
        private Point prevMousePosition;

        public Context Context { get; set; }

        private Point _locationOffset = new Point();

        public Point LocationOffset
        {
            get { return _locationOffset; }
            set { _locationOffset = value; }
        }

        public Balloon Balloon
        {
            get { return (Balloon)GetValue(BalloonProperty); }
            set { SetValue(BalloonProperty, value); }
        }

        public static readonly DependencyProperty BalloonProperty =
            DependencyProperty.Register("Balloon", typeof(Balloon), typeof(BalloonWindow));

        public TextViewer TextViewer
        {
            get { return textViewer; }
        }

        private void BalloonWindow_Activated(object sender, EventArgs e)
        {
            textBox.Focus();
        }

        private void BalloonWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            dragStartPosition = PointToScreen(e.GetPosition(this));
            prevMousePosition = dragStartPosition;
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        private void BalloonWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
            if (isDragging)
            {
                _locationOffset.X += prevMousePosition.X - dragStartPosition.X;
                _locationOffset.Y += prevMousePosition.Y - dragStartPosition.Y;
            }
        }

        private void BalloonWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var position = PointToScreen(e.GetPosition(this));
            if (position == prevMousePosition)
            {
                return;
            }
            if (IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                Left = Left + position.X - prevMousePosition.X;
                Top = Top + position.Y - prevMousePosition.Y;
                prevMousePosition = position;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
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
            Context.ExecCommand(textBox.Text);
            textBox.Clear();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int index = Context.HistoryIndex;
            int count = Context.CommandHistory.Count;
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
    }
}
