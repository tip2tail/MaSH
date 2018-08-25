using MaSH.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MaSH
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MashSchedule mashSchedule;
        private int editingIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void frmMain_Loaded(object sender, RoutedEventArgs e)
        {
            // Upgrade settings
            if (Settings.Default.UpdateSettings) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                Settings.Default.Save();
            }

            // Load the schedule
            if (!IsValidJson(Settings.Default.Schedule)) {
                mashSchedule = new MashSchedule();
                mashSchedule.Updated = DateTimeOffset.Now;
                mashSchedule.Apps = new List<MashApp>();
                Settings.Default.Schedule = mashSchedule.ToJson();
                Settings.Default.Enabled = false;
                Settings.Default.Save();
            }
            ReloadSchedule();

            // Populate the version information
            string[] versionParts = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            labVersion.Content = string.Format("{0}.{1}.{2}", versionParts[0], versionParts[1], versionParts[2]);

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MaSH.Resources.BuildDate.txt");
            StreamReader sr = new StreamReader(stream);
            labBuildDate.Content = Convert.ToDateTime(sr.ReadToEnd()).ToLongDateString();

            radYes.IsChecked = Settings.Default.Enabled;
            radNo.IsChecked = !Settings.Default.Enabled;
            StartupWithWindows();

        }

        private void StartupWithWindows()
        {
            ExtensionMethods.StartWithWindows("MaSH", Settings.Default.Enabled, "--execute-schedule");
        }

        private void ReloadSchedule()
        {

            EnableDisableEditorControls(false);
            EnableDisableScheduleControls(true);
            listApps.Items.Clear();

            mashSchedule = MashSchedule.FromJson(Settings.Default.Schedule);

            // Populate the grid
            if (mashSchedule.Apps != null && mashSchedule.Apps.Count > 0)
            {
                foreach (var app in mashSchedule.Apps)
                {
                    listApps.Items.Add(app);
                }
            }
        }

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            EnableDisableScheduleControls(false);
            EnableDisableEditorControls(true);
            ClearEditorControls();
            txtAppName.Focus();
            editingIndex = -1;
        }

        private void ClearEditorControls()
        {
            txtAppCommand.Clear();
            txtAppName.Clear();
            txtAppParams.Clear();
            txtAppDelay.Text = "1";
        }

        private void EnableDisableScheduleControls(bool enable)
        {
            listApps.IsEnabled = enable;
            btnAdd.IsEnabled = enable;
            btnEdit.IsEnabled = enable;
            btnDelete.IsEnabled = enable;
            btnUp.IsEnabled = enable;
            btnDown.IsEnabled = enable;
        }

        private void EnableDisableEditorControls(bool enable)
        {
            panelEditor.IsEnabled = enable;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            // Validation
            if (txtAppName.Text.Trim().Length < 1)
            {
                MessageBox.Show("You must enter an application name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (txtAppCommand.Text.Trim().Length < 1)
            {
                MessageBox.Show("You must enter an application command", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!IsDigitsOnly(txtAppDelay.Text.Trim()))
            {
                MessageBox.Show("You must enter only digits for the delay seconds (default is 1)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Convert.ToInt32(txtAppDelay.Text.Trim()) < 1 || Convert.ToInt32(txtAppDelay.Text.Trim()) > 120)
            {
                MessageBox.Show("Delay seconds should be between 1 and 120 seconds", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Save the changes
            if (editingIndex == -1)
            {
                // Add
                MashApp app = new MashApp()
                {
                    Name = txtAppName.Text.Trim(),
                    Command = txtAppCommand.Text.Trim(),
                    Params = txtAppParams.Text.Trim(),
                    Delay = Convert.ToInt32(txtAppDelay.Text.Trim()),
                };
                mashSchedule.Apps.Add(app);
            } else {
                // Edit
                mashSchedule.Apps[editingIndex].Name = txtAppName.Text.Trim();
                mashSchedule.Apps[editingIndex].Command = txtAppCommand.Text.Trim();
                mashSchedule.Apps[editingIndex].Params = txtAppParams.Text.Trim();
                mashSchedule.Apps[editingIndex].Delay = Convert.ToInt32(txtAppDelay.Text.Trim());
            }

            // Save and Reload
            SaveAndReloadSchedule();
        }

        private bool IsDigitsOnly(string str)
        {
            if (str == string.Empty)
            {
                txtAppDelay.Text = "1";
                str = "1";
            }
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        private void SaveAndReloadSchedule()
        {
            mashSchedule.Updated = DateTimeOffset.Now;
            Settings.Default.Schedule = mashSchedule.ToJson();
            Settings.Default.Save();
            ReloadSchedule();
        }

        private void listApps_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listApps.SelectedIndex > -1)
            {
                ClearEditorControls();
                MashApp app = (MashApp)listApps.SelectedItem;
                txtAppName.Text = app.Name;
                txtAppCommand.Text = app.Command;
                txtAppParams.Text = app.Params;
                txtAppDelay.Text = app.Delay.ToString();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you wish to cancel the changes made?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ClearEditorControls();
                ReloadSchedule();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            HandleEdit();
        }

        private void HandleEdit()
        {
            if (listApps.SelectedIndex > -1)
            {
                editingIndex = listApps.SelectedIndex;
                ClearEditorControls();
                MashApp app = (MashApp)listApps.SelectedItem;
                txtAppName.Text = app.Name;
                txtAppCommand.Text = app.Command;
                txtAppParams.Text = app.Params;
                txtAppDelay.Text = app.Delay.ToString();
                EnableDisableScheduleControls(false);
                EnableDisableEditorControls(true);
                txtAppName.Focus();
            }
        }

        private void listApps_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HandleEdit();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listApps.SelectedIndex > -1)
            {
                if (MessageBox.Show("Do you want to delete this app?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    mashSchedule.Apps.Remove((MashApp)listApps.SelectedItem);
                    SaveAndReloadSchedule();
                }
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (listApps.SelectedIndex > 0)
            {
                MashApp app = (MashApp)listApps.SelectedItem;
                mashSchedule.Apps.MoveUp(app);
                listApps.SelectedIndex = listApps.Items.IndexOf(app);
                SaveAndReloadSchedule();
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (listApps.SelectedIndex > -1 && listApps.SelectedIndex < (listApps.Items.Count - 1))
            {
                MashApp app = (MashApp)listApps.SelectedItem;
                mashSchedule.Apps.MoveDown(app);
                listApps.SelectedIndex = listApps.Items.IndexOf(app);
                SaveAndReloadSchedule();
            }
        }

        private void labGitHub_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://www.github.com/tip2tail/MaSH");
        }

        private void btnTestExecute_Click(object sender, RoutedEventArgs e)
        {
            ScheduleExecutor.Go(true);
        }

        private void radNo_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.Enabled = false;
            Settings.Default.Save();
            StartupWithWindows();
        }

        private void radYes_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.Enabled = true;
            Settings.Default.Save();
            StartupWithWindows();
        }
    }
}
