using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechShare.Utility.Tools.Export
{
    public class KMLDocument
    {
        #region Properties
        private List<KMLPlacemark> _placemarks = null;
        public IEnumerable<KMLPlacemark> Placemarks { get { return _placemarks; } }
        public bool HasData
        {
            get
            {
                return HasPlacemarks;
            }
        }
        public bool HasPlacemarks
        {
            get { return Placemarks != null && Placemarks.Any(x => x.HasData); }
        }
        #endregion

        #region Functions
        public void AddPlacemark(KMLPlacemark toAdd)
        {
            if (_placemarks == null)
                _placemarks = new List<KMLPlacemark>();

            if (toAdd.HasData)
                _placemarks.Add(toAdd);
        }
        public void AddPlacemark(string name, string description, double? latitude = null, double? longitude = null, double? altitude = null)
        {
            AddPlacemark(new KMLPlacemark(name, description, latitude, longitude, altitude));
        }
        public override string ToString()
        {
            string retVal = string.Empty;
            if (Placemarks != null && Placemarks.Any(x => !string.IsNullOrEmpty(x.ToString())))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<kml xmlns=\'http://www.opengis.net/kml/2.2'>");
                sb.AppendLine("<Document>");
                foreach (KMLPlacemark placeMark in Placemarks.Where(x => x.HasData))
                    sb.AppendLine(placeMark.ToString());
                sb.AppendLine("</Document>");
                sb.AppendLine("</kml>");
                retVal = sb.ToString();
            }
            return retVal;
        }
        #endregion
    }
}
