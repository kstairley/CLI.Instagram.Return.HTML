using System;

namespace TechShare.Utility.Tools.Logs
{
    /**
     * Logging interface.
     */
    public interface iLogger
    {
        bool log(string message);

        string LogFile { get; }
        void LogInfoAlert(string message);
        void LogInfo(string message);
        void LogImportant(string message);
        void LogWarning(string message);
        void LogError(string message, Exception ex = null);
    }
}