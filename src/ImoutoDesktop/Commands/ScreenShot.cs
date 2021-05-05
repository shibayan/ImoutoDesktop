using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using ImoutoDesktop.Remoting;
using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public class ScreenShot : CommandBase
    {
        public ScreenShot(RemoteConnectionManager remoteConnectionManager)
            : base("スクリーンショット", remoteConnectionManager)
        {
        }

        public override async Task<CommandResult> Execute(string input)
        {
            try
            {
                var path = Path.Combine(((App)Application.Current).RootDirectory, $@"temp\{Path.GetRandomFileName()}.png");
                //var stream = ConnectionPool.Connection.GetScreenshot(480);

                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                //bitmap.StreamSource = stream;
                bitmap.EndInit();

                using (var fileStream = File.OpenWrite(path))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(fileStream);
                }

                return Succeeded($@"\_i[{path}]");
            }
            catch
            {
                return Failed();
            }
        }
    }
}
