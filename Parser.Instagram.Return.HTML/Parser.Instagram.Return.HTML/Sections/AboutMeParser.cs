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
    public class AboutMeParser : SectionParser
    {
        public AboutMeParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
            : base(logger, fileInfo, isPreservation, preservationPrefix, version)
        {
            if (isPreservation)
            {
                List<PreservationQuery> pq = new List<PreservationQuery>();
                pq.Add(new PreservationQuery()
                {
                    PreservationTableName = MainTableName,
                    QueryText = string.Format(
                         @"SELECT 
                                (CASE WHEN a.[AboutMe] IS NULL THEN 'true' ELSE 'false' END) as [MissingInCurrent],
                                p.[AboutMe] as [P_AboutMe], 
                                a.[AboutMe],
                                (case when LTRIM(RTRIM(p.[AboutMe])) <> LTRIM(RTRIM(a.[AboutMe])) THEN 'true' ELSE 'false' END) AS [AboutMe_Changed],
                                a.[File]     
                            FROM {0} p 
                            LEFT JOIN {1} a ON a.[AboutMe] = p.[AboutMe]", MainTableName, MainTableName.Replace(preservationPrefix + "_", ""))
                });
                PreservationQueries = pq;
            }
        }

        #region Properties
        private string AboutMe { get; set; }
        protected override bool HasData { get { return !string.IsNullOrEmpty(AboutMe); } }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();

            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("AboutMe");
            data.Columns.Add("File");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            DataRow row = data.NewRow();
            row["AboutMe"] = !string.IsNullOrEmpty(AboutMe) ? AboutMe : null;
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
            IEnumerable<ParseDataItem> htmlItems = HtmlDoc.Items.Where(x => !x.Header.ToUpper().Contains("DEFINITION"));
            if (htmlItems != null && htmlItems.Any())
            {
                foreach (ParseDataItem item in htmlItems)
                {
                    if (item.Header.ToUpper() == "ABOUT ME" && !string.IsNullOrEmpty(item.Value) && !item.Value.StartsWith("No responsive records", StringComparison.InvariantCultureIgnoreCase))
                        AboutMe = item.Value;
                }
            }
            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }
        #endregion

    }
}
