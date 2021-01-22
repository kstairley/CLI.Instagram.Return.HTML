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
    public class UnifiedMessagesParser : SectionParser
    {
        public UnifiedMessagesParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
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
        private IEnumerable<Thread> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();

            retVal.Add(GenerateUnifiedMessageTable());
            retVal.Add(GenerateMessageAttachmentsTable());
            retVal.Add(GenerateMessageSharesTable());
            retVal.Add(GenerateMessageUsersTable());
            retVal.Add(GenerateMessageCallRecordsTable());

            return retVal;
        }
        public override IEnumerable<LocationDataPoint> GenerateLocationInformation()
        {
            return null;
        }
        private DataTable GenerateUnifiedMessageTable()
        {
            DataTable data = new DataTable(MainTableName);
            data.Columns.Add("ThreadId");
            data.Columns.Add("MessageGuid");
            data.Columns.Add("MessageSequence");
            data.Columns.Add("CurrentThreadDateUTC");
            data.Columns.Add("AuthorUserName");
            data.Columns.Add("AuthorUserId");
            data.Columns.Add("SentUTC");
            data.Columns.Add("Ip");
            data.Columns.Add("IsDeleted");
            data.Columns.Add("Body");
            data.Columns.Add("MarketplaceId");
            data.Columns.Add("RemoveBySenderUTC");
            data.Columns.Add("HasCallRecordInfo");
            data.Columns.Add("HasAttachmentInfo");
            data.Columns.Add("HasShare");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (Thread item in Items.Where(x => x.HasMessages))
            {
                foreach (Thread.Message m in item.Messages)
                {
                    DataRow row = data.NewRow();
                    row["ThreadId"] = !string.IsNullOrEmpty(item.ID) ? item.ID : null;
                    row["MessageGuid"] = !string.IsNullOrEmpty(m.GUID) ? m.GUID : null;
                    row["MessageSequence"] = !string.IsNullOrEmpty(m.Sequence.ToString()) ? m.Sequence.ToString() : null;
                    row["CurrentThreadDateUTC"] = !string.IsNullOrEmpty(item.ThreadDate) ? item.ThreadDate : null;
                    row["AuthorUserName"] = !string.IsNullOrEmpty(m.AuthorName) ? m.AuthorName : null;
                    row["AuthorUserId"] = !string.IsNullOrEmpty(m.AuthorId) ? m.AuthorId : null;
                    row["SentUTC"] = !string.IsNullOrEmpty(m.Sent) ? m.Sent : null;
                    row["Ip"] = !string.IsNullOrEmpty(m.IPAddress) ? m.IPAddress : null;
                    row["IsDeleted"] = !string.IsNullOrEmpty(m.IsDeleted) ? m.IsDeleted : null;
                    row["Body"] = !string.IsNullOrEmpty(m.Body) ? m.Body : null;
                    row["MarketplaceId"] = !string.IsNullOrEmpty(m.MarketplaceId) ? m.MarketplaceId : null;
                    row["RemoveBySenderUTC"] = !string.IsNullOrEmpty(m.RemoveBySender) ? m.RemoveBySender : null;
                    row["HasCallRecordInfo"] = m.HasCallRecords ? Boolean.TrueString : Boolean.FalseString;
                    row["HasAttachmentInfo"] = m.HasAttachments ? Boolean.TrueString : Boolean.FalseString;
                    row["HasShare"] = m.HasShares ? Boolean.TrueString : Boolean.FalseString;

                    data.Rows.Add(row);
                }
            }

            return data;
        }
        private DataTable GenerateMessageAttachmentsTable()
        {
            DataTable data = new DataTable(MainTableName + "_Attachments");
            data.Columns.Add("MessageGuid");
            data.Columns.Add("AttName");
            data.Columns.Add("AttId");
            data.Columns.Add("AttType");
            data.Columns.Add("AttSize");
            data.Columns.Add("Url");
            data.Columns.Add("LinkedMediaFile");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (Thread item in Items.Where(x => x.HasMessages))
            {
                foreach (Thread.Message m in item.Messages.Where(x => x.HasAttachments))
                {
                    foreach (Thread.Message.Attachment a in m.Attachments)
                    {
                        DataRow row = data.NewRow();
                        row["MessageGuid"] = !string.IsNullOrEmpty(m.GUID) ? m.GUID : null;
                        row["AttName"] = !string.IsNullOrEmpty(a.Name) ? a.Name : null;
                        row["AttId"] = !string.IsNullOrEmpty(a.Id) ? a.Id : null;
                        row["AttType"] = !string.IsNullOrEmpty(a.MIMEType) ? a.MIMEType : null;
                        row["AttSize"] = !string.IsNullOrEmpty(a.Size) ? a.Size : null;
                        row["Url"] = !string.IsNullOrEmpty(a.URL) ? a.URL : null;
                        row["LinkedMediaFile"] = !string.IsNullOrEmpty(a.LinkedMediaFile) ? a.LinkedMediaFile : null;
                        data.Rows.Add(row);
                    }
                }
            }
            return data;
        }
        private DataTable GenerateMessageSharesTable()
        {
            DataTable data = new DataTable(MainTableName + "_Shares");
            data.Columns.Add("MessageGuid");
            data.Columns.Add("DateCreatedUTC");
            data.Columns.Add("Link");
            data.Columns.Add("Summary");
            data.Columns.Add("TextStr");
            data.Columns.Add("Title");
            data.Columns.Add("Url");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (Thread item in Items.Where(x => x.HasMessages))
            {
                foreach (Thread.Message m in item.Messages.Where(x => x.HasShares))
                {
                    foreach (Thread.Message.Share s in m.Shares)
                    {
                        DataRow row = data.NewRow();
                        row["MessageGuid"] = !string.IsNullOrEmpty(m.GUID) ? m.GUID : null;
                        row["DateCreatedUTC"] = !string.IsNullOrEmpty(s.DateCreated) ? s.DateCreated : null;
                        row["Link"] = !string.IsNullOrEmpty(s.Link) ? s.Link : null;
                        row["Summary"] = !string.IsNullOrEmpty(s.Summary) ? s.Summary : null;
                        row["TextStr"] = !string.IsNullOrEmpty(s.Text) ? s.Text : null;
                        row["Title"] = !string.IsNullOrEmpty(s.Title) ? s.Title : null;
                        row["Url"] = !string.IsNullOrEmpty(s.URL) ? s.URL : null;
                        data.Rows.Add(row);
                    }
                }
            }
            return data;
        }
        private DataTable GenerateMessageUsersTable()
        {
            DataTable data = new DataTable(MainTableName + "_Recipients");
            data.Columns.Add("MessageGuid");
            data.Columns.Add("UserName");
            data.Columns.Add("UserId");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (Thread item in Items.Where(x => x.HasMessages))
            {
                foreach (Thread.Message m in item.Messages)
                {
                    if (item.HasCurrentParticipants)
                    {
                        foreach (InstagramObject fo in item.CurrentParticipants)
                        {
                            DataRow row = data.NewRow();
                            row["MessageGuid"] = !string.IsNullOrEmpty(m.GUID) ? m.GUID : null;
                            row["UserName"] = fo.HasData && !string.IsNullOrEmpty(fo.Name) ? fo.Name : null;
                            row["UserId"] = fo.HasData && !string.IsNullOrEmpty(fo.Id) ? fo.Id : null;
                            data.Rows.Add(row);
                        }
                    }
                    if (item.HasPastParticipants)
                    {
                        foreach (InstagramObject fo in item.PastParticipants)
                        {
                            DataRow row = data.NewRow();
                            row["MessageGuid"] = !string.IsNullOrEmpty(m.GUID) ? m.GUID : null;
                            row["UserName"] = fo.HasData && !string.IsNullOrEmpty(fo.Name) ? fo.Name : null;
                            row["UserId"] = fo.HasData && !string.IsNullOrEmpty(fo.Id) ? fo.Id : null;
                            data.Rows.Add(row);
                        }
                    }
                    if (m.HasRecipients)
                    {
                        foreach (InstagramObject fo in m.Recipients)
                        {
                            DataRow row = data.NewRow();
                            row["MessageGuid"] = !string.IsNullOrEmpty(m.GUID) ? m.GUID : null;
                            row["UserName"] = fo.HasData && !string.IsNullOrEmpty(fo.Name) ? fo.Name : null;
                            row["UserId"] = fo.HasData && !string.IsNullOrEmpty(fo.Id) ? fo.Id : null;
                            data.Rows.Add(row);
                        }
                    }
                }
            }
            return data;
        }
        private DataTable GenerateMessageCallRecordsTable()
        {
            DataTable data = new DataTable(MainTableName + "_CallRecords");
            data.Columns.Add("MessageGuid");
            data.Columns.Add("CallType");
            data.Columns.Add("IsCallMissed");
            data.Columns.Add("DurationInSec");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (Thread item in Items.Where(x => x.HasMessages))
            {
                foreach (Thread.Message m in item.Messages.Where(x => x.HasCallRecords))
                {
                    foreach (Thread.Message.CallRecord c in m.CallRecords)
                    {

                        DataRow row = data.NewRow();
                        row["MessageGuid"] = !string.IsNullOrEmpty(m.GUID) ? m.GUID : null;
                        row["CallType"] = !string.IsNullOrEmpty(c.CallType) ? c.CallType : null;
                        row["IsCallMissed"] = !string.IsNullOrEmpty(c.Missed) ? c.Missed : null;
                        row["DurationInSec"] = !string.IsNullOrEmpty(c.Duration) ? c.Duration : null;
                        data.Rows.Add(row);
                    }
                }
            }
            return data;
        }
        private DataTable GenerateMessageSubscriptionEventsTable()
        {
            DataTable data = new DataTable(MainTableName + "_SubscriptionEvents");
            data.Columns.Add("ThreadId");
            data.Columns.Add("SubscriptionType");
            data.Columns.Add("UserId");
            data.Columns.Add("UserName");

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);

            foreach (Thread item in Items.Where(x => x.HasMessages))
            {
                foreach (Thread.Message m in item.Messages)
                {
                    foreach (Thread.SubscriptionEvent s in m.SubscriptionEvents.Where(x => x.HasUsers))
                    {
                        foreach (InstagramObject user in s.Users.Where(x => x.HasData))
                        {
                            DataRow row = data.NewRow();
                            row["ThreadId"] = !string.IsNullOrEmpty(item.ID) ? item.ID : null;
                            row["SubscriptionType"] = !string.IsNullOrEmpty(s.Type) ? s.Type : null;
                            row["UserId"] = !string.IsNullOrEmpty(user.Id) ? user.Id : null;
                            row["UserName"] = !string.IsNullOrEmpty(user.Name) ? user.Name : null;
                            data.Rows.Add(row);
                        }
                    }
                }
            }
            return data;
        }

        protected override void ProcessHTML()
        {
            if (HtmlDoc != null && HtmlDoc.HasData)
            {
                IEnumerable<ParseDataItem> validItems = HtmlDoc.Items.Where(x => !string.IsNullOrEmpty(x.Header));
                IEnumerable<ParseDataItem> toSearch = (validItems.Count() > 1 || validItems.ElementAt(0).HasValues) ?
                    (!validItems.ElementAt(0).Header.EndsWith(" Definition", StringComparison.CurrentCultureIgnoreCase) ?
                    validItems : validItems.ElementAt(1).Children) :
                    validItems.ElementAt(0).Children;
                List<ParseDataItem> messages = null;

                List<Thread> items = new List<Thread>();
                string firstItem = string.Empty;
                if (toSearch != null && toSearch.Any())
                {
                    //List<ParseDataItem> components = null;
                    foreach (ParseDataItem item in toSearch)
                    {
                        if (item.HasValues && item.HasChildren)
                        {
                            if (string.IsNullOrEmpty(firstItem))
                                firstItem = item.Header;

                            string threadId = string.Empty;
                            string threadDate = string.Empty;

                            //int startIndex = 0;
                            int itemCount = 0;
                            foreach (ParseDataItem.ValueCount vc in item.ValueCounts)
                            {
                                if (vc.Val.Contains("UTC"))
                                    threadDate = vc.Val;
                                else
                                    threadId = vc.Val;

                                List<ParseDataItem> currentParticipants = null;
                                List<ParseDataItem> pastParticipants = null;
                                List<ParseDataItem> marketplaceIdItems = null;
                                List<ParseDataItem> children = item.Children.ToList();
                                List<ParseDataItem> subset = children.GetRange(itemCount, vc.AssociatedChildCount);
                                int participantIndex = 0;
                                bool noMessages = true;
                                while (true && participantIndex < subset.Count)
                                {
                                    if (subset[participantIndex].Header.ToUpper().Contains("PARTICIPANTS") ||
                                        subset[participantIndex].Header.ToUpper().Contains("MARKETPLACE"))
                                    {
                                        if (subset[participantIndex].Header.ToUpper().Trim() == "CURRENT PARTICIPANTS")
                                        {
                                            if (currentParticipants == null)
                                                currentParticipants = new List<ParseDataItem>();
                                            currentParticipants.Add(subset[participantIndex]);
                                        }
                                        if (subset[participantIndex].Header.ToUpper().Trim() == "PAST PARTICIPANTS")
                                        {
                                            if (pastParticipants == null)
                                                pastParticipants = new List<ParseDataItem>();
                                            pastParticipants.Add(subset[participantIndex]);
                                        }
                                        if (subset[participantIndex].Header.ToUpper().Trim() == "MARKETPLACE ID")
                                        {
                                            if (marketplaceIdItems == null)
                                                marketplaceIdItems = new List<ParseDataItem>();
                                            marketplaceIdItems.Add(subset[participantIndex]);
                                        }
                                        participantIndex++;
                                    }
                                    else
                                    {
                                        noMessages = false;
                                        Thread newItem = new Thread(Logger, DisplaySectionName, threadId, threadDate, marketplaceIdItems, currentParticipants, pastParticipants, subset.GetRange(participantIndex, subset.Count() - participantIndex));
                                        if (newItem.HasData)
                                            items.Add(newItem);
                                        currentParticipants = null;
                                        pastParticipants = null;
                                        marketplaceIdItems = null;
                                        break;
                                    }
                                }
                                if (noMessages)
                                {
                                    Thread newItem = new Thread(Logger, DisplaySectionName, threadId, threadDate, marketplaceIdItems, currentParticipants, pastParticipants, null);
                                    if (newItem.HasData)
                                        items.Add(newItem);
                                    currentParticipants = null;
                                    pastParticipants = null;
                                    marketplaceIdItems = null;
                                }
                                itemCount += vc.AssociatedChildCount;
                            }
                        }
                    }
                }

                Items = items;
            }

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }
        #endregion

        #region Internal Classes
        internal class Thread
        {
            internal Thread(CommandLineLogger logger, string dataSectionName, string threadId, string threadDate, IEnumerable<ParseDataItem> marketplaceIdItems, IEnumerable<ParseDataItem> currentParticipants, IEnumerable<ParseDataItem> pastParticipants, IEnumerable<ParseDataItem> messages)
            {
                ID = !string.IsNullOrEmpty(threadId) ? threadId : Guid.NewGuid().ToString();
                ThreadDate = threadDate;

                List<InstagramObject> currentPart = new List<InstagramObject>();
                List<InstagramObject> pastPart = new List<InstagramObject>();
                List<InstagramObject> participants = new List<InstagramObject>();

                if (currentParticipants != null && currentParticipants.Any())
                {
                    foreach (ParseDataItem cpItem in currentParticipants)
                    {
                        if (cpItem.HasValues)
                        {
                            foreach (string val in cpItem.Values)
                            {
                                if (val.Contains("UTC"))
                                    threadDate = val;
                                else
                                {
                                    InstagramObject fo = new InstagramObject(val);
                                    if (fo.HasData)
                                    {
                                        currentPart.Add(fo);
                                    }
                                }
                            }
                        }
                    }
                    if (currentPart.Any())
                        participants.AddRange(currentPart);
                }

                if (pastParticipants != null && pastParticipants.Any())
                {
                    foreach (ParseDataItem ppItem in pastParticipants)
                    {
                        if (ppItem.HasValues)
                        {
                            foreach (string val in ppItem.Values)
                            {
                                if (val.Contains("UTC"))
                                    threadDate = val;
                                else
                                {
                                    InstagramObject fo = new InstagramObject(val);
                                    if (fo.HasData)
                                    {
                                        pastPart.Add(fo);
                                    }
                                }
                            }
                        }
                    }
                    if (pastPart.Any())
                        participants.AddRange(currentPart);
                }

                List<Message> items = new List<Message>();
                List<ParseDataItem> components = null;
                int messageCount = 1;
                string firstItem = string.Empty;
                if (messages != null && messages.Any())
                {
                    foreach (ParseDataItem item in messages)
                    {
                        if (string.IsNullOrEmpty(firstItem))
                            firstItem = item.Header;

                        if (item.Header.Equals(firstItem) && components != null && components.Any())
                        {
                            Message newItem = new Message(logger, dataSectionName, messageCount, marketplaceIdItems, participants, components);
                            if (newItem.HasData)
                            {
                                items.Add(newItem);
                                messageCount++;
                            }
                            components = null;
                        }
                        if (components == null)
                            components = new List<ParseDataItem>();
                        components.Add(item);
                    }

                    if (components != null && components.Any())
                    {
                        Message newItem = new Message(logger, dataSectionName, messageCount, marketplaceIdItems, participants, components);
                        if (newItem.HasData)
                        {
                            items.Add(newItem);
                            messageCount++;
                        }
                    }
                    if (items != null && items.Any())
                        Messages = items;
                }
            }
            #region Properties
            internal string ID { get; private set; }
            internal string ThreadDate { get; private set; }
            internal IEnumerable<InstagramObject> CurrentParticipants { get; private set; }
            internal IEnumerable<InstagramObject> PastParticipants { get; private set; }
            internal IEnumerable<Message> Messages { get; private set; }
            internal bool HasMessages { get { return (Messages != null && Messages.Any(x => x.HasData)); } }
            internal bool HasCurrentParticipants { get { return (CurrentParticipants != null && CurrentParticipants.Any(x => x.HasData)); } }
            internal bool HasPastParticipants { get { return (PastParticipants != null && PastParticipants.Any(x => x.HasData)); } }

            internal bool HasData
            {
                get
                {
                    return !string.IsNullOrEmpty(ID) ||
                        !string.IsNullOrEmpty(ThreadDate) ||
                        HasMessages ||
                        HasCurrentParticipants ||
                        HasPastParticipants;
                }
            }
            #endregion

            internal class Message
            {
                internal Message(CommandLineLogger logger, string dataSectionName, int msgNum, IEnumerable<ParseDataItem> marketplaceIdItems, IEnumerable<InstagramObject> participants, IEnumerable<ParseDataItem> messageItems)
                {
                    List<InstagramObject> recipientList = new List<InstagramObject>();

                    GUID = Guid.NewGuid().ToString();
                    Sequence = msgNum;

                    if (marketplaceIdItems != null && marketplaceIdItems.Any())
                    {
                        foreach (ParseDataItem marketplaceIdItem in marketplaceIdItems)
                        {
                            switch (marketplaceIdItem.Header.ToUpper().Trim())
                            {
                                case "MARKETPLACE ID":
                                    MarketplaceId = marketplaceIdItem.Value;
                                    break;
                                default:
                                    logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + " - Message (Marketplace): " + marketplaceIdItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
                                    //_log.LogWarning("Unknown Section - \"Message:" + headerNode.InnerText + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                    throw new ApplicationException(marketplaceIdItem.Header);
#endif
                                    // break;
                            }
                        }
                    }

                    foreach (ParseDataItem messageItem in messageItems)
                    {
                        switch (messageItem.Header.ToUpper().Trim())
                        {
                            case "RECIPIENTS":
                                if (messageItem.HasValues)
                                {
                                    foreach (string recipient in messageItem.Values)
                                    {
                                        InstagramObject fo = new InstagramObject(recipient);
                                        if (fo.HasData)
                                        {
                                            recipientList.Add(fo);
                                        }
                                    }
                                }
                                break;
                            case "AUTHOR":
                                Author = new InstagramObject(messageItem.Value);
                                break;
                            case "SENT":
                                Sent = messageItem.Value;
                                break;
                            case "DELETED":
                                IsDeleted = messageItem.Value;
                                break;
                            case "BODY":
                                Body = messageItem.Value;
                                break;
                            case "IP":
                                IPAddress = messageItem.Value;
                                break;
                            case "CALL RECORD":
                                CallRecord cr = new CallRecord(logger, dataSectionName, messageItem.Children);
                                if (cr.HasData)
                                    CallRecords = new List<CallRecord>() { cr };
                                break;
                            case "ATTACHMENTS":
                                List<Attachment> items = new List<Attachment>();
                                int startIndex = 0;
                                foreach (ParseDataItem.ValueCount vc in messageItem.ValueCounts)
                                {
                                    string value = vc.Val;
                                    List<ParseDataItem> components = null;
                                    for (int i = startIndex; i < startIndex + vc.AssociatedChildCount; i++)
                                    {
                                        if (components == null)
                                            components = new List<ParseDataItem>();
                                        ParseDataItem childItem = messageItem.Children.ElementAt(i);
                                        if (childItem.HasData)
                                        {
                                            components.Add(childItem);
                                        }
                                    }
                                    Attachment att = new Attachment(logger, dataSectionName, value, components);
                                    if (att.HasData)
                                        items.Add(att);
                                    startIndex += vc.AssociatedChildCount;
                                }
                                Attachments = items;
                                break;
                            case "SHARE":
                                Share share = new Share(logger, dataSectionName, messageItem);
                                if (share.HasData)
                                    Shares = new List<Share>() { share };
                                break;
                            case "MARKETPLACE ID":
                                MarketplaceId = messageItem.Value;
                                break;
                            case "SUBSCRIPTION EVENT":
                                if (messageItem.HasChildren)
                                {
                                    SubscriptionEvent subscriptionEvent = new SubscriptionEvent(logger, dataSectionName, messageItem.Children);
                                    if (subscriptionEvent.HasData)
                                        SubscriptionEvents = new List<SubscriptionEvent>() { subscriptionEvent };
                                }
                                break;
                            case "REMOVED BY SENDER":
                                RemoveBySender = messageItem.Value;
                                break;
                            default:
                                logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + " - Message: " + messageItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
                                //_log.LogWarning("Unknown Section - \"Message:" + headerNode.InnerText + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                throw new ApplicationException(messageItem.Header);
#endif
                                // break;
                        }
                    }

                    if (participants != null && participants.Any(x => x.HasData))
                    {
                        recipientList.AddRange(participants.Where(x => x.HasData));
                    }

                    if (recipientList.Count() > 0)
                        Recipients = recipientList;
                }
                #region Properties
                internal int Sequence { get; private set; }
                internal string GUID { get; private set; }
                internal IEnumerable<InstagramObject> Recipients { get; private set; }
                internal InstagramObject Author { get; private set; }
                internal string AuthorName { get { return (Author != null) ? Author.Name.Trim() : null; } }
                internal string AuthorId { get { return (Author != null) ? Author.Id.Trim() : null; } }
                internal string Sent { get; private set; }
                internal string IsDeleted { get; private set; }
                internal string Body { get; private set; }
                internal string MarketplaceId { get; private set; }
                internal string RemoveBySender { get; private set; }
                internal string IPAddress { get; private set; }
                internal IEnumerable<Attachment> Attachments { get; private set; }
                internal IEnumerable<Share> Shares { get; private set; }
                internal IEnumerable<CallRecord> CallRecords { get; private set; }
                internal bool HasRecipients { get { return (Recipients != null && Recipients.Count() > 0); } }
                internal bool HasAttachments { get { return (Attachments != null && Attachments.Count() > 0); } }
                internal bool HasShares { get { return (Shares != null && Shares.Count() > 0); } }
                internal bool HasCallRecords { get { return (CallRecords != null && CallRecords.Count() > 0); } }
                internal IEnumerable<SubscriptionEvent> SubscriptionEvents { get; private set; }
                internal bool HasSubscriptionEvents { get { return (SubscriptionEvents != null && SubscriptionEvents.Any(x => x.HasData)); } }
                internal bool HasData
                {
                    get
                    {
                        return ((Recipients != null && Recipients.Count() > 0) ||
                            (Author.HasData) ||
                            !string.IsNullOrEmpty(Sent) ||
                            !string.IsNullOrEmpty(IsDeleted) ||
                            !string.IsNullOrEmpty(Body) ||
                            !string.IsNullOrEmpty(MarketplaceId) ||
                            !string.IsNullOrEmpty(RemoveBySender) ||
                            !string.IsNullOrEmpty(IPAddress) ||
                            HasAttachments ||
                            HasShares ||
                            HasCallRecords);

                    }
                }
                #endregion

                internal class Attachment
                {
                    internal Attachment(CommandLineLogger logger, string dataSectionName, string value, IEnumerable<ParseDataItem> dataItems)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            InstagramObject fo = new InstagramObject(value);
                            if (fo.HasData)
                            {
                                Name = fo.Name;
                                Id = fo.Id;
                            }
                        }

                        foreach (ParseDataItem dataItem in dataItems)
                        {
                            string header = !string.IsNullOrEmpty(dataItem.Header) ? dataItem.Header : string.Empty;
                            switch (header.Trim().ToUpper())
                            {
                                case "TYPE":
                                    MIMEType = dataItem.Value;
                                    break;
                                case "SIZE":
                                    Size = dataItem.Value;
                                    break;
                                case "URL":
                                    URL = dataItem.Value;
                                    break;
                                case "":
                                    if (dataItem.HasChildren)
                                    {
                                        foreach (ParseDataItem childDataItem in dataItem.Children)
                                        {
                                            switch (childDataItem.Header.Trim().ToUpper())
                                            {
                                                case "LINKED MEDIA FILE:":
                                                    LinkedMediaFile = childDataItem.Value;
                                                    break;
                                                default:
                                                    logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + " - Comment: " + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                                    throw new ApplicationException(dataItem.Header);
#endif
                                                    // break;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + " - Message - Attachment:" + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
                                    //_log.LogWarning("Unknown Section - \"Message:Attachment:" + headerNode.InnerText + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                    throw new ApplicationException(dataItem.Header);
#endif
                                    // break;

                            }
                        }
                    }
                    #region Properties
                    internal string Name { get; private set; }
                    internal string Id { get; private set; }
                    internal string MIMEType { get; private set; }
                    internal string Size { get; private set; }
                    internal string URL { get; private set; }
                    internal string LinkedMediaFile { get; private set; }
                    internal bool HasData
                    {
                        get
                        {
                            return (!string.IsNullOrEmpty(Name) ||
                                !string.IsNullOrEmpty(Id) ||
                                !string.IsNullOrEmpty(MIMEType) ||
                                !string.IsNullOrEmpty(Size) |
                                !string.IsNullOrEmpty(URL) ||
                                !string.IsNullOrEmpty(LinkedMediaFile));
                        }
                    }
                    #endregion
                }
                internal class Share
                {
                    internal Share(CommandLineLogger logger, string dataSectionName, ParseDataItem shareItem)
                    {
                        if (shareItem != null && shareItem.HasChildren)
                        {
                            foreach (ParseDataItem dataItem in shareItem.Children)
                            {
                                switch (dataItem.Header.ToUpper().Trim())
                                {
                                    case "DATE CREATED":
                                        DateCreated = dataItem.Value;
                                        break;
                                    case "LINK":
                                        Link = dataItem.Value;
                                        break;
                                    case "SUMMARY":
                                        Summary = dataItem.Value;
                                        break;
                                    case "TEXT":
                                        Text = dataItem.Value;
                                        break;
                                    case "TITLE":
                                        Title = dataItem.Value;
                                        break;
                                    case "URL":
                                        URL = dataItem.Value;
                                        break;
                                    default:
                                        logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + " - Message - Share: " + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                        throw new ApplicationException(dataItem.Header);
#endif
                                        // break;

                                }
                            }
                        }
                    }

                    #region Properties
                    internal string DateCreated { get; private set; }
                    internal string Link { get; private set; }
                    internal string Summary { get; private set; }
                    internal string Text { get; private set; }
                    internal string Title { get; private set; }
                    internal string URL { get; private set; }
                    internal bool HasData
                    {
                        get
                        {
                            return (!string.IsNullOrEmpty(DateCreated) ||
                                    !string.IsNullOrEmpty(Link) ||
                                    !string.IsNullOrEmpty(Summary) ||
                                    !string.IsNullOrEmpty(Text) ||
                                    !string.IsNullOrEmpty(Title) ||
                                    !string.IsNullOrEmpty(URL));
                        }
                    }
                    #endregion
                }
                internal class CallRecord
                {
                    internal CallRecord(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
                    {
                        foreach (ParseDataItem dataItem in dataItems)
                        {
                            switch (dataItem.Header.ToUpper().Trim())
                            {
                                case "TYPE":
                                    CallType = dataItem.Value;
                                    break;
                                case "MISSED":
                                    Missed = dataItem.Value;
                                    break;
                                case "DURATION":
                                    Duration = dataItem.Value;
                                    break;
                                default:
                                    logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + " - Message - Call Record:" + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                    throw new ApplicationException(dataItem.Header);
#endif
                                    // break;
                            }
                        }
                    }
                    #region Properties
                    internal string CallType { get; private set; }
                    internal string Missed { get; private set; }
                    internal string Duration { get; private set; }
                    internal bool HasData
                    {
                        get
                        {
                            return (!string.IsNullOrEmpty(CallType) ||
                                !string.IsNullOrEmpty(Missed) ||
                                !string.IsNullOrEmpty(Duration));
                        }
                    }
                    #endregion
                }
            }

            internal class SubscriptionEvent
            {
                internal SubscriptionEvent(CommandLineLogger logger, string dataSectionName, IEnumerable<ParseDataItem> dataItems)
                {
                    if (dataItems != null && dataItems.Any())
                    {
                        foreach (ParseDataItem dataItem in dataItems)
                        {
                            switch (dataItem.Header.ToUpper().Trim())
                            {
                                case "TYPE":
                                    Type = dataItem.Value;
                                    break;
                                case "USERS":
                                    List<InstagramObject> users = new List<InstagramObject>();
                                    foreach (string user in dataItem.Values)
                                    {
                                        InstagramObject fo = new InstagramObject(user);
                                        if (fo.HasData)
                                            users.Add(fo);
                                    }
                                    if (users != null && users.Any(x => x.HasData))
                                        Users = users;
                                    break;
                                default:
                                    logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + " - Message - Subscription Event:" + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                    throw new ApplicationException(dataItem.Header);
#endif
                                    // break;
                            }
                        }
                    }
                }
                #region Properties
                internal string Type { get; private set; }
                internal IEnumerable<InstagramObject> Users { get; private set; }
                internal bool HasData
                {
                    get
                    {
                        return !string.IsNullOrEmpty(Type) ||
                            (Users != null && Users.Any(x => x.HasData));
                    }
                }
                internal bool HasUsers
                {
                    get
                    {
                        return Users != null && Users.Any(x => x.HasData);
                    }
                }
                #endregion
            }
        }
        #endregion
    }
}
