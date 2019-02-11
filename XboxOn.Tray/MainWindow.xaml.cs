using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XboxOn.Tray
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings set = new Settings();
            set.Show();
        }

        private async void XboxOnBtn_Click(object sender, RoutedEventArgs e)
        {
            await XboxWake(Properties.Settings.Default.IP, Properties.Settings.Default.LiveId);
        }

        public static async Task XboxWake(string ipAddress, string liveId, int retries = 5)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint sending_end_point = new IPEndPoint(ip, 5050);

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

                    socket.SendTo(ms.ToArray(), sending_end_point);
                }

                await Task.Delay(1000);
            }
        }
    }
}
