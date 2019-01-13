using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaSH
{
    public class Logging
    {

        public enum LogClass
        {
            Info    = 0,
            Warning = 1,
            Error   = 2,
            Debug   = 3,
        }

        public string LogFile { get; }

        public Logging(string logFile)
        {
            LogFile = logFile;
            CreateNewLogger();
        }

        public Logging()
        {
            LogFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppDomain.CurrentDomain.FriendlyName,
                "logs",
                DateTime.Today.ToString("yyyy-MM-dd") + ".log"
            );
            CreateNewLogger();
        }

        private void CheckCreatePath(string fileName)
        {
            string filePath = Directory.GetParent(fileName).ToString();
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }

        private void CreateNewLogger()
        {
            try
            {

                if (!IsPathValid(LogFile))
                {
                    // Path is invalid - could never exist
                    throw new Exception("The log file path provided could never be valid.");
                }

                if (!File.Exists(LogFile))
                {
                    CheckCreatePath(LogFile);
                    File.Create(LogFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Logging Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Log(LogClass lClass, string message)
        {
            var log = string.Format("[{3}] {0} ==> {1}{2}", DateTime.Now.ToString("HH:mm:ss.fff"), message.Trim(), Environment.NewLine, lClass.ToString().ToUpper());
            using (FileStream fileStream = File.Open(LogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fileStream);
                long endPoint = fileStream.Length;
                // Set the stream position to the end of the file.        
                fileStream.Seek(endPoint, SeekOrigin.Begin);
                sw.Write(log);
                sw.Flush();
            }
        }

        private bool IsPathValid(string path)
        {
            FileInfo fi = null;
            try
            {
                fi = new FileInfo(path);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }
            return !(fi is null);
        }

    }
}
