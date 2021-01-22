using System;

namespace TechShare.Utility.Tools.Maps
{
    public static class PrecisionLocationHelper
    {
        public static double? ConvertE7ToStandard(double? loc)
        {
            return loc.HasValue ? loc.Value / 10000000 : (double?)null;
        }

        public static double? ConvertStandardToE7(double? loc)
        {
            return loc.HasValue ? Math.Round(loc.Value * 10000000, 0) : (double?)null;
        }
    }
}
