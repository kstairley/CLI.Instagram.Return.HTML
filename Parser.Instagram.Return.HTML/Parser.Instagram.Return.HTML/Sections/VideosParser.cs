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
    public class VideosParser : SectionParser
    {
        public VideosParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
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
        private IEnumerable<Video> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();
            retVal.Add(GenerateVideoTable());
            retVal.Add(GenerateVideoCommentsTable());
            return retVal;
        }
        public DataTable GenerateVideoTable()
        {
            DataTable retVal = new DataTable(MainTableName);

            retVal.Columns.Add("VideoId", typeof(string));
            retVal.Columns.Add("TakenUTC", typeof(string));
            retVal.Columns.Add("ExpireAtUTC", typeof(string));
            retVal.Columns.Add("Status", typeof(string));
            retVal.Columns.Add("Url", typeof(string));
            retVal.Columns.Add("Source", typeof(string));
            retVal.Columns.Add("Filter", typeof(string));
            retVal.Columns.Add("IsPublished", typeof(string));
            retVal.Columns.Add("SharedByAuthor", typeof(string));
            retVal.Columns.Add("UploadIp", typeof(string));
            retVal.Columns.Add("CarouselId", typeof(string));
            retVal.Columns.Add("LinkedMediaFile", typeof(string));
            retVal.Columns.Add("File", typeof(string));

            if (Items != null && Items.Any(x => x.HasData))
            {
                foreach (Video v in Items.Where(x => x.HasData))
                {
                    DataRow row = retVal.NewRow();
                    row["VideoId"] = !string.IsNullOrEmpty(v.Id) ? v.Id : null;
                    row["TakenUTC"] = !string.IsNullOrEmpty(v.Taken) ? v.Taken : null;
                    row["ExpireAtUTC"] = !string.IsNullOrEmpty(v.ExpireAtUTC) ? v.ExpireAtUTC : null;
                    row["Status"] = !string.IsNullOrEmpty(v.Status) ? v.Status : null;
                    row["Url"] = !string.IsNullOrEmpty(v.Url) ? v.Url : null;
                    row["Source"] = !string.IsNullOrEmpty(v.Source) ? v.Source : null;
                    row["Filter"] = !string.IsNullOrEmpty(v.Filter) ? v.Filter : null;
                    row["IsPublished"] = !string.IsNullOrEmpty(v.IsPublished) ? v.IsPublished : null;
                    row["SharedByAuthor"] = !string.IsNullOrEmpty(v.SharedByAuthor) ? v.SharedByAuthor : null;
                    row["UploadIp"] = !string.IsNullOrEmpty(v.UploadIp) ? v.UploadIp : null;
                    row["CarouselId"] = !string.IsNullOrEmpty(v.CarouselId) ? v.CarouselId : null;
                    row["LinkedMediaFile"] = !string.IsNullOrEmpty(v.LinkedMediaFile) ? v.LinkedMediaFile : null;
                    row["File"] = SourceFile;
                    retVal.Rows.Add(row);
                }
            }
            return retVal;
        }

        public DataTable GenerateVideoCommentsTable()
        {
            DataTable retVal = new DataTable(MainTableName + "_Comments");

            retVal.Columns.Add("VideoId", typeof(string));
            retVal.Columns.Add("CommentId", typeof(string));
            retVal.Columns.Add("UserName", typeof(string));
            retVal.Columns.Add("UserNameId", typeof(string));
            retVal.Columns.Add("UserDisplayName", typeof(string));
            retVal.Columns.Add("DateCreatedUTC", typeof(string));
            retVal.Columns.Add("Text", typeof(string));

            if (Items != null && Items.Any(x => x.HasData))
            {
                foreach (Video v in Items.Where(x => x.HasComments))
                {
                    foreach (Video.Comment c in v.Comments)
                    {
                        DataRow row = retVal.NewRow();
                        row["VideoId"] = !string.IsNullOrEmpty(v.Id) ? v.Id : null;
                        row["CommentId"] = !string.IsNullOrEmpty(c.Id) ? c.Id : null;
                        row["UserName"] = !string.IsNullOrEmpty(c.UserName) ? c.UserName : null;
                        row["UserNameId"] = !string.IsNullOrEmpty(c.UserId) ? c.UserId : null;
                        row["UserDisplayName"] = !string.IsNullOrEmpty(c.UserDisplayName) ? c.UserDisplayName : null;
                        row["DateCreatedUTC"] = !string.IsNullOrEmpty(c.DateCreatedUTC) ? c.DateCreatedUTC : null;
                        row["Text"] = !string.IsNullOrEmpty(c.Text) ? c.Text : null;
                        retVal.Rows.Add(row);
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
                List<Video> items = new List<Video>();
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
                            Video newItem = new Video(Logger, DisplaySectionName, components);
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
                        Video newItem = new Video(Logger, DisplaySectionName, components);
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
        internal class Video
        {
            internal Video(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
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
                            case "TAKEN":
                                Taken = dataItem.Value;
                                break;
                            case "EXPIRE AT":
                                ExpireAtUTC = dataItem.Value;
                                break;
                            case "STATUS":
                                Status = dataItem.Value;
                                break;
                            case "URL":
                                Url = dataItem.Value;
                                break;
                            case "SOURCE":
                                Source = dataItem.Value;
                                break;
                            case "FILTER":
                                Filter = dataItem.Value;
                                break;
                            case "IS PUBLISHED":
                                IsPublished = dataItem.Value;
                                break;
                            case "SHARED BY AUTHOR":
                                SharedByAuthor = dataItem.Value;
                                break;
                            case "UPLOAD IP":
                                UploadIp = dataItem.Value;
                                break;
                            case "CAROUSEL ID":
                                CarouselId = dataItem.Value;
                                break;
                            case "VIDEO":
                                if (dataItem.HasChildren && dataItem.Children.Count() == 1)
                                {
                                    foreach (ParseDataItem childDataItem in dataItem.Children)
                                    {
                                        switch (childDataItem.Header.Trim().ToUpper())
                                        {
                                            case "LINKED MEDIA FILE:":
                                                LinkedMediaFile = childDataItem.Value;
                                                break;
                                            default:
                                                logger.LogWarning("Unknown Section - \"Photos - " + dataItem.Header + " - " + childDataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                                throw new ApplicationException(childDataItem.Header);
#endif
                                                break;
                                        }
                                    }
                                }
                                break;
                            case "COMMENTS":
                                List<Comment> records = new List<Comment>();
                                List<ParseDataItem> components = null;
                                if (dataItem.HasChildren && dataItem.Children.Count() == 1)
                                {
                                    string firstItem = string.Empty;
                                    foreach (ParseDataItem childDataItem in dataItem.Children)
                                    {
                                        if (string.IsNullOrEmpty(firstItem))
                                            firstItem = childDataItem.Header;

                                        if (childDataItem.Header.Equals(firstItem) && components != null && components.Any())
                                        {
                                            Comment newItem = new Comment(logger, dataSectionName, components);
                                            if (newItem.HasData)
                                                records.Add(newItem);
                                            components = null;
                                        }
                                        if (components == null)
                                            components = new List<ParseDataItem>();
                                        components.Add(childDataItem);
                                    }

                                    if (components != null && components.Any())
                                    {
                                        Comment newItem = new Comment(logger, dataSectionName, components);
                                        if (newItem.HasData)
                                            records.Add(newItem);
                                    }
                                }

                                if (records != null && records.Any(x => x.HasData))
                                    Comments = records;
                                break;

                            default:
                                logger.LogWarning("Unknown Section - \"Videos:" + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                throw new ApplicationException(dataItem.Header);
#endif
                                break;
                        }
                    }
                }
            }
            public string Id { get; set; }
            public string Taken { get; set; }
            public string ExpireAtUTC { get; set; }
            public string Status { get; set; }
            public string Url { get; set; }
            public string Source { get; set; }
            public string Filter { get; set; }
            public string IsPublished { get; set; }
            public string SharedByAuthor { get; set; }
            public string UploadIp { get; set; }
            public string CarouselId { get; set; }
            public string LinkedMediaFile { get; set; }
            public IEnumerable<Comment> Comments { get; set; }
            public bool HasComments { get { return Comments != null && Comments.Any(x => x.HasData); } }
            public bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(Id) ||
                            !string.IsNullOrEmpty(Taken) ||
                            !string.IsNullOrEmpty(ExpireAtUTC) ||
                            !string.IsNullOrEmpty(Status) ||
                            !string.IsNullOrEmpty(Url) ||
                            !string.IsNullOrEmpty(Source) ||
                            !string.IsNullOrEmpty(Filter) ||
                            !string.IsNullOrEmpty(IsPublished) ||
                            !string.IsNullOrEmpty(SharedByAuthor) ||
                            !string.IsNullOrEmpty(UploadIp) ||
                            !string.IsNullOrEmpty(CarouselId) ||
                            !string.IsNullOrEmpty(LinkedMediaFile) ||
                            HasComments);
                }
            }
            internal class Comment
            {
                public Comment(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
                {
                    if (dataItems != null && dataItems.Any())
                    {
                        foreach (ParseDataItem dataItem in dataItems)
                        {
                            switch (dataItem.Header.Trim().ToUpper())
                            {
                                case "USER":
                                    User = new InstagramObject(dataItem.Value);
                                    break;
                                case "ID":
                                    Id = dataItem.Value;
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
                                default:
                                    logger.LogWarning("Unknown Section - \"Videos - Comment:" + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                    throw new ApplicationException(dataItem.Header);
#endif
                                    break;
                            }
                        }
                    }
                }

                public string VideoId { get; set; }
                public InstagramObject User { get; set; }
                public string UserName { get { return User != null && !string.IsNullOrEmpty(User.Name) ? User.Name : null; } }
                public string UserId { get { return User != null && !string.IsNullOrEmpty(User.Id) ? User.Id : null; } }
                public string UserDisplayName { get { return User != null && !string.IsNullOrEmpty(User.DisplayName) ? User.DisplayName : null; } }
                public string Id { get; set; }
                public string DateCreatedUTC { get; set; }
                public string Status { get; set; }
                public string Text { get; set; }

                public bool HasData
                {
                    get
                    {
                        return (!string.IsNullOrEmpty(Id) ||
                                (User != null && User.HasData) ||
                                !string.IsNullOrEmpty(DateCreatedUTC) ||
                                !string.IsNullOrEmpty(Status) ||
                                !string.IsNullOrEmpty(Text));
                    }
                }
            }
        }
        #endregion
    }
}
