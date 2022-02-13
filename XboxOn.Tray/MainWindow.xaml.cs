using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace XboxOn.Tray
{
    public partial class MainWindow : Window
    {
        private NotifyIcon noticon;
        public MainWindow()
        {
            InitializeComponent();

            Rect desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;

            if (Properties.Settings.Default.LaunchMinimized == true)
            {
                Hide();
            }

            SystemTray();
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings set = new();
            set.Show();
        }

        private async void XboxOnBtn_Click(object sender, RoutedEventArgs e) =>
            await XboxWake(Properties.Settings.Default.IP, Properties.Settings.Default.LiveId);

        public async Task XboxWake(string ipAddress, string liveId, int retries = 5)
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint port = new(ip, 5050);

            for (int retry = 0; retry < retries; retry++)
            {
                byte[] payload = new byte[3 + liveId.Length];
                payload[0] = 0x00;
                payload[1] = (byte)liveId.Length;

                for (int i = 0; i < liveId.Length; i++)
                {
                    payload[i + 2] = (byte)liveId[i];
                }
                payload[payload.Length - 1] = 0x00;

                byte[] header = new byte[6];
                header[0] = 0xdd;
                header[1] = 0x02;
                header[2] = 0x00;
                header[3] = (byte)payload.Length;
                header[4] = 0x00;
                header[5] = 0x00;

                using (MemoryStream ms = new(header.Length + payload.Length))
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
                Hide();
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
            Hide();
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
                Show();
                WindowState = WindowState.Normal;
            };

            ContextMenuStrip strip = new();
            noticon.ContextMenuStrip = strip;

            strip.Items.Add("Turn on Xbox", null, TurnOnFromMenu);
            strip.Items.Add("Exit", null, ExitEvent);
        }

        private void ExitEvent(object sender, EventArgs e) => Environment.Exit(0);

        private async void TurnOnFromMenu(object sender, EventArgs e) =>
            await XboxWake(Properties.Settings.Default.IP, Properties.Settings.Default.LiveId);
    }
}
