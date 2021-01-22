using System;

namespace TechShare.Utility.Tools.Extensions
{
    public static class TimespanExtensions
    {
        public static string GetFormattedElapsedTime(this TimeSpan ts)
        {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
        }
    }
}
