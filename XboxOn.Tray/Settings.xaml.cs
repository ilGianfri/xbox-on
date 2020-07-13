using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System.Windows;

namespace XboxOn.Tray
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : MetroWindow
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(IPTxt.Text))
            {
                Properties.Settings.Default.IP = IPTxt.Text;
            }

            if (!string.IsNullOrEmpty(LiveIdTxt.Text))
            {
                Properties.Settings.Default.LiveId = LiveIdTxt.Text;
            }

            Properties.Settings.Default.Save();
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            //ThemeCombo.SelectedIndex = Properties.Settings.Default.Theme;

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.IP))
            {
                IPTxt.Text = Properties.Settings.Default.IP;
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.LiveId))
            {
                LiveIdTxt.Text = Properties.Settings.Default.LiveId;
            }

            LaunchMinimized.IsChecked = Properties.Settings.Default.LaunchMinimized;
        }

        private void LaunchMinimized_Checked(object sender, RoutedEventArgs e) =>
            Properties.Settings.Default.LaunchMinimized = true;

        private void LaunchMinimized_Unchecked(object sender, RoutedEventArgs e) =>
            Properties.Settings.Default.LaunchMinimized = false;

        //private void ThemeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    Properties.Settings.Default.Theme = ThemeCombo.SelectedIndex;
        //}
    }
}
