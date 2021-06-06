using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Google.Protobuf.WellKnownTypes;

using ImoutoDesktop.Models;
using ImoutoDesktop.Scripting;
using ImoutoDesktop.Services;
using ImoutoDesktop.Views;

namespace ImoutoDesktop
{
    public class CharacterContext
    {
        private CharacterContext(Character character)
        {
            // いもうとの定義
            Character = character;

            // ルートディレクトリ
            BaseDirectory = character.Directory;

            // プロファイルを読み込む
            Profile = Profile.LoadFrom(Path.Combine(BaseDirectory, "profile.yml")) ?? new Profile { Age = Character.Age, TsundereLevel = Character.TsundereLevel };

            // バルーン読み込み
            Balloon = BalloonManager.GetValueOrDefault(Profile.LastBalloon);

            // ルートからイメージ用ディレクトリを作る
            SurfaceLoader = new SurfaceLoader(Path.Combine(BaseDirectory, "images"));

            // ウィンドウを作成する
            BalloonWindow = new BalloonWindow
            {
                Context = this,
                Balloon = Balloon,
                LocationOffset = Profile.BalloonOffset
            };

            CharacterWindow = new CharacterWindow
            {
                Context = this,
                BalloonWindow = BalloonWindow
            };

            // メニューのコマンドを定義する
            var contextMenu = CharacterWindow.ContextMenu;

            contextMenu.CommandBindings.Add(new CommandBinding(Input.DefaultCommands.Character, CharacterCommand_Executed, CharacterCommand_CanExecute));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.DefaultCommands.Balloon, BalloonCommand_Executed, BalloonCommand_CanExecute));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.DefaultCommands.Option, OptionCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.DefaultCommands.Version, VersionCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommand_Executed));

            // スクリプトエンジンを作成する
            ScriptEngine = new ScriptEngine(Path.Combine(BaseDirectory, "scripts"));

            InitializeScriptEngine();

            // スクリプトプレイヤーを作成
            ScriptPlayer = new ScriptPlayer(this);

            RemoteConnectionManager = new RemoteConnectionManager();

            RemoteCommandManager = new RemoteCommandManager(Character, RemoteConnectionManager);
        }

        private static readonly Dictionary<string, CharacterContext> _activeContexts = new();

        public static CharacterContext Create(string id)
        {
            if (!CharacterManager.TryGetValue(id, out var character))
            {
                return null;
            }

            var context = new CharacterContext(character);

            _activeContexts.Add(id, context);

            return context;
        }

        public static void Delete(CharacterContext context)
        {
            // 設定を保存
            Settings.Default.LastCharacter = context.Character.Id;

            _activeContexts.Remove(context.Character.Id);

            // 起動中のコンテキストが 1 つも無くなったら終了
            if (_activeContexts.Count == 0)
            {
                Application.Current.Shutdown();
            }
        }

        public string BaseDirectory { get; }

        public Balloon Balloon { get; private set; }

        public Character Character { get; }

        public Profile Profile { get; }

        public SurfaceLoader SurfaceLoader { get; }

        public CharacterWindow CharacterWindow { get; }

        public BalloonWindow BalloonWindow { get; }

        public ScriptEngine ScriptEngine { get; }

        public ScriptPlayer ScriptPlayer { get; }

        public RemoteCommandManager RemoteCommandManager { get; }

        public RemoteConnectionManager RemoteConnectionManager { get; }

        public int HistoryIndex { get; set; } = -1;

        public List<string> CommandHistory { get; } = new();

        public void Start()
        {
            // 初期サーフェスを表示して起動
            CharacterWindow.ChangeSurface(0);
            BalloonWindow.Show();

            PlayEvent("OnBoot");
        }

        public void Close()
        {
            PlayEvent("OnClose");
        }

        public void Shutdown()
        {
            // リソースを破棄する
            CharacterWindow.Close();
            BalloonWindow.Close();
            ScriptPlayer.Stop();
            ScriptEngine.Dispose();

            // プロファイルを保存
            Profile.LastBalloon = Balloon.Id;
            Profile.BalloonOffset = BalloonWindow.LocationOffset;
            Profile.SaveTo(Path.Combine(BaseDirectory, "profile.yml"));

            // コンテキストを削除
            Delete(this);
        }

        public void PlayEvent(string id)
        {
            var result = ScriptEngine.Invoke(id);

            var script = CreateScript();

            script.AppendLine(Script.Scope.Character, result);

            if (id == "OnClose")
            {
                script.AppendLine(Script.Scope.System, @"\-");
            }

            ScriptPlayer.Play(script);
        }

        public async Task ExecCommand(string input)
        {
            var script = CreateScript();

            script.AppendLine(Script.Scope.User, input.Replace(@"\", @"\\"));

            if (RemoteConnectionManager.GetServiceClient() != null)
            {
                var heartbeatResponse = await RemoteConnectionManager.GetServiceClient().HeartbeatAsync(new Empty());

                ScriptEngine.Connecting = heartbeatResponse.Succeeded;
            }
            else
            {
                ScriptEngine.Connecting = false;
            }

            try
            {
                var command = RemoteCommandManager.Get(input);

                // コマンドが見つかったか調べる
                if (command != null)
                {
                    // コマンド実行前準備
                    var canExecuteResult = await command.PreExecute(input);

                    // 事前イベントのスクリプトを実行
                    var preEventResult = ScriptEngine.Invoke($"OnPre{canExecuteResult.EventId}", canExecuteResult.Arguments);

                    // スクリプトの実行結果を追加する
                    if (!string.IsNullOrEmpty(preEventResult))
                    {
                        script.AppendLine(Script.Scope.Character, preEventResult);
                    }

                    // 実際にコマンドを実行するか判別
                    if (!ScriptEngine.Reject)
                    {
                        // コマンドを実行する
                        var executeResult = await command.Execute(input);

                        // イベントのスクリプトを実行
                        var eventResult = ScriptEngine.Invoke($"On{executeResult.EventId}", executeResult.Arguments);

                        // スクリプトの実行結果を追加する
                        if (!string.IsNullOrEmpty(eventResult))
                        {
                            script.AppendLine(Script.Scope.Character, eventResult);
                        }

                        // 実行結果が存在すれば追加する
                        if (!string.IsNullOrEmpty(executeResult.Message))
                        {
                            script.AppendLine(Script.Scope.System, executeResult.Message);
                        }
                    }

                    // 終了コマンドなら終了する
                    if (canExecuteResult.EventId == "Close")
                    {
                        script.AppendLine(Script.Scope.System, @"\-");
                    }
                }
                else
                {
                    // コマンドが存在しない
                    var result = ScriptEngine.Invoke("OnUnknownCommand", input);

                    // スクリプトの実行結果を追加する
                    if (!string.IsNullOrEmpty(result))
                    {
                        script.AppendLine(Script.Scope.Character, result);
                    }
                }
            }
            catch
            {
                // コマンドが存在しない
                var result = ScriptEngine.Invoke("OnUnknownCommand", input);

                // スクリプトの実行結果を追加する
                if (!string.IsNullOrEmpty(result))
                {
                    script.AppendLine(Script.Scope.Character, result);
                }
            }

            // スクリプトを再生
            ScriptPlayer.Play(script);
        }

        private Script CreateScript()
        {
            var script = new Script
            {
                UserName = Settings.Default.UserName,
                ImoutoName = Character.Name,
                Honorific = Settings.Default.Honorific,
                UserColor = Balloon.UserColor,
                ImoutoColor = Balloon.ImoutoColor
            };

            return script;
        }

        private void InitializeScriptEngine()
        {
            ScriptEngine.Age = Profile.Age ?? Character.Age;
            ScriptEngine.TsundereLevel = Profile.TsundereLevel ?? Character.TsundereLevel;
            ScriptEngine.Connecting = false;
            ScriptEngine.AllowOperate = Settings.Default.AllowImoutoAllOperation;
        }

        private void CharacterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var id = (string)e.Parameter;

            e.CanExecute = id != Character.Id;
        }

        private void CharacterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var id = (string)e.Parameter;
            var context = Create(id);
            if (context == null)
            {
                return;
            }
            Shutdown();
            context.Start();
        }

        private void BalloonCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var id = (string)e.Parameter;

            e.CanExecute = id != Balloon.Id;
        }

        private void BalloonCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var id = (string)e.Parameter;

            if (!BalloonManager.TryGetValue(id, out var balloon))
            {
                return;
            }

            Balloon = balloon;
            BalloonWindow.Balloon = balloon;
        }

        private void OptionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SettingDialog
            {
                Age = Profile.Age ?? Character.Age,
                TsundereLevel = Profile.TsundereLevel ?? Character.TsundereLevel
            };

            if (dialog.ShowDialog() ?? false)
            {
                Profile.Age = dialog.Age;
                Profile.TsundereLevel = dialog.TsundereLevel;

                InitializeScriptEngine();
            }
        }

        private void VersionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var attribute = typeof(App).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            MessageBox.Show(
                $"いもうとデスクトップ v{attribute.InformationalVersion}",
                "バージョン情報",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e) => Close();
    }
}
