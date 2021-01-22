using System;

namespace TechShare.Utility.Tools.Export
{
    public interface IKMLPlacemark
    {
        DateTime? TimestampUTC { get; set; }
        string TimestampCompareString { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        double? Latitude { get; set; }
        double? Longitude { get; set; }
        double? Altitude { get; set; }
        bool HasData { get; }
    }
}
