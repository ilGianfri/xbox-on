using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XboxOn.Tray
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(IPTxt.Text))
                Properties.Settings.Default.IP = IPTxt.Text;
            if (!string.IsNullOrEmpty(LiveIdTxt.Text))
                Properties.Settings.Default.LiveId = LiveIdTxt.Text;

            Properties.Settings.Default.Save();
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            IPTxt.Text = Properties.Settings.Default.IP;
            LiveIdTxt.Text = Properties.Settings.Default.LiveId;
        }
    }
}
