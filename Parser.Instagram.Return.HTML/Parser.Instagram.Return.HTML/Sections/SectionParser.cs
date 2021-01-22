using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TechShare.Parser.Instagram.Return.HTML.Support;
using TechShare.Utility.Tools.HtmlParse;
using TechShare.Utility.Tools.Logs;
using TechShare.Utility.Tools.Zip;

namespace TechShare.Parser.Instagram.Return.HTML.Sections
{
    public abstract class SectionParser
    {
        protected CommandLineLogger Logger { get; set; }
        protected ExtractFileInfo FileInfo { get; set; }
        protected string SourceFile { get; set; }
        protected bool IsPreservation { get; set; }
        protected string PreservationPrefix { get; set; }
        protected ParserVersionEnum ParserVersion { get; set; }
        protected ParseDocument HtmlDoc { get; set; }
        protected string SectionName { get; set; }
        protected string MainTableName { get; set; }
        public string DisplaySectionName { get; internal set; }
        public bool ContainsLocationData { get; internal set; }
        public bool ContainsPreservationQueries { get { return PreservationQueries != null && PreservationQueries.Any(x => x.HasData); } }
        protected abstract bool HasData { get; }
        public IEnumerable<PreservationQuery> PreservationQueries { get; internal set; }

        public SectionParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
        {
            ContainsLocationData = false;
            Logger = logger;
            FileInfo = fileInfo;
            SourceFile = FileInfo.OriginalFile_Path;
            SectionName = Path.GetFileNameWithoutExtension(FileInfo.File_Name);

            List<string> sectionStrings = new List<string>();

            if (SectionName.Contains("_"))
                sectionStrings = SectionName.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            else if (SectionName.Contains("_"))
                sectionStrings = SectionName.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!sectionStrings.Any())
                sectionStrings.Add(SectionName);

            List<string> sectionStringsUpper = new List<string>();
            string temp = string.Empty;
            foreach (string s in sectionStrings)
                sectionStringsUpper.Add(s.First().ToString().ToUpper() + s.Substring(1));

            IsPreservation = isPreservation;
            PreservationPrefix = preservationPrefix;
            ParserVersion = version;

            MainTableName = string.Join("", sectionStringsUpper);
            if (isPreservation)
                MainTableName = PreservationPrefix + "_" + MainTableName;
            DisplaySectionName = string.Join(" ", sectionStringsUpper);

            LoadHtml();
            ProcessHTML();
        }

        protected abstract void ProcessHTML();
        public abstract IEnumerable<DataTable> GenerateDataTables();
        public abstract IEnumerable<LocationDataPoint> GenerateLocationInformation();
        protected void LoadHtml()
        {
            HtmlDoc = new ParseDocument();
            HtmlDoc.Load(FileInfo.File_Path);
        }
    }
}
