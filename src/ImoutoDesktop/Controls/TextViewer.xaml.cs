using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImoutoDesktop.Controls
{
    /// <summary>
    /// TextViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class TextViewer : UserControl
    {
        public TextViewer()
        {
            InitializeComponent();
        }

        public bool CanScrollUp
        {
            get { return (bool)GetValue(CanScrollUpProperty); }
            private set { SetValue(CanScrollUpPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey CanScrollUpPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanScrollUp), typeof(bool), typeof(TextViewer), new PropertyMetadata(false));

        public static readonly DependencyProperty CanScrollUpProperty = CanScrollUpPropertyKey.DependencyProperty;

        public bool CanScrollDown
        {
            get { return (bool)GetValue(CanScrollDownProperty); }
            private set { SetValue(CanScrollDownPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey CanScrollDownPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanScrollDown), typeof(bool), typeof(TextViewer), new PropertyMetadata(false));

        public static readonly DependencyProperty CanScrollDownProperty = CanScrollDownPropertyKey.DependencyProperty;

        private Brush _currentFontColor;

        public Brush CurrentFontColor
        {
            set { _currentFontColor = value; _run = null; }
        }

        private double? _currentFontSize;

        public double? CurrentFontSize
        {
            set { _currentFontSize = value; _run = null; }
        }

        private FontFamily _currentFontFamily;

        public FontFamily CurrentFontFamily
        {
            set { _currentFontFamily = value; _run = null; }
        }

        private FontWeight? _currentFontWeight;

        public FontWeight? CurrentFontWeight
        {
            set { _currentFontWeight = value; _run = null; }
        }

        private Run _run;
        private Paragraph _paragraph;

        public void AddChar(char c)
        {
            if (_run == null)
            {
                _run = CreateRun();
                _paragraph.Inlines.Add(_run);
            }
            _run.Text += c;
            richTextBox.ScrollToEnd();
        }

        public void AddLine(string line)
        {
            var run = CreateRun();
            run.Text = line;
            _paragraph.Inlines.Add(run);
            richTextBox.ScrollToEnd();
        }

        /// <summary>
        /// 画像を追加する。
        /// </summary>
        /// <param name="uri">追加する画像の URI。</param>
        public void AddImage(Uri uri)
        {
            var bitmap = new BitmapImage(uri);

            var binding = new Binding
            {
                Path = new PropertyPath("ActualWidth"),
                Source = richTextBox
            };

            var image = new Image();

            image.BeginInit();
            image.SetBinding(Image.WidthProperty, binding);
            image.StretchDirection = StretchDirection.DownOnly;
            image.Source = bitmap;
            image.EndInit();

            _paragraph.Inlines.Add(image);

            // スクロール
            richTextBox.ScrollToEnd();

            // Run をリセット
            _run = null;
        }

        /// <summary>
        /// サイズを指定して、画像を追加する。
        /// </summary>
        /// <param name="uri">追加する画像の URI。</param>
        /// <param name="width">画像の幅。</param>
        /// <param name="height">画像の高さ。</param>
        public void AddImage(Uri uri, double width, double height)
        {
            var bitmap = new BitmapImage(uri);

            var image = new Image();

            image.BeginInit();
            image.Width = width;
            image.Height = height;
            image.Source = bitmap;
            image.EndInit();

            _paragraph.Inlines.Add(image);

            // スクロール
            richTextBox.ScrollToEnd();

            // Run をリセット
            _run = null;
        }

        public void AddBlock()
        {
            _paragraph = new Paragraph { Margin = new Thickness(0) };
            richTextBox.Document.Blocks.Add(_paragraph);
        }

        public void LineBreak()
        {
            _paragraph.Inlines.Add(new LineBreak());
        }

        public void Clear()
        {
            _run = null;
            _paragraph = null;
            richTextBox.Document.Blocks.Clear();
        }

        private Run CreateRun()
        {
            var run = new Run();

            if (_currentFontSize.HasValue)
            {
                run.FontSize = _currentFontSize.Value;
            }

            if (_currentFontFamily != null)
            {
                run.FontFamily = _currentFontFamily;
            }

            if (_currentFontColor != null)
            {
                run.Foreground = _currentFontColor;
            }

            if (_currentFontWeight.HasValue)
            {
                run.FontWeight = _currentFontWeight.Value;
            }

            return run;
        }

        private void RichTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CanScrollUp = e.VerticalOffset != 0;
            CanScrollDown = richTextBox.VerticalOffset != (richTextBox.ViewportHeight - richTextBox.ExtentHeight);
        }

        private void LineUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            richTextBox.LineUp();
        }

        private void LineDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            richTextBox.LineDown();
        }
    }
}
