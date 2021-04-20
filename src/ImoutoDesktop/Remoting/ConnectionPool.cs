using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Cryptography;
using System.Text;

namespace ImoutoDesktop.Remoting
{
    static class ConnectionPool
    {
        private static IChannel _channel;
        private static IRemoteService _connection;

        public static IRemoteService Connection
        {
            get { return _connection; }
        }

        public static bool IsConnected
        {
            get
            {
                try
                {
                    return _connection != null && _connection.IsConnecting;
                }
                catch
                {
                    _connection = null;
                    return false;
                }
            }
        }

        public static bool? Connect(string address, int port, string password)
        {
            try
            {
                if (_channel != null)
                {
                    ChannelServices.UnregisterChannel(_channel);
                }
                var dic = new Dictionary<string, string>
                {
                    { "secure", "true" }
                };
                _channel = new TcpChannel(dic, null, null);
                ChannelServices.RegisterChannel(_channel, true);
                _connection = (IRemoteService)Activator.GetObject(typeof(IRemoteService),
                    $"tcp://{address}:{port}/ImoutoDesktop");
            }
            catch
            {
                return null;
            }
            // ログインする
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            var digest = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return _connection.Login(digest);
        }

        public static void Disconnect()
        {
            _connection = null;
        }
    }
}
