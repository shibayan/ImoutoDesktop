using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Google.Protobuf.WellKnownTypes;

using ImoutoDesktop.Models;
using ImoutoDesktop.Remoting;
using ImoutoDesktop.Scripting;
using ImoutoDesktop.Windows;

namespace ImoutoDesktop
{
    public class Context
    {
        private Context(Character character)
        {
            // いもうとの定義
            Character = character;

            // ルートディレクトリ
            RootDirectory = character.Directory;

            // プロファイルを読み込む
            Profile = Profile.LoadFrom(Path.Combine(RootDirectory, "profile.yml"));
            Profile.Age = Profile.Age == 0 ? Character.Age : Profile.Age;
            Profile.TsundereLevel = Profile.TsundereLevel == 0 ? Character.TsundereLevel : Profile.TsundereLevel;

            // バルーン読み込み
            Balloon = BalloonManager.GetBalloon(Profile.LastBalloon);
            Balloon.CanSelect = false;

            // ルートからイメージ用ディレクトリを作る
            SurfaceLoader = new SurfaceLoader(Path.Combine(RootDirectory, "images"));

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

            // スクリプトエンジンを作成する
            ScriptEngine = new ScriptEngine(Path.Combine(RootDirectory, "scripts"));

            // スクリプトプレイヤーを作成
            ScriptPlayer = new ScriptPlayer(this);

            RemoteConnectionManager = new RemoteConnectionManager();

            CommandManager = new Commands.CommandManager(Character, RemoteConnectionManager);

            InitializeScriptEngine();
        }

        private static readonly object _syncLock = new();

        public static Context Create(Guid id)
        {
            lock (_syncLock)
            {
                if (!CharacterManager.TryGetCharacter(id, out var character))
                {
                    return null;
                }

                character.CanSelect = false;
                var context = new Context(character);
                return context;
            }
        }

        public static void Delete(Context context)
        {
            lock (_syncLock)
            {
                context.Character.CanSelect = true;
                // 設定を保存
                Settings.Default.LastCharacter = context.Character.Id;
                // 起動中のコンテキストが 1 つも無くなったら終了
                Application.Current.Shutdown();
            }
        }

        public string RootDirectory { get; }

        public Balloon Balloon { get; private set; }

        public Character Character { get; }

        public Profile Profile { get; }

        public SurfaceLoader SurfaceLoader { get; }

        public CharacterWindow CharacterWindow { get; }

        public BalloonWindow BalloonWindow { get; }

        public ScriptEngine ScriptEngine { get; }

        public ScriptPlayer ScriptPlayer { get; }

        public Commands.CommandManager CommandManager { get; }

        public RemoteConnectionManager RemoteConnectionManager { get; }

        public int HistoryIndex { get; set; } = -1;

        public List<string> CommandHistory { get; } = new();

        public void Run()
        {
            // メニューのコマンドを定義する
            var contextMenu = (ContextMenu)Application.Current.Resources["ContextMenuKey"];
            contextMenu.CommandBindings.Clear();
            contextMenu.CommandBindings.Add(new CommandBinding(Input.DefaultCommands.Character, CharacterCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.DefaultCommands.Balloon, BalloonCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.DefaultCommands.Option, OptionCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.DefaultCommands.Version, VersionCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommand_Executed));
            // 初期サーフェスを表示して起動
            CharacterWindow.ChangeSurface(0);
            BalloonWindow.Show();
            PlayInvoke("OnBoot");
        }

        public void Close()
        {
            PlayInvoke("OnClose");
        }

        public void Shutdown()
        {
            // メニュー周りクリーンアップ
            var contextMenu = (ContextMenu)Application.Current.Resources["ContextMenuKey"];
            contextMenu.CommandBindings.Clear();
            // リソースを破棄する
            CharacterWindow.Close();
            BalloonWindow.Close();
            ScriptPlayer.Stop();
            ScriptEngine.Dispose();
            // プロファイルを保存
            Profile.LastBalloon = Balloon.Id;
            Profile.BalloonOffset = BalloonWindow.LocationOffset;
            Profile.SaveTo(Path.Combine(RootDirectory, "profile.yml"));
            // コンテキストを削除
            Delete(this);
        }

        public void PlayInvoke(string id)
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

            var command = CommandManager.Get(input);

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
            ScriptEngine.Age = Profile.Age;
            ScriptEngine.TsundereLevel = Profile.TsundereLevel;
            ScriptEngine.Connecting = false;
            ScriptEngine.AllowOperate = Settings.Default.AllowImoutoAllOperation;
        }

        private void CharacterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var id = (Guid)e.Parameter;
            var context = Create(id);
            if (context == null)
            {
                return;
            }
            Shutdown();
            context.Run();
        }

        private void BalloonCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var id = (Guid)e.Parameter;

            if (!BalloonManager.TryGetBalloon(id, out var balloon))
            {
                return;
            }

            Balloon.CanSelect = true;
            balloon.CanSelect = false;

            Balloon = balloon;
            BalloonWindow.Balloon = balloon;
        }

        private void OptionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SettingDialog
            {
                Age = Profile.Age,
                TsundereLevel = Profile.TsundereLevel
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

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
