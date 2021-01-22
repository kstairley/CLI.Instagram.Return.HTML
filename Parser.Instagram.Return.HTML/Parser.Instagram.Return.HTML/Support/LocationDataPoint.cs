using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechShare.Parser.Instagram.Return.HTML.Support
{
    public class LocationDataPoint
    {
        public string LocationName { get; set; }
        public string LocationId { get; set; }
        public string ReportedOn { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ReportType { get; set; }
        public bool IsPreservation { get; set; }
        public string PresearvationSource { get; set; }
        public bool HasData
        {
            get
            {
                return !string.IsNullOrEmpty(LocationName) ||
                    !string.IsNullOrEmpty(LocationId) ||
                    !string.IsNullOrEmpty(ReportedOn) ||
                    !string.IsNullOrEmpty(Latitude) ||
                    !string.IsNullOrEmpty(Longitude) ||
                    !string.IsNullOrEmpty(PresearvationSource) ||
                    !string.IsNullOrEmpty(ReportType);
            }
        }

        public static DataTable GenerateLocationTable(IEnumerable<LocationDataPoint> points)
        {
            DataTable retVal = new DataTable("ReportLocationInformation");

            retVal.Columns.Add("LocationName");
            retVal.Columns.Add("LocationId");
            retVal.Columns.Add("TimeReportedUTC");
            retVal.Columns.Add("Latitude");
            retVal.Columns.Add("Longitude");
            retVal.Columns.Add("ReportType");
            retVal.Columns.Add("IsPresearvation");
            retVal.Columns.Add("PresearvationSource");

            if (points != null && points.Any())
            {
                foreach (LocationDataPoint point in points)
                {
                    DataRow row = retVal.NewRow();

                    row["LocationName"] = !string.IsNullOrEmpty(point.LocationName) ? point.LocationName : null;
                    row["LocationId"] = !string.IsNullOrEmpty(point.LocationId) ? point.LocationId : null;
                    row["TimeReportedUTC"] = !string.IsNullOrEmpty(point.ReportedOn) ? point.ReportedOn : null;
                    row["Latitude"] = !string.IsNullOrEmpty(point.Latitude) ? point.Latitude : null;
                    row["Longitude"] = !string.IsNullOrEmpty(point.Longitude) ? point.Longitude : null;
                    row["ReportType"] = !string.IsNullOrEmpty(point.ReportType) ? point.ReportType : null;
                    row["IsPresearvation"] = point.IsPreservation ? Boolean.TrueString : Boolean.FalseString;
                    row["PresearvationSource"] = !string.IsNullOrEmpty(point.PresearvationSource) ? point.PresearvationSource : null;

                    retVal.Rows.Add(row);
                }
            }

            return retVal;
        }
    }
}
