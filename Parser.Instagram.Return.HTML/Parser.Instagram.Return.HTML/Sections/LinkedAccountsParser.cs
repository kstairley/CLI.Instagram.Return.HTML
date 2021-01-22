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
    public class LinkedAccountsParser : SectionParser
    {
        public LinkedAccountsParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
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
        private IEnumerable<LinkedAccount> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();
            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("ServiceName");
            data.Columns.Add("Email");
            data.Columns.Add("Id");
            data.Columns.Add("File");


            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (LinkedAccount item in Items.Where(x => x.HasData))
            {
                DataRow row = data.NewRow();

                row["ServiceName"] = !string.IsNullOrEmpty(item.Service) ? item.Service : null;
                row["Email"] = !string.IsNullOrEmpty(item.Email) ? item.Email : null;
                row["Id"] = !string.IsNullOrEmpty(item.ServiceId) ? item.ServiceId : null;
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
                List<LinkedAccount> items = new List<LinkedAccount>();
                List<ParseDataItem> components = null;
                IEnumerable<ParseDataItem> htmlItems = HtmlDoc.Items.Where(x => !x.Header.ToUpper().Contains("DEFINITION"));
                IEnumerable<ParseDataItem> toSearch = htmlItems.Count() > 1 ? htmlItems : htmlItems.ElementAt(0).Children;
                //IEnumerable<ParseDataItem> toSearch = HtmlDoc.Items.Count() > 1 ? HtmlDoc.Items : HtmlDoc.Items.ElementAt(0).Children;
                if (toSearch != null && toSearch.Any())
                {
                    if (toSearch != null && toSearch.Any())
                    {
                        foreach (ParseDataItem item in toSearch)
                        {
                            if (string.IsNullOrEmpty(firstItem))
                                firstItem = item.Header;

                            if (item.Header.Equals(firstItem) && components != null && components.Any())
                            {
                                LinkedAccount newItem = new LinkedAccount(Logger, DisplaySectionName, components);
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
                            LinkedAccount newItem = new LinkedAccount(Logger, DisplaySectionName, components);
                            if (newItem.HasData)
                                items.Add(newItem);
                        }
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
        internal class LinkedAccount
        {
            internal LinkedAccount(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
            {
                if (dataItems != null && dataItems.Any())
                {
                    foreach (ParseDataItem dataItem in dataItems)
                    {
                        switch (dataItem.Header.Trim().ToUpper())
                        {
                            case "SERVICE":
                                Service = dataItem.Value;
                                break;
                            case "EMAIL":
                                Email = dataItem.Value;
                                break;
                            case "IDENTIFIER":
                                ServiceId = dataItem.Value;
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
            internal string Service { get; set; }
            internal string Email { get; set; }
            internal string ServiceId { get; set; }
            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(Service) ||
                            !string.IsNullOrEmpty(Email) ||
                            !string.IsNullOrEmpty(ServiceId));
                }
            }
        }
        #endregion
    }
}
