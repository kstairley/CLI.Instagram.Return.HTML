using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TechShare.Parser.Instagram.Return.HTML.Support;
using TechShare.Utility.Tools.Exceptions;
using TechShare.Utility.Tools.HtmlParse;
using TechShare.Utility.Tools.Logs;
using TechShare.Utility.Tools.Zip;

namespace TechShare.Parser.Instagram.Return.HTML.Sections
{
    public class IncomingFollowRequestsParser : SectionParser
    {
        public IncomingFollowRequestsParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
            : base(logger, fileInfo, isPreservation, preservationPrefix, version)
        {
        }

        #region Properties
        protected override bool HasData
        {
            get
            {
                return Items != null && Items.Any(x => x.HasData);
            }
        }
        private IEnumerable<InstagramObject> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();

            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("UserName", typeof(string));
            data.Columns.Add("UserId", typeof(string));
            data.Columns.Add("UserFullName", typeof(string));
            data.Columns.Add("File");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (InstagramObject item in Items.Where(x => x.HasData))
            {
                DataRow row = data.NewRow();
                row["UserName"] = !string.IsNullOrEmpty(item.Name) ? item.Name : null;
                row["UserId"] = !string.IsNullOrEmpty(item.Id) ? item.Id : null;
                row["UserFullName"] = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : null;
                row["File"] = SourceFile;
                data.Rows.Add(row);
            }

            retVal.Add(data);

            return retVal;
        }
        public override IEnumerable<LocationDataPoint> GenerateLocationInformation()
        {
            return null;
        }
        protected override void ProcessHTML()
        {
            if (HtmlDoc != null && HtmlDoc.HasData)
            {
                List<InstagramObject> items = new List<InstagramObject>();
                IEnumerable<ParseDataItem> htmlItems = HtmlDoc.Items.Where(x => !x.Header.ToUpper().Contains("DEFINITION"));
                foreach (ParseDataItem item in htmlItems)
                {
                    if (item.HasValues)
                    {
                        foreach (string value in item.Values)
                        {
                            if (!string.IsNullOrEmpty(value) && !value.StartsWith("No responsive records", StringComparison.InvariantCultureIgnoreCase))
                            {
                                InstagramObject fo = new InstagramObject(value);
                                if (fo.HasData)
                                    items.Add(fo);
                            }
                        }
                    }
                }
                if (items.Count == 0)
                    throw new SectionEmptyException(DisplaySectionName);

                Items = items;
            }

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }
        #endregion
    }
}
