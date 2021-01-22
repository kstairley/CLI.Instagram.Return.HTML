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
    public class DirectStoriesParser : SectionParser
    {
        public DirectStoriesParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
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
        private IEnumerable<DirectStory> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();
            retVal.Add(GenerateDirectStoryTable());
            retVal.Add(GenerateDirectStoryRecipientTable());
            return retVal;
        }
        public DataTable GenerateDirectStoryTable()
        {
            DataTable retVal = new DataTable(MainTableName);
            retVal.Columns.Add("MediaId", typeof(string));
            retVal.Columns.Add("TimeUTC", typeof(string));
            retVal.Columns.Add("AuthorName", typeof(string));
            retVal.Columns.Add("AuthorId", typeof(string));
            retVal.Columns.Add("LinkedMediaFile", typeof(string));
            retVal.Columns.Add("File", typeof(string));

            if (Items != null && Items.Any(x => x.HasData))
            {
                foreach (DirectStory s in Items.Where(x => x.HasData))
                {
                    if (s.HasData)
                    {
                        DataRow row = retVal.NewRow();
                        row["MediaId"] = !string.IsNullOrEmpty(s.MediaId) ? s.MediaId : null;
                        row["TimeUTC"] = !string.IsNullOrEmpty(s.Time) ? s.Time : null;
                        row["AuthorName"] = !string.IsNullOrEmpty(s.AuthorName) ? s.AuthorName : null;
                        row["AuthorId"] = !string.IsNullOrEmpty(s.AuthorId) ? s.AuthorId : null;
                        row["LinkedMediaFile"] = !string.IsNullOrEmpty(s.LinkedMediaFile) ? s.LinkedMediaFile : null;
                        row["File"] = SourceFile;
                        retVal.Rows.Add(row);
                    }
                }
            }

            return retVal;
        }
        public DataTable GenerateDirectStoryRecipientTable()
        {
            DataTable retVal = new DataTable(MainTableName + "_Recipients");
            retVal.Columns.Add("MediaId", typeof(string));
            retVal.Columns.Add("RecipientName", typeof(string));
            retVal.Columns.Add("RecipientId", typeof(string));

            if (Items != null && Items.Any(x => x.HasRecipients))
            {
                foreach (DirectStory s in Items.Where(x => x.HasRecipients))
                {
                    foreach (InstagramObject recipient in s.Recipients)
                    {
                        if (recipient.HasData)
                        {
                            DataRow row = retVal.NewRow();
                            row["MediaId"] = !string.IsNullOrEmpty(s.MediaId) ? s.MediaId : null;
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
                List<DirectStory> items = new List<DirectStory>();
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
                            DirectStory newItem = new DirectStory(Logger, DisplaySectionName, components);
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
                        DirectStory newItem = new DirectStory(Logger, DisplaySectionName, components);
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
        internal class DirectStory
        {
            internal DirectStory(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
            {
                if (dataItems != null && dataItems.Any())
                {
                    foreach (ParseDataItem dataItem in dataItems)
                    {
                        switch (dataItem.Header.Trim().ToUpper())
                        {
                            case "MEDIA ID":
                                MediaId = dataItem.Value;
                                break;
                            case "TIME":
                                Time = dataItem.Value;
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
            internal string MediaId { get; set; }
            internal string Time { get; set; }
            internal string LinkedMediaFile { get; set; }
            internal InstagramObject Author { get; set; }
            internal string AuthorName { get { return Author != null && !string.IsNullOrEmpty(Author.Name) ? Author.Name : null; } }
            internal string AuthorId { get { return Author != null && !string.IsNullOrEmpty(Author.Id) ? Author.Id : null; } }
            internal IEnumerable<InstagramObject> Recipients { get; set; }
            internal bool HasRecipients { get { return Recipients != null && Recipients.Any(x => x.HasData); } }
            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(MediaId) ||
                            !string.IsNullOrEmpty(Time) ||
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
