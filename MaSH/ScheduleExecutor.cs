using MaSH.Properties;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MaSH
{
    static class ScheduleExecutor
    {

        private static bool testMode;
        private static LogWindow logWin;
        private static Logging logging = new Logging();
        private static DispatcherTimer timer;
        private static int delayCountdown;
        private static MashSchedule schedule;
        private static int appIndex;

        public static void Go(bool isTestMode = false)
        {
            // Test mode?
            testMode = isTestMode;

            // Setup a timer to tick every second
            timer = new DispatcherTimer();
            timer.Tick += dispatcherTimer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);

            // Initial delay is five seconds (set to -1 here to indicate initial delay)
            delayCountdown = -1;

            // This will execute the schedule from settings
            schedule = MashSchedule.FromJson(Settings.Default.Schedule);
            appIndex = 0;

            logWin = new LogWindow(testMode);
            logWin.Show();

            Log("Schedule starting...");
            if (testMode)
            {
                Log("=== This is TEST MODE ===");
            }

            if (schedule.Apps.Count < 1)
            {
                Log("ERROR: No apps in schedule...");
                Finished();
            }
            else
            {
                // Start the timer
                timer.Start();
            }
        }

        private static void Finished()
        {
            Log("!!! Finished !!!");
            if (!testMode)
            {
                Environment.Exit(0);
            }
        }

        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (delayCountdown == -1)
            {
                delayCountdown = 5;
                Log("Waiting 5 seconds before processing first app...");
            }

            // Stop before executing the next app
            delayCountdown--;
            if (delayCountdown > 0)
            {
                return;
            }

            // Ready to execute the next app
            MashApp app = schedule.Apps[appIndex];
            Log("Launching: " + app.Name);

            // Launch it!
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = app.Command,
                Arguments = app.Params,
            };

            Log("Command: " + app.Command);
            Log("Parameters: " + app.Params);
            if (!testMode || Debugger.IsAttached)
            {
                try
                {
                    var proc = Process.Start(startInfo);
                    Log("New PID: " + proc.Id);
                }
                catch (Exception ex)
                {
                    Log("ERROR: " + ex.Message);
                }
            }

            // Next app...
            appIndex++;
            if (appIndex >= schedule.Apps.Count)
            {
                // Schedule is finished
                Finished();
            }
            else
            {
                // Set delay then loop again
                Log("Sleeping for " + app.Delay + " seconds before next app...");
                delayCountdown = app.Delay;
            }
        }

        private static void Log(string message)
        {
            if (Settings.Default.DebugLogging)
            {
                logging.Log(Logging.LogClass.Debug, message);
            }

            message = string.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), message);
            logWin.listLog.Items.Add(message);
            logWin.listLog.ScrollToBottom();
        }

    }
}
