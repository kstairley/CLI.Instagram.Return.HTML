using System;
using System.IO;

namespace TechShare.Utility.Tools.Logs
{
    /**
     * Logger added to make code testable as spy methods don't exists in C#.
     */
    public class CommandLineLogger : iLogger
    {
        public bool log(string message)
        {
            Console.WriteLine(message);
            return true;
        }
        public string LogFile { get; private set; }

        public CommandLineLogger(string exportFile, string defaultDirectory)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + defaultDirectory + exportFile))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + defaultDirectory + exportFile);
            LogFile = AppDomain.CurrentDomain.BaseDirectory + defaultDirectory + exportFile + "\\" + exportFile + ".log";
        }

        public void LogInfoAlert(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            LogMessage(message);
        }
        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            LogMessage(message);
        }
        public void LogImportant(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            LogMessage("IMPORTANT: " + message);
        }
        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            LogMessage("WARNING: " + message);
        }
        public void LogError(string message, Exception ex = null)
        {
            LogImportant("ERROR: " + message + " - " + ex.Message);
        }

        private void LogMessage(string message)
        {
            Console.WriteLine(message);
            if (!string.IsNullOrEmpty(LogFile))
            {
                using (StreamWriter sw = File.AppendText(LogFile))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}
