using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace XboxOn.Tray
{
    public partial class MainWindow : MetroWindow
    {
        private NotifyIcon noticon;
        public MainWindow()
        {
            InitializeComponent();

            Rect desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            if (Properties.Settings.Default.LaunchMinimized == true)
                this.Hide();

            SystemTray();
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings set = new Settings();
            set.Show();
        }

        private async void XboxOnBtn_Click(object sender, RoutedEventArgs e) =>
            await XboxWake(Properties.Settings.Default.IP, Properties.Settings.Default.LiveId);

        public static async Task XboxWake(string ipAddress, string liveId, int retries = 5)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint port = new IPEndPoint(ip, 5050);

            for (int retry = 0; retry < retries; retry++)
            {
                byte[] payload = new byte[3 + liveId.Length];
                payload[0] = 0x00;
                payload[1] = (byte)liveId.Length;

                for (int i = 0; i < liveId.Length; i++)
                    payload[i + 2] = (byte)liveId[i];
                payload[payload.Length - 1] = 0x00;

                byte[] header = new byte[6];
                header[0] = 0xdd;
                header[1] = 0x02;
                header[2] = 0x00;
                header[3] = (byte)payload.Length;
                header[4] = 0x00;
                header[5] = 0x00;

                using (var ms = new MemoryStream(header.Length + payload.Length))
                {
                    ms.Write(header, 0, header.Length);
                    ms.Write(payload, 0, payload.Length);

                    socket.SendTo(ms.ToArray(), port);
                }

                await Task.Delay(1000);
            }
        }

        private void XboxOn_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();
        }

        private void XboxOn_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (!Properties.Settings.Default.MinimizedNotificationShown)
            {
                noticon.BalloonTipTitle = "Xbox On is still running";
                noticon.BalloonTipText = "You can close the app from the system tray.";
                noticon.ShowBalloonTip(50);

                if (!Properties.Settings.Default.MinimizedNotificationShown)
                {
                    Properties.Settings.Default.MinimizedNotificationShown = true;
                    Properties.Settings.Default.Save();
                }
            }
            this.Hide();
        }

        private void SystemTray()
        {
            noticon = new NotifyIcon
            {
                Icon = Properties.Resources.xboxon,
                Visible = true
            };
            noticon.DoubleClick += delegate (object s, EventArgs e)
            {
                this.Show();
                WindowState = WindowState.Normal;
            };

            ContextMenuStrip strip = new ContextMenuStrip();
            noticon.ContextMenuStrip = strip;

            strip.Items.Add("Turn on XBox", null, TurnOnFromMenu);
            strip.Items.Add("Exit", null, ExitEvent);
        }

        private void ExitEvent(object sender, EventArgs e) => Environment.Exit(0);

        private async void TurnOnFromMenu(object sender, EventArgs e) => 
            await XboxWake(Properties.Settings.Default.IP, Properties.Settings.Default.LiveId);
    }
}
