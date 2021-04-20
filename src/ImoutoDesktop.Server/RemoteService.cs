namespace ImoutoDesktop.Server
{
    //public class RemoteService : IRemoteService
    //{
    //    private bool _isLogined;

    //    public bool Login(string password)
    //    {
    //        // ログインする
    //        var md5 = new MD5CryptoServiceProvider();
    //        var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(Settings.Default.Password));
    //        var masterPassword = BitConverter.ToString(hash).Replace("-", "").ToLower();
    //        if (masterPassword == password)
    //        {
    //            _isLogined = true;
    //            return true;
    //        }
    //        return false;
    //    }

    //    public bool IsConnecting
    //    {
    //        get { return true; }
    //    }

    //    private object _syncLock = new();

    //    public FileStream OpenFile(string path, FileMode mode)
    //    {
    //        if (!_isLogined)
    //        {
    //            return null;
    //        }
    //        try
    //        {
    //            return File.Open(path, mode);
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    public string[] GetFiles(string path, string searchPattern)
    //    {
    //        if (!_isLogined)
    //        {
    //            return null;
    //        }
    //        return Directory.GetFiles(path, searchPattern);
    //    }

    //    public string[] GetDirectories(string path, string searchPattern)
    //    {
    //        if (!_isLogined)
    //        {
    //            return null;
    //        }
    //        return Directory.GetDirectories(path, searchPattern);
    //    }

    //    public bool DeleteFile(string path)
    //    {
    //        if (!_isLogined)
    //        {
    //            return false;
    //        }
    //        try
    //        {
    //            if (File.Exists(path))
    //            {
    //                File.Delete(path);
    //                return true;
    //            }
    //            return false;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    public bool CopyFile(string sourcePath, string destPath)
    //    {
    //        if (!_isLogined)
    //        {
    //            return false;
    //        }

    //        try
    //        {
    //            File.Copy(sourcePath, destPath);
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    public bool MoveFile(string sourcePath, string destPath)
    //    {
    //        if (!_isLogined)
    //        {
    //            return false;
    //        }

    //        try
    //        {
    //            File.Move(sourcePath, destPath);
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    public string GetFolderPath(Environment.SpecialFolder folder)
    //    {
    //        if (!_isLogined)
    //        {
    //            return null;
    //        }

    //        return Environment.GetFolderPath(folder);
    //    }

    //    private delegate object RemoteInvoker();

    //    public string ExecuteCommand(string command)
    //    {
    //        if (!_isLogined)
    //        {
    //            return null;
    //        }

    //        return (string)Form1.Form.Invoke((RemoteInvoker)delegate
    //        {
    //            var psi = new ProcessStartInfo
    //            {
    //                FileName = Environment.GetEnvironmentVariable("ComSpec"),
    //                RedirectStandardInput = false,
    //                RedirectStandardOutput = true,
    //                UseShellExecute = false,
    //                CreateNoWindow = true,
    //                WorkingDirectory = Environment.CurrentDirectory,
    //                Arguments = $"/c {command}"
    //            };

    //            var process = Process.Start(psi);
    //            var result = process.StandardOutput.ReadToEnd();

    //            process.WaitForExit();

    //            return result;
    //        });
    //    }

    //    private Dictionary<string, Process> _processes = new();

    //    public bool ExecuteProcess(string fileName, string argument)
    //    {
    //        if (!_isLogined)
    //        {
    //            return false;
    //        }
    //        try
    //        {
    //            var process = (Process)Form1.Form.Invoke((RemoteInvoker)(() => Process.Start(fileName, argument)));

    //            lock (_syncLock)
    //            {
    //                if (process != null)
    //                {
    //                    process.EnableRaisingEvents = true;
    //                    process.Exited += Process_Exited;
    //                    process.SynchronizingObject = Form1.Form;
    //                    if (!_processes.ContainsKey(fileName))
    //                    {
    //                        _processes.Add(fileName, process);
    //                    }
    //                }
    //            }

    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    private void Process_Exited(object sender, EventArgs e)
    //    {
    //        lock (_syncLock)
    //        {
    //            string key = null;
    //            foreach (var item in _processes)
    //            {
    //                if (item.Value == (Process)sender)
    //                {
    //                    key = item.Key;
    //                    break;
    //                }
    //            }
    //            if (string.IsNullOrEmpty(key))
    //            {
    //                return;
    //            }
    //            _processes.Remove(key);
    //        }
    //    }

    //    public bool CloseProcess(string name)
    //    {
    //        if (!_isLogined)
    //        {
    //            return false;
    //        }
    //        lock (_syncLock)
    //        {
    //            foreach (var item in _processes)
    //            {
    //                try
    //                {
    //                    if (item.Value.HasExited)
    //                    {
    //                        continue;
    //                    }
    //                }
    //                catch
    //                {
    //                    continue;
    //                }
    //                if (item.Value.ProcessName == name || item.Key == name)
    //                {
    //                    if (!item.Value.CloseMainWindow())
    //                    {
    //                        item.Value.Kill();
    //                    }
    //                    return true;
    //                }
    //            }
    //            var result = false;
    //            foreach (var item in Process.GetProcessesByName(name))
    //            {
    //                item.Kill();
    //                result = true;
    //            }
    //            return result;
    //        }
    //    }

    //    public string CurrentDirectory
    //    {
    //        get
    //        {
    //            if (!_isLogined)
    //            {
    //                return null;
    //            }
    //            return Environment.CurrentDirectory;
    //        }
    //        set
    //        {
    //            if (!_isLogined)
    //            {
    //                return;
    //            }
    //            Environment.CurrentDirectory = value;
    //        }
    //    }

    //    public void Shutdown()
    //    {
    //        if (!_isLogined)
    //        {
    //            return;
    //        }
    //        NativeMethods.ExitWindows(NativeMethods.Shutdown);
    //    }

    //    public Exists Exists(string path)
    //    {
    //        if (!_isLogined)
    //        {
    //            return Remoting.Exists.None;
    //        }

    //        if (File.Exists(path))
    //        {
    //            return Remoting.Exists.File;
    //        }

    //        if (Directory.Exists(path))
    //        {
    //            return Remoting.Exists.Directory;
    //        }

    //        return Remoting.Exists.None;
    //    }

    //    public Stream GetScreenshot(int width)
    //    {
    //        if (!_isLogined)
    //        {
    //            return null;
    //        }
    //        var rate = (double)Screen.PrimaryScreen.Bounds.Height / (double)Screen.PrimaryScreen.Bounds.Width;
    //        var temp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
    //        var g = Graphics.FromImage(temp);
    //        g.CopyFromScreen(new Point(0, 0), new Point(0, 0), temp.Size);
    //        g.Dispose();
    //        var bitmap = new Bitmap(width, (int)(rate * width));
    //        g = Graphics.FromImage(bitmap);
    //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
    //        g.DrawImage(temp, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, temp.Width, temp.Height, GraphicsUnit.Pixel);
    //        g.Dispose();
    //        temp.Dispose();
    //        var stream = new MemoryStream();
    //        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
    //        return stream;
    //    }

    //    private static readonly Dictionary<string, DirectoryType> ext2type = new()
    //    {
    //        { ".bmp", DirectoryType.Picture }, { ".png", DirectoryType.Picture },
    //        { ".gif", DirectoryType.Picture }, { ".jpg", DirectoryType.Picture },
    //        { ".jpeg", DirectoryType.Picture },
    //        { ".mpg", DirectoryType.Movie }, { ".mpeg", DirectoryType.Movie },
    //        { ".flv", DirectoryType.Movie }, { ".avi", DirectoryType.Movie },
    //        { ".wmv", DirectoryType.Movie },
    //        { ".mp3", DirectoryType.Music }, { ".m4a", DirectoryType.Music },
    //        { ".m4p", DirectoryType.Music }, { ".ogg", DirectoryType.Music },
    //        { ".wav", DirectoryType.Music }, { ".mid", DirectoryType.Music },
    //        { ".wma", DirectoryType.Music },
    //        { ".txt", DirectoryType.Document }, { ".rtf", DirectoryType.Document },
    //        { ".doc", DirectoryType.Document }, { ".docx", DirectoryType.Document }
    //    };

    //    public DirectoryType GetDirectoryType(string directory)
    //    {
    //        if (!_isLogined)
    //        {
    //            return DirectoryType.None;
    //        }

    //        if (!Directory.Exists(directory))
    //        {
    //            return DirectoryType.None;
    //        }

    //        var count = 0;
    //        var filecount = 0;
    //        var types = new Dictionary<DirectoryType, int>();

    //        foreach (var item in Directory.GetFiles(directory))
    //        {
    //            if (ext2type.TryGetValue(Path.GetExtension(item).ToLower(), out var type))
    //            {
    //                if (!types.ContainsKey(type))
    //                {
    //                    types.Add(type, 1);
    //                }
    //                else
    //                {
    //                    types[type] += 1;
    //                }
    //                count++;
    //            }
    //            filecount++;
    //        }

    //        if (filecount == 0)
    //        {
    //            var temp = Directory.GetDirectories(directory);

    //            return temp.Length == 0 ? DirectoryType.Empty : DirectoryType.Mixed;
    //        }

    //        if (count < 10 || types.Count == 0)
    //        {
    //            return DirectoryType.Mixed;
    //        }

    //        var values = new List<KeyValuePair<int, DirectoryType>>();

    //        foreach (var item in types)
    //        {
    //            values.Add(new KeyValuePair<int, DirectoryType>(item.Value, item.Key));
    //        }

    //        values.Sort((left, right) => Comparer<int>.Default.Compare(right.Key, left.Key));

    //        return values[0].Value;
    //    }
    //}
}
