using System.Collections.Generic;
using System.Linq;

namespace TechShare.Utility.Tools.Export
{
    public class KMLCoordinate
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Altitude { get; set; }
        public bool HasData { get { return Latitude.HasValue && Longitude.HasValue; } }
        public override string ToString()
        {
            string retVal = null;
            if (HasData)
            {
                List<string> toJoin = new List<string>()
                {
                    Longitude.Value.ToString(),
                    Latitude.Value.ToString(),
                    Altitude.HasValue ? Altitude.Value.ToString() : string.Empty
                };
                retVal = string.Join(",", toJoin.Where(x => !string.IsNullOrEmpty(x)));
            }
            return retVal;
        }
    }
}
