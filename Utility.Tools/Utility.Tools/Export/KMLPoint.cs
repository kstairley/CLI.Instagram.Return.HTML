using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechShare.Utility.Tools.Export
{
    public class KMLPoint
    {
        public bool Extrude { get; set; }
        public string AltitudeMode { get; set; }
        public List<KMLCoordinate> _coordinates = null;
        public IEnumerable<KMLCoordinate> Coordinates { get { return _coordinates != null ? _coordinates.Where(x => x.HasData) : null; } }
        public bool HasCoordinates
        {
            get
            {
                return Coordinates != null && Coordinates.Any(x => x.HasData);
            }
        }
        public bool HasData
        {
            get
            {
                return !string.IsNullOrEmpty(AltitudeMode) ||
                    Extrude ||
                    HasCoordinates;
            }
        }
        public void AddCoordinate(double? latitude = null, double? longitude = null, double? altitude = null)
        {
            if (_coordinates == null)
                _coordinates = new List<KMLCoordinate>();

            KMLCoordinate toAdd = new KMLCoordinate()
            {
                Latitude = latitude,
                Longitude = longitude,
                Altitude = altitude
            };
            _coordinates.Add(toAdd);
        }
        public override string ToString()
        {
            string coords = null;
            if (HasCoordinates)
                coords = string.Join(Environment.NewLine, Coordinates.Select(x => x.ToString())) + Environment.NewLine;

            StringBuilder sb = new StringBuilder();
            if (Extrude)
                sb.AppendLine("<extrude>1</extrude>");
            if (!string.IsNullOrEmpty(AltitudeMode))
                sb.AppendLine("<altitudeMode>" + AltitudeMode + "</altitudeMode>");
            if (!string.IsNullOrEmpty(coords))
                sb.AppendLine("<coordinates>" + coords + "</coordinates>");

            string retVal = null;
            if (!string.IsNullOrEmpty(sb.ToString()))
            {

                retVal = "<Point>" + Environment.NewLine;
                retVal += sb.ToString();
                retVal += "</Point>";
            }
            return retVal;
        }
    }
}
