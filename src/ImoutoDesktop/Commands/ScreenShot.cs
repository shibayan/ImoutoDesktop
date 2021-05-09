using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public class ScreenShot : RemoteCommandBase
    {
        public ScreenShot(RemoteConnectionManager remoteConnectionManager)
            : base("スクリーンショット", remoteConnectionManager)
        {
        }

        protected override Task<CommandResult> ExecuteCore(string input)
        {
            try
            {
                var path = Path.Combine(Path.GetTempPath(), $@"temp\{Path.GetRandomFileName()}.png");
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

                return Task.FromResult(Succeeded($@"\_i[{path}]"));
            }
            catch
            {
                return Task.FromResult(Failed());
            }
        }
    }
}
