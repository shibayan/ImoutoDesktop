using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImoutoDesktop.Commands
{
    public class ScreenShot : CommandBase
    {
        public ScreenShot()
            : base("スクリーンショット")
        {
        }

        public override bool PreExecute(string input)
        {
            return true;
        }

        public override bool Execute(string input, out string result)
        {
            try
            {
                var path = Path.Combine(((App)Application.Current).RootDirectory, $@"temp\{Path.GetRandomFileName()}.png");
                var stream = ConnectionPool.Connection.GetScreenshot(480);

                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.EndInit();

                using (var fileStream = File.OpenWrite(path))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(fileStream);
                }

                result = $@"\_i[{path}]";

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
