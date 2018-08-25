using System.Windows;

namespace MaSH
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {

        private bool testMode;

        public LogWindow(bool testMode = false)
        {
            InitializeComponent();
            this.testMode = testMode;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!testMode)
            {
                e.Cancel = true;
            }
        }
    }
}
