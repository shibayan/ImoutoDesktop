using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ImoutoDesktop.Scripting
{
    public class ScriptPlayer : DispatcherObject
    {
        public ScriptPlayer(CharacterContext context)
        {
            _context = context;
            _thread = new Thread(PlayThread);
            _thread.Start();
        }

        private readonly CharacterContext _context;

        private readonly Thread _thread;

        private readonly object _syncLock = new();
        private readonly AutoResetEvent _event = new(false);

        private readonly Queue<Script> _queue = new();

        private bool _isAbort;

        public void Play(Script script)
        {
            lock (_syncLock)
            {
                if (script.Length == 0)
                {
                    return;
                }
                _queue.Enqueue(script);
                _event.Set();
            }
        }

        public void Stop()
        {
            if (_thread == null || !_thread.IsAlive)
            {
                return;
            }

            _isAbort = true;

            lock (_syncLock)
            {
                _queue.Clear();
                _event.Set();
            }

            _thread.Join(5000);
        }

        private void Invoke(Action method)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, method);
        }

        private void Invoke<T>(Action<T> method, T arg)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, method, arg);
        }

        private void AsyncInvoke(Action method)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
        }

        private void AsyncInvoke<T>(Action<T> method, T arg)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, method, arg);
        }

        private void PlayThread()
        {
            var isClosed = false;
            while (!_isAbort)
            {
                var queueCount = 0;
                var isInitialized = false;
                var isQuickSession = false;
                Script script;
                lock (_syncLock)
                {
                    queueCount = _queue.Count;
                }
                if (queueCount == 0)
                {
                    _event.WaitOne();
                }
                lock (_syncLock)
                {
                    if (_queue.Count == 0)
                    {
                        continue;
                    }
                    script = _queue.Dequeue();
                }
                var imoutoWindow = _context.CharacterWindow;
                var balloonWindow = _context.BalloonWindow;
                using var e = script.GetEnumerator();
                while (e.MoveNext())
                {
                    var token = e.Current;
                    switch (token.Kind)
                    {
                        case TokenKind.Text:
                            if (!isInitialized)
                            {
                                Invoke(() =>
                                {
                                    if (!balloonWindow.IsVisible)
                                    {
                                        balloonWindow.Show();
                                    }
                                    balloonWindow.TextViewer.AddBlock();
                                });
                                isInitialized = true;
                            }
                            if (!isQuickSession)
                            {
                                foreach (var item in (string)token.Value)
                                {
                                    Invoke(p => balloonWindow.TextViewer.AddChar(p), item);
                                    Thread.Sleep(50);
                                }
                            }
                            else
                            {
                                Invoke(p => balloonWindow.TextViewer.AddLine(p), (string)token.Value);
                            }
                            break;
                        case TokenKind.Surface:
                            Invoke(p => imoutoWindow.ChangeSurface(p), (int)token.Value);
                            break;
                        case TokenKind.Font:
                            switch ((FontOperation)token.Parameters[0])
                            {
                                case FontOperation.Color:
                                    if (token.Parameters[1] != null)
                                    {
                                        Invoke(p => balloonWindow.TextViewer.CurrentFontColor = new SolidColorBrush(p), (Color)token.Parameters[1]);
                                    }
                                    else
                                    {
                                        Invoke(() => balloonWindow.TextViewer.CurrentFontColor = null);
                                    }
                                    break;
                                case FontOperation.FontFamily:
                                    if (token.Parameters[1] != null)
                                    {
                                        Invoke(p => balloonWindow.TextViewer.CurrentFontFamily = new FontFamily(p), (string)token.Parameters[1]);
                                    }
                                    else
                                    {
                                        Invoke(() => balloonWindow.TextViewer.CurrentFontFamily = null);
                                    }
                                    break;
                                case FontOperation.Size:
                                    if (token.Parameters[1] != null)
                                    {
                                        Invoke(p => balloonWindow.TextViewer.CurrentFontSize = p, (double)token.Parameters[1]);
                                    }
                                    else
                                    {
                                        Invoke(() => balloonWindow.TextViewer.CurrentFontSize = null);
                                    }
                                    break;
                                case FontOperation.Weight:
                                    if ((bool)token.Parameters[1])
                                    {
                                        Invoke(() => balloonWindow.TextViewer.CurrentFontWeight = FontWeights.Bold);
                                    }
                                    else
                                    {
                                        Invoke(() => balloonWindow.TextViewer.CurrentFontWeight = null);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TokenKind.Clear:
                            Invoke(delegate { balloonWindow.TextViewer.Clear(); });
                            break;
                        case TokenKind.LineBreak:
                            Invoke(delegate { balloonWindow.TextViewer.LineBreak(); });
                            break;
                        case TokenKind.Invoke:
                            break;
                        case TokenKind.Sleep:
                            Thread.Sleep((int)token.Value);
                            break;
                        case TokenKind.Exit:
                            isClosed = true;
                            break;
                        //case TokenKind.Audio:
                        //    if (token.Parameters.Length == 1)
                        //    {
                        //    }
                        //    else
                        //    {
                        //        switch ((MediaOperation)token.Parameters[0])
                        //        {
                        //            case MediaOperation.Play:
                        //                Invoke(delegate { balloonWindow.TextViewer.AddAudio(new Uri((string)token.Parameters[1])); });
                        //                break;
                        //            case MediaOperation.Stop:
                        //                break;
                        //            case MediaOperation.Pause:
                        //                break;
                        //            case MediaOperation.Wait:
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //    }
                        //    break;
                        case TokenKind.Image:
                            if (!isInitialized)
                            {
                                Invoke(() =>
                                {
                                    if (!balloonWindow.IsVisible)
                                    {
                                        balloonWindow.Show();
                                    }
                                    balloonWindow.TextViewer.AddBlock();
                                });
                                isInitialized = true;
                            }
                            if (token.Parameters.Length == 1)
                            {
                                Invoke(p => balloonWindow.TextViewer.AddImage(new Uri(p)), (string)token.Parameters[0]);
                            }
                            else
                            {
                                Invoke(p => balloonWindow.TextViewer.AddImage(new Uri((string)p[0]), (double)p[1], (double)p[2]), token.Parameters);
                            }
                            break;
                        //case TokenKind.Video:
                        //    if (!isInitialized)
                        //    {
                        //        Invoke(delegate
                        //        {
                        //            if (!balloonWindow.IsVisible)
                        //            {
                        //                balloonWindow.Show();
                        //                context.CommandWindow.Activate();
                        //            }
                        //            balloonWindow.TextViewer.AddBlock();
                        //        }
                        //        );
                        //        isInitialized = true;
                        //    }
                        //    if (token.Parameters.Length == 1)
                        //    {
                        //    }
                        //    else if (token.Parameters.Length == 2)
                        //    {
                        //        switch ((MediaOperation)token.Parameters[0])
                        //        {
                        //            case MediaOperation.Play:
                        //                Invoke(delegate { balloonWindow.TextViewer.AddVideo(new Uri((string)token.Parameters[1])); });
                        //                break;
                        //            case MediaOperation.Stop:
                        //                break;
                        //            case MediaOperation.Pause:
                        //                break;
                        //            case MediaOperation.Wait:
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Invoke(delegate { balloonWindow.TextViewer.AddVideo(new Uri((string)token.Parameters[1]), (double)token.Parameters[2], (double)token.Parameters[3]); });
                        //    }
                        //    break;
                        case TokenKind.BeginQuickSession:
                            isQuickSession = true;
                            break;
                        case TokenKind.EndQuickSession:
                            isQuickSession = false;
                            break;
                    }
                }
                if (isClosed)
                {
                    Thread.Sleep(500);
                    AsyncInvoke(() => _context.Shutdown());
                    break;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
