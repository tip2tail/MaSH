﻿using MaSH.Properties;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MaSH
{
    static class ScheduleExecutor
    {

        private static LogWindow logWin;

        public async static void Go(bool testMode = false)
        {
            // This will execute the schedule
            MashSchedule schedule = MashSchedule.FromJson(Settings.Default.Schedule);

            logWin = new LogWindow(testMode);
            logWin.Show();

            Log("Schedule starting");
            if (testMode)
            {
                Log("=== TEST MODE ===");
                Log("=== Nothing will be executed ===");
            }

            await Task.Delay(1000);

            foreach (MashApp app in schedule.Apps)
            {

                Log("Next app found: " + app.Name);

                // Execute an app
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = app.Command,
                    Arguments = app.Params,
                };

                Log("Command: " + app.Command);
                Log("Parameters: " + app.Params);
                Log("Launching...");
                if (!testMode)
                {
                    Process.Start(startInfo);
                }

                Log("Sleep for " + app.Delay + " second(s)");
                await Task.Delay((app.Delay * 1000));
            }

            Log("Schedule Complete!");
            await Task.Delay(2000);

            if (!testMode)
            {
                Environment.Exit(0);
            }

        }

        private static void Log(string message)
        {
            message = string.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), message);
            logWin.listLog.Items.Insert(0, message);
        }

    }
}