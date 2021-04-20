using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;

namespace ImoutoDesktop.Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Form = this;
            notifyIcon1.Icon = Icon;
            Settings.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "settings.xml"));
            // ライフタイム設定
            LifetimeServices.LeaseTime = TimeSpan.FromDays(1);
            LifetimeServices.RenewOnCallTime = TimeSpan.Zero;
        }

        private IChannel _channel;
        private bool _isClosing;

        public static Form Form { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Settings.Default.PortNumber.ToString();
            textBox2.Text = Settings.Default.Password;
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var address in addresses)
            {
                if (!address.IsIPv6LinkLocal && !address.IsIPv6Multicast && !address.IsIPv6SiteLocal)
                {
                    label6.Text = address.ToString();
                    break;
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            int port = 1024;
            if (int.TryParse(textBox1.Text, out port))
            {
                Settings.Default.PortNumber = port;
            }
            Settings.Default.Password = textBox2.Text;
            Settings.Save(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "settings.xml"));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isClosing && e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
            _isClosing = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_channel == null)
            {
                if (textBox2.Text.Length < 8)
                {
                    MessageBox.Show("8 文字以上のパスワードを入力してください");
                    return;
                }
                // 設定を保存
                Settings.Default.PortNumber = int.Parse(textBox1.Text);
                Settings.Default.Password = textBox2.Text;
                // 接続開始
                label2.Text = "起動処理中です...";
                _channel = new TcpChannel(Settings.Default.PortNumber);
                ChannelServices.RegisterChannel(_channel, true);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteService), "ImoutoDesktop", WellKnownObjectMode.Singleton);
                label2.Text = "起動中です";
                notifyIcon1.Text = "いもうとデスクトップ - 起動中";
                // ボタンを変更
                button1.Text = "サーバ停止";
                // アイコン以外隠す
                Hide();
            }
            else
            {
                // 接続終了
                label2.Text = "停止処理中です...";
                ChannelServices.UnregisterChannel(_channel);
                _channel = null;
                label2.Text = "停止中です";
                notifyIcon1.Text = "いもうとデスクトップ - 停止中";
                // ボタンを変更
                button1.Text = "サーバ起動";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _isClosing = true;
            if (_channel != null)
            {
                ChannelServices.UnregisterChannel(_channel);
                _channel = null;
            }
            Close();
        }

        private void サーバ情報SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }
    }
}
