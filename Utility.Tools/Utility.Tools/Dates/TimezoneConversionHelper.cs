using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TechShare.Utility.Tools.Dates
{
    public static class TimezoneConversionHelper
    {
        private static Dictionary<string, TimeZoneInfo> timezoneDictionary = null;
        private static void PopulateTimezoneDictionary()
        {
            if (timezoneDictionary == null)
            {
                timezoneDictionary = new Dictionary<string, TimeZoneInfo>();
                IEnumerable<TimeZoneInfo> systemTZs = TimeZoneInfo.GetSystemTimeZones();

                timezoneDictionary.Add("UT", TimeZoneInfo.FindSystemTimeZoneById("UTC"));
                timezoneDictionary.Add("GMT", TimeZoneInfo.FindSystemTimeZoneById("UTC"));
                timezoneDictionary.Add("UTC", TimeZoneInfo.FindSystemTimeZoneById("UTC"));
                timezoneDictionary.Add("EST", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                timezoneDictionary.Add("EDT", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                timezoneDictionary.Add("CST", TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                timezoneDictionary.Add("CDT", TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                timezoneDictionary.Add("MST", TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"));
                timezoneDictionary.Add("MDT", TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"));
                timezoneDictionary.Add("PST", TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                timezoneDictionary.Add("PDT", TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            }
        }
        public static bool IsValidTimezone(string abbr)
        {
            PopulateTimezoneDictionary();
            return timezoneDictionary.ContainsKey(abbr);
        }
        public static TimeZoneInfo GetTimezoneByAbbreviation(string abbr)
        {
            PopulateTimezoneDictionary();

            if (timezoneDictionary.ContainsKey(abbr))
                return timezoneDictionary[abbr];
            else
                return TimeZoneInfo.Utc;
        }

        public static string Abbreviation(this TimeZoneInfo zone)
        {
            var zoneName = zone.Id;
            var zoneAbbr = zoneName.CapitalLetters();
            return zoneAbbr;
        }

        public static string CapitalLetters(this string str)
        {
            return str.Transform(c => char.IsUpper(c)
                ? c.ToString(CultureInfo.InvariantCulture)
                : null);
        }

        private static string Transform(this string src, Func<char, string> transformation)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return src;
            }

            var result = src.Select(transformation)
                .Where(res => res != null)
                .ToList();

            return string.Join("", result);
        }
    }
}
