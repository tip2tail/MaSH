using System.Windows;

namespace MaSH
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 1 && e.Args[0] == "--execute-schedule")
            {
                ScheduleExecutor.Go();
            }
            else
            {
                MainWindow mainWin = new MainWindow();
                mainWin.Show();
            }
        }
    }
}
