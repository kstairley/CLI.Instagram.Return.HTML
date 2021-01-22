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
    public class CommentsParser : SectionParser
    {
        public CommentsParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
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
        private IEnumerable<Comment> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();
            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("Id", typeof(string));
            data.Columns.Add("DateCreatedUTC", typeof(string));
            data.Columns.Add("Status", typeof(string));
            data.Columns.Add("Text", typeof(string));
            data.Columns.Add("MediaContentId", typeof(string));
            data.Columns.Add("MediaContentOwnerName", typeof(string));
            data.Columns.Add("MediaContentOwnerId", typeof(string));
            data.Columns.Add("File", typeof(string));


            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (Comment item in Items.Where(x => x.HasData))
            {
                DataRow row = data.NewRow();

                row["Id"] = !string.IsNullOrEmpty(item.CommentId) ? item.CommentId : null;
                row["DateCreatedUTC"] = !string.IsNullOrEmpty(item.DateCreatedUTC) ? item.DateCreatedUTC : null;
                row["Status"] = !string.IsNullOrEmpty(item.Status) ? item.Status : null;
                row["Text"] = !string.IsNullOrEmpty(item.Text) ? item.Text : null;
                row["MediaContentId"] = !string.IsNullOrEmpty(item.ContentId) ? item.ContentId : null;
                row["MediaContentOwnerName"] = !string.IsNullOrEmpty(item.ContentOwnerName) ? item.ContentOwnerName : null;
                row["MediaContentOwnerId"] = !string.IsNullOrEmpty(item.ContentOwnerId) ? item.ContentOwnerId : null;
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
                List<Comment> items = new List<Comment>();
                List<ParseDataItem> components = null;
                IEnumerable<ParseDataItem> htmlItems = HtmlDoc.Items.Where(x => !x.Header.ToUpper().Contains("DEFINITION"));
                IEnumerable<ParseDataItem> toSearch = htmlItems.Count() > 1 ? htmlItems : htmlItems.ElementAt(0).Children;
                if (toSearch != null && toSearch.Any())
                {
                    foreach (ParseDataItem item in toSearch)
                    {
                        if (string.IsNullOrEmpty(firstItem))
                            firstItem = item.Header;

                        if (item.Header.Equals(firstItem) && components != null && components.Any())
                        {
                            Comment newItem = new Comment(Logger, DisplaySectionName, components);
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
                        Comment newItem = new Comment(Logger, DisplaySectionName, components);
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
        internal class Comment
        {
            internal Comment(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
            {
                if (dataItems != null && dataItems.Any())
                {
                    foreach (ParseDataItem dataItem in dataItems)
                    {
                        switch (dataItem.Header.Trim().ToUpper())
                        {
                            case "ID":
                                CommentId = dataItem.Value;
                                break;
                            case "DATE CREATED":
                                DateCreatedUTC = dataItem.Value;
                                break;
                            case "STATUS":
                                Status = dataItem.Value;
                                break;
                            case "TEXT":
                                Text = dataItem.Value;
                                break;
                            case "MEDIA CONTENT ID":
                                ContentId = dataItem.Value;
                                break;
                            case "MEDIA OWNER":
                                ContentOwner = new InstagramObject(dataItem.Value);
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
            internal string CommentId { get; set; }
            internal string DateCreatedUTC { get; set; }
            internal string Status { get; set; }
            internal string Text { get; set; }
            internal string ContentId { get; set; }
            internal InstagramObject ContentOwner { get; set; }
            internal string ContentOwnerName { get { return ContentOwner != null && !string.IsNullOrEmpty(ContentOwner.Name) ? ContentOwner.Name : null; } }
            internal string ContentOwnerId { get { return ContentOwner != null && !string.IsNullOrEmpty(ContentOwner.Id) ? ContentOwner.Id : null; } }
            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(CommentId) ||
                            !string.IsNullOrEmpty(DateCreatedUTC) ||
                            !string.IsNullOrEmpty(Status) ||
                            !string.IsNullOrEmpty(Text) ||
                            !string.IsNullOrEmpty(ContentId) ||
                            ContentOwner.HasData);
                }
            }
        }
        #endregion
    }
}
