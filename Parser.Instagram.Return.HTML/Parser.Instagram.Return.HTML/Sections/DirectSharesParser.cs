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
    public class DirectSharesParser : SectionParser
    {
        public DirectSharesParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
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
        private IEnumerable<DirectShare> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();
            retVal.Add(GenerateDirectShareTable());
            retVal.Add(GenerateDirectShareRecipientTable());
            return retVal;
        }
        public DataTable GenerateDirectShareTable()
        {
            DataTable retVal = new DataTable(MainTableName);
            retVal.Columns.Add("ThreadId", typeof(string));
            retVal.Columns.Add("Id", typeof(string));
            retVal.Columns.Add("TimeUTC", typeof(string));
            retVal.Columns.Add("Uri", typeof(string));
            retVal.Columns.Add("ItemType", typeof(string));
            retVal.Columns.Add("IPAddress", typeof(string));
            retVal.Columns.Add("Text", typeof(string));
            retVal.Columns.Add("Hashtag", typeof(string));
            retVal.Columns.Add("AuthorName", typeof(string));
            retVal.Columns.Add("AuthorId", typeof(string));
            retVal.Columns.Add("RecipientsString", typeof(string));
            retVal.Columns.Add("LinkedMediaFile", typeof(string));
            retVal.Columns.Add("File", typeof(string));

            if (Items != null && Items.Any(x => x.HasData))
            {
                foreach (DirectShare s in Items.Where(x => x.HasData))
                {
                    DataRow row = retVal.NewRow();
                    row["ThreadId"] = !string.IsNullOrEmpty(s.ThreadId) ? s.ThreadId : null;
                    row["Id"] = !string.IsNullOrEmpty(s.Id) ? s.Id : null;
                    row["TimeUTC"] = !string.IsNullOrEmpty(s.Time) ? s.Time : null;
                    row["Uri"] = !string.IsNullOrEmpty(s.Uri) ? s.Uri : null;
                    row["ItemType"] = !string.IsNullOrEmpty(s.ItemType) ? s.ItemType : null;
                    row["IPAddress"] = !string.IsNullOrEmpty(s.IPAddress) ? s.IPAddress : null;
                    row["Text"] = !string.IsNullOrEmpty(s.Text) ? s.Text : null;
                    row["Hashtag"] = !string.IsNullOrEmpty(s.Hashtag) ? s.Hashtag : null;
                    row["AuthorName"] = !string.IsNullOrEmpty(s.AuthorName) ? s.AuthorName : null;
                    row["AuthorId"] = !string.IsNullOrEmpty(s.AuthorId) ? s.AuthorId : null;
                    row["RecipientsString"] = !string.IsNullOrEmpty(s.RecipientsString) ? s.RecipientsString : null;
                    row["LinkedMediaFile"] = !string.IsNullOrEmpty(s.LinkedMediaFile) ? s.LinkedMediaFile : null;
                    row["File"] = SourceFile;
                    retVal.Rows.Add(row);
                }
            }

            return retVal;
        }
        public DataTable GenerateDirectShareRecipientTable()
        {
            DataTable retVal = new DataTable(MainTableName + "_Recipients");
            retVal.Columns.Add("ShareId", typeof(string));
            retVal.Columns.Add("RecipientName", typeof(string));
            retVal.Columns.Add("RecipientId", typeof(string));

            if (Items != null && Items.Any(x => x.HasRecipients))
            {
                foreach (DirectShare s in Items.Where(x => x.HasRecipients))
                {
                    foreach (InstagramObject recipient in s.Recipients)
                    {
                        if (recipient.HasData)
                        {
                            DataRow row = retVal.NewRow();
                            row["ShareId"] = !string.IsNullOrEmpty(s.Id) ? s.Id : null;
                            row["RecipientName"] = !string.IsNullOrEmpty(recipient.Name) ? recipient.Name : null;
                            row["RecipientId"] = !string.IsNullOrEmpty(recipient.Id) ? recipient.Id : null;
                            retVal.Rows.Add(row);
                        }
                    }
                }
            }
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
                List<DirectShare> items = new List<DirectShare>();
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
                            DirectShare newItem = new DirectShare(Logger, DisplaySectionName, components);
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
                        DirectShare newItem = new DirectShare(Logger, DisplaySectionName, components);
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
        internal class DirectShare
        {
            internal DirectShare(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
            {
                if (dataItems != null && dataItems.Any())
                {
                    foreach (ParseDataItem dataItem in dataItems)
                    {
                        switch (dataItem.Header.Trim().ToUpper())
                        {
                            case "ID":
                                Id = dataItem.Value;
                                break;
                            case "TIME":
                                Time = dataItem.Value;
                                break;
                            case "URI":
                                Uri = dataItem.Value;
                                break;
                            case "ITEM-TYPE":
                            case "ITEM TYPE":
                                ItemType = dataItem.Value;
                                break;
                            case "IP-ADDRESS":
                            case "IP ADDRESS":
                                IPAddress = dataItem.Value;
                                break;
                            case "THREAD-ID":
                            case "THREAD ID":
                                ThreadId = dataItem.Value;
                                break;
                            case "TEXT":
                                Text = dataItem.Value;
                                break;
                            case "HASHTAG":
                                Hashtag = dataItem.Value;
                                break;
                            case "AUTHOR":
                                InstagramObject fo = new InstagramObject(dataItem.Value);
                                if (fo.HasData)
                                    Author = fo;
                                break;
                            case "RECIPIENTS":
                                if (dataItem.HasValues)
                                {
                                    List<InstagramObject> recipients = null;
                                    foreach (string value in dataItem.Values)
                                    {
                                        {
                                            InstagramObject newRecipient = new InstagramObject(value);
                                            if (newRecipient.HasData)
                                            {
                                                if (recipients == null)
                                                    recipients = new List<InstagramObject>();
                                                recipients.Add(newRecipient);
                                            }
                                        }
                                    }
                                    if (recipients != null && recipients.Any())
                                    {
                                        Recipients = recipients;
                                    }
                                }
                                break;

                            case "LINKED MEDIA FILE:":
                                LinkedMediaFile = dataItem.Value;
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
            #region Properties
            internal string ThreadId { get; set; }
            internal string Id { get; set; }
            internal string Time { get; set; }
            internal string Uri { get; set; }
            internal string ItemType { get; set; }
            internal string IPAddress { get; set; }
            internal string Text { get; set; }
            internal string Hashtag { get; set; }
            internal InstagramObject Author { get; set; }
            internal string AuthorName { get { return Author != null && !string.IsNullOrEmpty(Author.Name) ? Author.Name : null; } }
            internal string AuthorId { get { return Author != null && !string.IsNullOrEmpty(Author.Id) ? Author.Id : null; } }
            internal IEnumerable<InstagramObject> Recipients { get; set; }
            internal string RecipientsString { get { return HasRecipients ? string.Join(", ", Recipients.Select(x => x.ToString())) : null; } }
            internal bool HasRecipients { get { return Recipients != null && Recipients.Any(x => x.HasData); } }
            internal string LinkedMediaFile { get; set; }
            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(ThreadId) ||
                            !string.IsNullOrEmpty(Id) ||
                            !string.IsNullOrEmpty(Time) ||
                            !string.IsNullOrEmpty(Uri) ||
                            !string.IsNullOrEmpty(ItemType) ||
                            !string.IsNullOrEmpty(IPAddress) ||
                            !string.IsNullOrEmpty(Text) ||
                            !string.IsNullOrEmpty(Hashtag) ||
                            !string.IsNullOrEmpty(LinkedMediaFile) ||
                            Author != null && Author.HasData ||
                            HasRecipients);
                }
            }
            #endregion
        }
        #endregion
    }
}
