using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TechShare.Parser.Instagram.Return.HTML.Support;
using TechShare.Utility.Tools.Exceptions;
using TechShare.Utility.Tools.Logs;
using TechShare.Utility.Tools.Zip;

namespace TechShare.Parser.Instagram.Return.HTML.Sections
{
    public class DirectDebitParser : SectionParser
    {
        public DirectDebitParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
            : base(logger, fileInfo, isPreservation, preservationPrefix, version)
        {
            throw new MissingTestDataException(DisplaySectionName);
        }

        #region Properties
        protected override bool HasData
        {
            get
            {
                return Items != null && Items.Any(x => x.HasData);
            }
        }
        private IEnumerable<InstagramObject> Items { get; set; }
        #endregion

        #region Functions
        public override IEnumerable<DataTable> GenerateDataTables()
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<LocationDataPoint> GenerateLocationInformation()
        {
            return null;
        }
        protected override void ProcessHTML()
        {
        }
        #endregion
    }
}
