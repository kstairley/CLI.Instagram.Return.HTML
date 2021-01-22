using System.Text;

namespace TechShare.Utility.Tools.Export
{
    public class KMLPlacemark
    {
        public KMLPlacemark(string name, string description, double? latitude = null, double? longitude = null, double? altitude = null)
        {
            Name = name;
            Description = description;
            if (latitude.HasValue && longitude.HasValue)
            {
                Point = new KMLPoint();
                Point.AddCoordinate(latitude, longitude, altitude);
            }
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public KMLPoint Point { get; set; }
        public bool HasPoint { get { return Point != null && Point.HasData; } }
        public bool HasData
        {
            get
            {
                return !string.IsNullOrEmpty(Name) ||
                    !string.IsNullOrEmpty(Description) ||
                    HasPoint;
            }
        }
        public override string ToString()
        {
            string retVal = null;
            if (Point != null)
            {
                string pointString = Point.ToString();
                if (!string.IsNullOrEmpty(pointString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<Placemark>");
                    if (!string.IsNullOrEmpty(Name))
                        sb.AppendLine("<name>" + Name + "</name>");
                    if (!string.IsNullOrEmpty(Description))
                        sb.AppendLine("<description>" + Name + "</description>");
                    sb.AppendLine(pointString);
                    sb.AppendLine("</Placemark>");
                    retVal = sb.ToString();
                }
            }
            return retVal;
        }
    }
}
