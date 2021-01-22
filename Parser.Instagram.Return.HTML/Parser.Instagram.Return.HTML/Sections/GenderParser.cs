using System.Collections.Generic;
using System.Data;
using TechShare.Parser.Instagram.Return.HTML.Support;
using TechShare.Utility.Tools.Exceptions;
using TechShare.Utility.Tools.HtmlParse;
using TechShare.Utility.Tools.Logs;
using TechShare.Utility.Tools.Zip;
namespace TechShare.Parser.Instagram.Return.HTML.Sections
{

    public class GenderParser : SectionParser
    {
        public GenderParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
            : base(logger, fileInfo, isPreservation, preservationPrefix, version)
        {
        }

        #region Properties
        private string Gender { get; set; }
        protected override bool HasData { get { return !string.IsNullOrEmpty(Gender); } }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();

            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("Gender");
            data.Columns.Add("File");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            DataRow row = data.NewRow();
            row["Gender"] = !string.IsNullOrEmpty(Gender) ? Gender : null;
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
                foreach (ParseDataItem item in HtmlDoc.Items)
                {
                    if (item.Header.ToUpper() == "GENDER")
                        Gender = item.Value;
                }
            }
            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }
        #endregion
    }
}
