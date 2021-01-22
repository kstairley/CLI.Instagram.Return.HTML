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

    public class PopularBlockParser : SectionParser
    {
        public PopularBlockParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
            : base(logger, fileInfo, isPreservation, preservationPrefix, version)
        {
        }

        #region Properties
        private string Blocked { get; set; }
        protected override bool HasData { get { return !string.IsNullOrEmpty(Blocked); } }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();

            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("Blocked");
            data.Columns.Add("File");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            DataRow row = data.NewRow();
            row["Blocked"] = !string.IsNullOrEmpty(Blocked) ? Blocked : null;
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
                //IEnumerable<ParseDataItem> htmlItems = HtmlDoc.Items.Where(x => !x.Header.ToUpper().Contains("DEFINITION"));
                //IEnumerable<ParseDataItem> toSearch = htmlItems.Count() > 1 ? htmlItems : htmlItems.ElementAt(0).Children;
                ParseDataItem header = HtmlDoc.Items.FirstOrDefault();
                if (header != null && header.HasChildren && header.Children.Count() == 1)
                {
                    Blocked = header.Children.First().Value;
                }
            }

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }
        #endregion
    }
}
