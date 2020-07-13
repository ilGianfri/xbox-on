using ControlzEx.Theming;
using System.Windows;

namespace XboxOn.Tray
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set the application theme to Dark.Green
            ThemeManager.Current.ChangeTheme(this, Tray.Properties.Settings.Default.Theme == 0 ? "Light.Blue" : "Dark.Blue");
        }
    }
}
