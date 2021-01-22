using System;

namespace TechShare.Utility.Tools.Dates
{
    public static class DateTimeHelper
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime? FromUnixTime(double? unixTime)
        {
            return unixTime.HasValue ? (unixTime.Value > 9999999999 ? epoch.AddMilliseconds(unixTime.Value) : epoch.AddSeconds(unixTime.Value)) : (DateTime?)null;
        }

        public static double? ToUnixTime(DateTime? dateValue)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dateValue.HasValue ? Convert.ToDouble((dateValue.Value - epoch).TotalSeconds) : (double?)null;
        }
    }
}
