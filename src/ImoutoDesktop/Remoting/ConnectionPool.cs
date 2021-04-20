using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace ImoutoDesktop.Remoting
{
    static class ConnectionPool
    {
        static ConnectionPool()
        {
        }

        private static IChannel _channel = null;
        private static IRemoteService _connection = null;

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
                    string.Format("tcp://{0}:{1}/ImoutoDesktop", address, port));
            }
            catch
            {
                return null;
            }
            // ログインする
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            string digest = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return _connection.Login(digest);
        }

        public static void Disconnect()
        {
            _connection = null;
        }
    }
}
