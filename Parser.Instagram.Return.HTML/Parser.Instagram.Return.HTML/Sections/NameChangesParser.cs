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
    public class NameChangesParser : SectionParser
    {
        public NameChangesParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
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
        private IEnumerable<NameChange> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();
            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("OldName");
            data.Columns.Add("NewName");
            data.Columns.Add("File");


            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (NameChange item in Items.Where(x => x.HasData))
            {
                DataRow row = data.NewRow();

                row["OldName"] = !string.IsNullOrEmpty(item.OldName) ? item.OldName : null;
                row["NewName"] = !string.IsNullOrEmpty(item.NewName) ? item.NewName : null;
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
                string firstItem = string.Empty;
                List<NameChange> items = new List<NameChange>();
                List<ParseDataItem> components = null;
                IEnumerable<ParseDataItem> htmlItems = HtmlDoc.Items.Where(x => !x.Header.ToUpper().Contains("DEFINITION"));
                IEnumerable<ParseDataItem> toSearch = htmlItems.Count() > 1 ? htmlItems : htmlItems.ElementAt(0).Children;
                //IEnumerable<ParseDataItem> toSearch = HtmlDoc.Items.Count() > 1 ? HtmlDoc.Items : HtmlDoc.Items.ElementAt(0).Children;
                if (toSearch != null && toSearch.Any())
                {
                    foreach (ParseDataItem item in toSearch)
                    {
                        if (string.IsNullOrEmpty(firstItem))
                            firstItem = item.Header;

                        if (item.Header.Equals(firstItem) && components != null && components.Any())
                        {
                            NameChange newItem = new NameChange(Logger, DisplaySectionName, components);
                            if (newItem.HasData)
                                items.Add(newItem);
                            components = null;
                        }
                        if (components == null)
                            components = new List<ParseDataItem>();
                        components.Add(item);
                    }

                    if (components != null && components.Any())
                    {
                        NameChange newItem = new NameChange(Logger, DisplaySectionName, components);
                        if (newItem.HasData)
                            items.Add(newItem);
                    }

                    if (items.Count == 0)
                        throw new SectionEmptyException(DisplaySectionName);

                    Items = items;
                }
            }

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }
        #endregion

        #region Internal Classes
        internal class NameChange
        {
            internal NameChange(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
            {
                if (dataItems != null && dataItems.Any())
                {
                    foreach (ParseDataItem dataItem in dataItems)
                    {
                        switch (dataItem.Header.Trim().ToUpper())
                        {
                            case "OLD NAME":
                                OldName = dataItem.Value;
                                break;
                            case "NEW NAME":
                                NewName = dataItem.Value;
                                break;
                            default:
                                logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + ": " + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                throw new ApplicationException(dataItem.Header);
#endif
                                break;
                        }
                    }
                }
            }
            internal string OldName { get; set; }
            internal string NewName { get; set; }
            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(OldName) ||
                            !string.IsNullOrEmpty(NewName));
                }
            }
        }
        #endregion
    }
}
