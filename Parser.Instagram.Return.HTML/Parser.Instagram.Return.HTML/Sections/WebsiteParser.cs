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

    public class WebsiteParser : SectionParser
    {
        public WebsiteParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
            : base(logger, fileInfo, isPreservation, preservationPrefix, version)
        {
        }

        #region Properties
        private string URL { get; set; }
        protected override bool HasData { get { return !string.IsNullOrEmpty(URL); } }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();

            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("URL");
            data.Columns.Add("File");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            DataRow row = data.NewRow();
            row["URL"] = !string.IsNullOrEmpty(URL) ? URL : null;
            row["File"] = SourceFile;
            data.Rows.Add(row);

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
                IEnumerable<ParseDataItem> htmlItems = HtmlDoc.Items.Where(x => !x.Header.ToUpper().Contains("DEFINITION"));
                //IEnumerable<ParseDataItem> toSearch = htmlItems.Count() > 1 ? htmlItems : htmlItems.ElementAt(0).Children;
                foreach (ParseDataItem item in htmlItems)
                {
                    if (item.Header.ToUpper() == "WEBSITE" && !string.IsNullOrEmpty(item.Value) &&
                        !item.Value.StartsWith("No responsive records", StringComparison.InvariantCultureIgnoreCase))
                        URL = item.Value;
                }
            }
            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }
        #endregion
    }
}
