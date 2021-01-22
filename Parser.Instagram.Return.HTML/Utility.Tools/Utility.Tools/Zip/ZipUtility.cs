using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TechShare.Utility.Tools.Logs;
using TechShare.Utility.Tools.ProgressIndicator;

namespace TechShare.Utility.Tools.Zip
{
    public class ZipUtility
    {
        private CommandLineLogger _log = null;
        private int? _containerFolderLevels = null;

        public ZipUtility(CommandLineLogger log)
        {
            _log = log;
        }

        public string CaseName { get; set; }
        public string ParentZipFilePath { get; set; }
        public IEnumerable<ExtractFileInfo> ExtractZip(string defaultDirectory, string zipFilePath, string fileType, IProgress<ProgressValue> zipProgress = null)
        {
            string rootDataHolderPath = !string.IsNullOrEmpty(defaultDirectory) ? AppDomain.CurrentDomain.BaseDirectory + defaultDirectory :
                AppDomain.CurrentDomain.BaseDirectory + "Holder";

            List<ExtractFileInfo> files = new List<ExtractFileInfo>();
            string folderHolder = Path.GetFileNameWithoutExtension(zipFilePath);


            string extractTo = string.Empty;

            if (string.IsNullOrEmpty(ParentZipFilePath))
            {
                extractTo = rootDataHolderPath + Path.GetFileNameWithoutExtension(zipFilePath);
            }
            else
            {
                extractTo = rootDataHolderPath + Path.GetFileNameWithoutExtension(ParentZipFilePath) + "\\" + Path.GetFileNameWithoutExtension(zipFilePath);
            }

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    if (!Directory.Exists(extractTo))
                    {
                        Directory.CreateDirectory(extractTo);
                        _log.LogInfo("Created Directory: " + extractTo);
                    }

                    if (!_containerFolderLevels.HasValue)
                    {
                        if (archive.Entries.Any(x => x.FullName.ToUpper().Equals("INDEX.HTML")))
                            _containerFolderLevels = 0;
                        else
                        {
                            IEnumerable<ZipArchiveEntry> indexEntries = archive.Entries.Where(x => x.Name.ToUpper().Equals("INDEX.HTML"));
                            List<string[]> entries = new List<string[]>();
                            foreach (ZipArchiveEntry indexEntry in indexEntries)
                            {
                                entries.Add(indexEntry.FullName.Split('/'));
                            }
                            _containerFolderLevels = entries.Any() ? entries.Select(x => x.Length).Min() - 1 : (int?)null;
                        }

                    }
                    int entryCount = 0;
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(fileType))
                        {
                            string[] spFile = entry.FullName.Split('/').Skip(_containerFolderLevels.HasValue ? _containerFolderLevels.Value : 0).ToArray();
                            string originalFile = string.Empty;
                            string file = string.Empty;

                            originalFile = string.Join("\\", spFile);
                            if (spFile.Count() > 1 && spFile[spFile.Length - 1].ToUpper().Contains("INDEX"))
                                file = spFile[spFile.Length - 1].Replace("index", spFile[spFile.Length - 2]);
                            else
                                file = originalFile;

                            if (!file.StartsWith("._") && !file.EndsWith("\\"))
                            {
                                if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(extractTo, file))))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(extractTo, file)));
                                    _log.LogInfo("Created Directory: " + Path.GetDirectoryName(Path.Combine(extractTo, file)));
                                }

                                try
                                {
                                    if (!File.Exists(Path.Combine(extractTo, file)))
                                    {
                                        entry.ExtractToFile(Path.Combine(extractTo, file));
                                        _log.LogInfo("Created:" + Path.Combine(extractTo, file));
                                        files.Add(new ExtractFileInfo() { File_Path = Path.Combine(extractTo, file), OriginalFile_Path = Path.Combine(extractTo, originalFile), Root_Path = extractTo });
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        else
                        {
                            if (entry.FullName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                            {

                                string[] spFile = entry.FullName.Split('/').Skip(_containerFolderLevels.HasValue ? _containerFolderLevels.Value : 0).ToArray();
                                string originalFile = string.Empty;
                                string file = string.Empty;

                                originalFile = string.Join("\\", spFile);
                                if (spFile.Count() > 1 && spFile[spFile.Length - 1].ToUpper().Contains("INDEX"))
                                    file = spFile[spFile.Length - 1].Replace("index", spFile[spFile.Length - 2]);
                                else
                                    file = originalFile;

                                if (!file.StartsWith("._") && !file.EndsWith("\\"))
                                {
                                    if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(extractTo, file))))
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(extractTo, file)));
                                        _log.LogInfo("Created Directory: " + Path.GetDirectoryName(Path.Combine(extractTo, file)));
                                    }
                                    try
                                    {
                                        if (!File.Exists(Path.Combine(extractTo, file)))
                                        {
                                            entry.ExtractToFile(Path.Combine(extractTo, file));
                                            _log.LogInfo("Created:" + Path.Combine(extractTo, file));
                                            files.Add(new ExtractFileInfo() { File_Path = Path.Combine(extractTo, file), OriginalFile_Path = Path.Combine(extractTo, originalFile), Root_Path = extractTo });
                                        }
                                    }
                                    catch (Exception ep)
                                    {

                                    }
                                }
                            }
                        }
                        entryCount++;
                        if (zipProgress != null)
                        {
                            int percent = (int)Math.Round(((double)entryCount / (double)archive.Entries.Count) * 100, 0);
                            zipProgress.Report(new ProgressValue { Message = "Extracting " + Path.GetFileName(zipFilePath) + "... ", PercentComplete = percent });
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return files;
        }
        public IEnumerable<ExtractFileInfo> GenerateParsedHtml(IEnumerable<ExtractFileInfo> physicalHtmlFiles)
        {
            List<ExtractFileInfo> files = new List<ExtractFileInfo>();

            foreach (ExtractFileInfo htmlFile in physicalHtmlFiles)
            {
                string extractPath = Path.GetFileNameWithoutExtension(htmlFile.File_Path).Equals("INDEX", StringComparison.InvariantCultureIgnoreCase) ?
                    Path.GetDirectoryName(htmlFile.File_Path) : Path.GetDirectoryName(htmlFile.File_Path) + @"\" + UppercaseFirst(Path.GetFileNameWithoutExtension(htmlFile.File_Path));
                string fileContents = File.ReadAllText(htmlFile.File_Path);
                if (fileContents.Contains("additional-property-link"))
                {
                    string[] divs = fileContents.Replace("<div", "~%~%<div").Split(new string[] { "~%~%" }, StringSplitOptions.None);
                    List<string> sections = new List<string>();
                    foreach (string div in divs)
                    {
                        if (div.Contains("<div") && div.Contains("id=\""))
                        {
                            string id = div.Substring(div.IndexOf("id=\"") + 4);
                            id = id.Substring(0, id.IndexOf("\""));
                            sections.Add(id);
                        }
                    }

                    string toParse = fileContents;
                    foreach (string section in sections)
                    {
                        int startOfSectionDiv = toParse.LastIndexOf("<div", toParse.IndexOf("\"" + section + "\""));
                        int endOfSectionDiv = toParse.IndexOf("div>", startOfSectionDiv);
                        string sectionDiv = toParse.Substring(startOfSectionDiv, (endOfSectionDiv - startOfSectionDiv + 4));

                        if (!string.IsNullOrEmpty(sectionDiv))
                        {
                            string sectionHtml = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head></head><body>" + sectionDiv + "</body></html>";

                            string fileName = (section.Contains("property-") ? section.Replace("property-", "") : section.Replace("home", "index")) + ".html";
                            if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(extractPath, fileName))))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(extractPath, fileName)));
                                _log.LogInfo("Created Directory: " + Path.GetDirectoryName(Path.Combine(extractPath, fileName)));
                            }
                            File.WriteAllText(Path.Combine(extractPath, fileName), sectionHtml);
                            _log.LogInfo("Created temporary file:" + Path.Combine(extractPath, fileName));
                            files.Add(new ExtractFileInfo() { File_Path = Path.Combine(extractPath, fileName), ParentFile_Path = htmlFile.File_Path, OriginalFile_Path = htmlFile.File_Path, Root_Path = extractPath });
                        }
                        toParse.Replace(sectionDiv, "");
                    }
                }

            }
            return files;
        }
        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
