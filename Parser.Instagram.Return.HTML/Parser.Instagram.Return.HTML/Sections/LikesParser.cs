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
    public class LikesParser : SectionParser
    {
        public LikesParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
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
        private IEnumerable<Like> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();
            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("Id");
            data.Columns.Add("TakenUTC");
            data.Columns.Add("Status");
            data.Columns.Add("Url");
            data.Columns.Add("Source");
            data.Columns.Add("Filter");
            data.Columns.Add("UploadIp");
            data.Columns.Add("IsPublished");
            data.Columns.Add("SharedByAuthor");
            data.Columns.Add("CarouselId");
            data.Columns.Add("File");


            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (Like item in Items.Where(x => x.HasData))
            {
                DataRow row = data.NewRow();

                row["Id"] = !string.IsNullOrEmpty(item.ID) ? item.ID : null;
                row["TakenUTC"] = !string.IsNullOrEmpty(item.Taken) ? item.Taken : null;
                row["Status"] = !string.IsNullOrEmpty(item.Status) ? item.Status : null;
                row["Url"] = !string.IsNullOrEmpty(item.URL) ? item.URL : null;
                row["Source"] = !string.IsNullOrEmpty(item.Source) ? item.Source : null;
                row["Filter"] = !string.IsNullOrEmpty(item.Filter) ? item.Filter : null;
                row["UploadIp"] = !string.IsNullOrEmpty(item.UploadIP) ? item.UploadIP : null;
                row["IsPublished"] = !string.IsNullOrEmpty(item.IsPublished) ? item.IsPublished : null;
                row["SharedByAuthor"] = !string.IsNullOrEmpty(item.SharedByAuthor) ? item.SharedByAuthor : null;
                row["CarouselId"] = !string.IsNullOrEmpty(item.CarouselId) ? item.CarouselId : null;
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
                List<Like> items = new List<Like>();
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
                            Like newItem = new Like(Logger, DisplaySectionName, components);
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
                        Like newItem = new Like(Logger, DisplaySectionName, components);
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
        internal class Like
        {
            internal Like(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
            {
                if (dataItems != null && dataItems.Any())
                {
                    foreach (ParseDataItem dataItem in dataItems)
                    {
                        switch (dataItem.Header.Trim().ToUpper())
                        {
                            case "ID":
                                ID = dataItem.Value;
                                break;
                            case "TAKEN":
                                Taken = dataItem.Value;
                                break;
                            case "STATUS":
                                Status = dataItem.Value;
                                break;
                            case "URL":
                                URL = dataItem.Value;
                                break;
                            case "SOURCE":
                                Source = dataItem.Value;
                                break;
                            case "FILTER":
                                Filter = dataItem.Value;
                                break;
                            case "UPLOAD IP":
                                UploadIP = dataItem.Value;
                                break;
                            case "IS PUBLISHED":
                                IsPublished = dataItem.Value;
                                break;
                            case "SHARED BY AUTHOR":
                                SharedByAuthor = dataItem.Value;
                                break;
                            case "CAROUSEL ID":
                                CarouselId = dataItem.Value;
                                break;
                            case "MEDIA":
                                //Skip this ase because it has no data, and shows when the like has no responsive records
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
            internal string ID { get; set; }
            internal string Taken { get; set; }
            internal string Status { get; set; }
            internal string URL { get; set; }
            internal string Source { get; set; }
            internal string Filter { get; set; }
            internal string UploadIP { get; set; }
            internal string IsPublished { get; set; }
            internal string SharedByAuthor { get; set; }
            internal string CarouselId { get; set; }

            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(ID) ||
                            !string.IsNullOrEmpty(Taken) ||
                            !string.IsNullOrEmpty(Status) ||
                            !string.IsNullOrEmpty(URL) ||
                            !string.IsNullOrEmpty(Source) ||
                            !string.IsNullOrEmpty(Filter) ||
                            !string.IsNullOrEmpty(UploadIP) ||
                            !string.IsNullOrEmpty(IsPublished) ||
                            !string.IsNullOrEmpty(SharedByAuthor) ||
                            !string.IsNullOrEmpty(CarouselId));
                }
            }
        }
        #endregion
    }
}
