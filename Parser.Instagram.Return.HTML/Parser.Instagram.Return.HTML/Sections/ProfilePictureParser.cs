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

    public class ProfilePictureParser : SectionParser
    {
        public ProfilePictureParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
            : base(logger, fileInfo, isPreservation, preservationPrefix, version)
        {
        }

        #region Properties
        private string LinkedMediaFile { get; set; }
        protected override bool HasData { get { return !string.IsNullOrEmpty(LinkedMediaFile); } }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();

            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("LinkedMediaFile");
            data.Columns.Add("File");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            DataRow row = data.NewRow();
            row["LinkedMediaFile"] = !string.IsNullOrEmpty(LinkedMediaFile) ? LinkedMediaFile : null;
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
                ParseDataItem header = HtmlDoc.Items.FirstOrDefault();
                if (header != null && header.HasChildren && header.Children.Count() == 1)
                {
                    LinkedMediaFile = header.Children.First().Value;
                }
            }

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }
        #endregion
    }
}
