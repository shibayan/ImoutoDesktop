using System;
using System.Security.Cryptography;
using System.Text;

namespace ImoutoDesktop.Remoting
{
    static class ConnectionPool
    {
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
                    return _connection is { IsConnecting: true };
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
