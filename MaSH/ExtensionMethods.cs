using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MaSH
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Returns the build date and time.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="target"></param>
        /// <see cref="https://stackoverflow.com/a/1600990/1174692"/>
        /// <returns></returns>
        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt64(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        public static void MoveUp<T>(this List<T> list, T item)
        {
            var indexOf = list.IndexOf(item);
            if (indexOf <= 0)
            {
                return;
            }
            list.RemoveAt(indexOf);
            list.Insert(indexOf - 1, item);
        }

        public static void MoveDown<T>(this List<T> list, T item)
        {
            var indexOf = list.IndexOf(item);
            if (indexOf >= list.Count)
            {
                return;
            }
            list.RemoveAt(indexOf);
            list.Insert(indexOf + 1, item);
        }

        public static string GetAppExePath()
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }

        public static void StartWithWindows(string appName, bool startWithWindows, string arguments = "")
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (startWithWindows)
            {
                if (arguments != "")
                {
                    arguments = " " + arguments;
                }
                rk.SetValue(appName, "\"" + GetAppExePath() + "\"" + arguments);
            }
            else
            {
                rk.DeleteValue(appName, false);
            }

        }

    }
}
