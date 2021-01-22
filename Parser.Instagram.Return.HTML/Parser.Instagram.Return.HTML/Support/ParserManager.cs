using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TechShare.Parser.Instagram.Return.HTML.Sections;
using TechShare.Utility.Tools.Exceptions;
using TechShare.Utility.Tools.Logs;
using TechShare.Utility.Tools.Zip;
using TechShare.Parser.Instagram.Return.HTML.Support;

namespace TechShare.Parser.Instagram.Return.HTML.Support
{
    public class ParserManager
    {
        public ParserManager(CommandLineLogger log)
        {
            _log = log;
        }
        #region variables and properties
        private readonly CommandLineLogger _log = null;

        public string CaseNumber { get; set; }
        public string HtmlToLoad { get; set; }
        public string HtmlToRecord { get; set; }
        public bool IsPreservation { get; set; }
        public string DatabasePreservationNoPrefix { get; set; }
        public string DefaultDirectory { get; set; }
        public ParserVersionEnum Version { get; set; }
        public IEnumerable<LocationDataPoint> LocationData { get; private set; }
        public IEnumerable<PreservationQuery> PreservationQueries { get; private set; }
        #endregion

        internal void AboutMeParse(ExtractFileInfo fileInfo)
        {
            AboutMeParser sectionParser = new AboutMeParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void AccountStatusHistoryParse(ExtractFileInfo fileInfo)
        {
            AccountStatusHistoryParser sectionParser = new AccountStatusHistoryParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void CommentsParse(ExtractFileInfo fileInfo)
        {
            CommentsParser sectionParser = new CommentsParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void DevicesParse(ExtractFileInfo fileInfo)
        {
            DevicesParser sectionParser = new DevicesParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void DirectDebitParse(ExtractFileInfo fileInfo)
        {
            DirectDebitParser sectionParser = new DirectDebitParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void DirectSharesParse(ExtractFileInfo fileInfo)
        {
            DirectSharesParser sectionParser = new DirectSharesParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void DirectStoriesParse(ExtractFileInfo fileInfo)
        {
            DirectStoriesParser sectionParser = new DirectStoriesParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void FollowersParse(ExtractFileInfo fileInfo)
        {
            FollowersParser sectionParser = new FollowersParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void FollowingParse(ExtractFileInfo fileInfo)
        {
            FollowingParser sectionParser = new FollowingParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void GenderParse(ExtractFileInfo fileInfo)
        {
            GenderParser sectionParser = new GenderParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void IncomingFollowRequestsParse(ExtractFileInfo fileInfo)
        {
            IncomingFollowRequestsParser sectionParser = new IncomingFollowRequestsParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void IndexParse(ExtractFileInfo fileInfo)
        {
            IndexParser sectionParser = new IndexParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void LikesParse(ExtractFileInfo fileInfo)
        {
            LikesParser sectionParser = new LikesParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void LinkedAccountsParse(ExtractFileInfo fileInfo)
        {
            LinkedAccountsParser sectionParser = new LinkedAccountsParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void LiveVideosParse(ExtractFileInfo fileInfo)
        {
            LiveVideosParser sectionParser = new LiveVideosParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void NameChangesParse(ExtractFileInfo fileInfo)
        {
            NameChangesParser sectionParser = new NameChangesParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void NcmecReportsParse(ExtractFileInfo fileInfo)
        {
            NcmecReportsParser sectionParser = new NcmecReportsParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void PhotosParse(ExtractFileInfo fileInfo)
        {
            PhotosParser sectionParser = new PhotosParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        
        // addded for unified messages
        internal void UnifiedMessagesParse(ExtractFileInfo fileInfo)
        {
            UnifiedMessagesParser sectionParser = new UnifiedMessagesParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }

        internal void PopularBlockParse(ExtractFileInfo fileInfo)
        {
            PopularBlockParser sectionParser = new PopularBlockParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void PrivacySettingsParse(ExtractFileInfo fileInfo)
        {
            PrivacySettingsParser sectionParser = new PrivacySettingsParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void ProfilePictureParse(ExtractFileInfo fileInfo)
        {
            ProfilePictureParser sectionParser = new ProfilePictureParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void VanityChangesParse(ExtractFileInfo fileInfo)
        {
            VanityChangesParser sectionParser = new VanityChangesParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void VideosParse(ExtractFileInfo fileInfo)
        {
            VideosParser sectionParser = new VideosParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }
        internal void WebsiteParse(ExtractFileInfo fileInfo)
        {
            WebsiteParser sectionParser = new WebsiteParser(_log, fileInfo, IsPreservation, DatabasePreservationNoPrefix, Version);
            WriteTables(sectionParser);
        }

        private void WriteTables(SectionParser parser)
        {
            bool tablesWritten = false;
            IEnumerable<DataTable> tables = parser.GenerateDataTables();
            foreach (DataTable table in tables)
            {
                if (table.Rows.Count > 0)
                {
                    DataAccess.CreateDatabase(DefaultDirectory, table, table.TableName, CaseNumber);
                    tablesWritten = true;
                }
            }

            if (!tablesWritten)
                throw new SectionEmptyException(parser.DisplaySectionName);

            if (parser.ContainsLocationData)
            {
                LocationData = parser.GenerateLocationInformation();
            }

            if (parser.ContainsPreservationQueries)
            {
                PreservationQueries = parser.PreservationQueries;
            }
        }
    }
}
