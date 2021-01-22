using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TechShare.Parser.Instagram.Return.HTML.Support;
using TechShare.Utility.Tools.Exceptions;
using TechShare.Utility.Tools.Extensions;
using TechShare.Utility.Tools.Logs;
using TechShare.Utility.Tools.Zip;

namespace InstagramHTMLParser
{
    public class InstagramHTMLParse
    {
        public InstagramHTMLParse(string defaultDirectory)
        {
            if (string.IsNullOrEmpty(defaultDirectory))
                throw new ApplicationException("Unable to parse data: Default Directory is required.");
            _defaultDirectory = defaultDirectory;
        }

        #region variables and properities
        private string _defaultDirectory = string.Empty;
        private List<ExtractFileInfo> _htmlToParse = new List<ExtractFileInfo>();
        private List<ExtractFileInfo> _tempFilesToRemove = new List<ExtractFileInfo>();
        private List<ExtractFileInfo> _allFiles = new List<ExtractFileInfo>();
        private List<ExtractFileInfo> _filesToPreserve = new List<ExtractFileInfo>();
        private List<LocationDataPoint> _locationData = new List<LocationDataPoint>();
        private Dictionary<string, List<PreservationQuery>> _preservationList = null;
        #endregion
        private void SaveFiles(IEnumerable<ExtractFileInfo> files)
        {
            if (files != null && files.Any())
                _allFiles.AddRange(files);
        }

        public string ParseInstagramHTMLExtract(string extractZipFileNameAndPath)
        {
            Stopwatch totalProcessingTime = new Stopwatch();
            totalProcessingTime.Start();

            Stopwatch stopWatch = null;
            bool hasPreservation = false;
            ParserVersionEnum version = ParserVersionEnum.One;
            string holderFolder = Path.GetFileNameWithoutExtension(extractZipFileNameAndPath);
            CommandLineLogger logger = new CommandLineLogger(holderFolder, _defaultDirectory);

            //pass case name to extract zip
            ZipUtility zp = new ZipUtility(logger)
            {
                CaseName = holderFolder
            };

            logger.LogInfo("Begin extracting files.");
            stopWatch = new Stopwatch();
            stopWatch.Start();
            UnzipFiles(logger, holderFolder, _defaultDirectory, null, extractZipFileNameAndPath, ref version);
            stopWatch.Stop();
            logger.LogInfo("Extract files complete (Time: " + stopWatch.Elapsed.GetFormattedElapsedTime() + ")... ");

            string holderPath = AppDomain.CurrentDomain.BaseDirectory + _defaultDirectory;
            string extractPath = holderPath + Path.GetFileNameWithoutExtension(holderFolder);

            ParseHTML(logger, version, holderFolder, ref hasPreservation);


            if (_allFiles.Count > 0)
                DataAccess.AddSourceFiles(_defaultDirectory, _allFiles, holderFolder);

            // TODO: Need to revisit this in the future to determine what analytics are requred or wanted by the user base.
            ////compare preservation to current tables if exists
            //if (hasPreservation)
            //{
            //    _log.LogInfo("Begin comparing preservation files.");
            //    DifPreservationTables(holderFolder);
            //}

            if (_tempFilesToRemove.Any())
            {
                logger.LogInfo("Begin removing temporary files.");
                stopWatch = new Stopwatch();
                stopWatch.Start();
                List<string> tempPathsToRemove = new List<string>();
                foreach (ExtractFileInfo tempFile in _tempFilesToRemove)
                {
                    logger.LogInfo("Removing file: " + tempFile.File_Path.Replace(extractPath + "\\", ""));
                    File.Delete(tempFile.File_Path);
                    if (!tempPathsToRemove.Contains(Path.GetDirectoryName(tempFile.File_Path)))
                        tempPathsToRemove.Add(Path.GetDirectoryName(tempFile.File_Path));
                }
                if (tempPathsToRemove.Any())
                {
                    foreach (string pathToRemove in tempPathsToRemove)
                    {
                        if (Directory.GetFiles(pathToRemove).Length == 0)
                            Directory.Delete(pathToRemove);
                    }
                }
                stopWatch.Stop();
                logger.LogInfo("Removing temporary files complete (Time: " + stopWatch.Elapsed.GetFormattedElapsedTime() + ")... ");
            }
            if (_filesToPreserve.Any())
            {
                foreach (ExtractFileInfo file in _filesToPreserve)
                {
                    string preservedFileName = Path.GetDirectoryName(file.File_Path) + "\\save-" + file.File_Name;
                    if (File.Exists(preservedFileName))
                    {
                        if (!File.Exists(file.File_Path))
                            File.Copy(preservedFileName, file.File_Path);
                        File.Delete(Path.GetDirectoryName(file.File_Path) + "\\save-" + file.File_Name);
                    }
                }
            }

            totalProcessingTime.Stop();
            logger.LogInfoAlert("Processing Complete.  Total Processing time: " + totalProcessingTime.Elapsed.GetFormattedElapsedTime());
            return "";
        }

        private void UnzipFiles(CommandLineLogger logger, string holderFolder, string defaultDirectory, string parentZipFile, string toExtract, ref ParserVersionEnum version)
        {

            ZipUtility zp = new ZipUtility(logger)
            {
                CaseName = holderFolder,
                ParentZipFilePath = parentZipFile
            };

            IEnumerable<ExtractFileInfo> htmlFiles = zp.ExtractZip(defaultDirectory, toExtract, ".html");
            if (htmlFiles.Any())
            {
                foreach (ExtractFileInfo file in htmlFiles)
                {
                    File.Copy(file.File_Path, Path.GetDirectoryName(file.File_Path) + "\\save-" + file.File_Name);
                    _filesToPreserve.Add(file);
                }

                _htmlToParse.AddRange(htmlFiles.Where(x => !x.File_Name.ToUpper().Trim().StartsWith("INDEX.") && !x.File_Name.ToUpper().Trim().StartsWith("PRESERVATION")));
                IEnumerable<ExtractFileInfo> generatedHtmlFiles = zp.GenerateParsedHtml(htmlFiles);
                if (generatedHtmlFiles.Any())
                {
                    version = ParserVersionEnum.Two;
                    foreach (ExtractFileInfo generatedHtmlFile in generatedHtmlFiles)
                    {
                        if (!_htmlToParse.Any(x => x.File_Path.Equals(generatedHtmlFile.File_Path)) &&
                            !_htmlToParse.Any(x => x.File_Path.Equals(generatedHtmlFile.File_Path + @"\" + Path.GetFileNameWithoutExtension(generatedHtmlFile.File_Path))))
                            _htmlToParse.Add(generatedHtmlFile);
                    }
                    _tempFilesToRemove.AddRange(generatedHtmlFiles);
                }
                else
                {
                    _htmlToParse.AddRange(htmlFiles.Where(x => x.File_Name.ToUpper().Trim().StartsWith("INDEX.")));
                }
                SaveFiles(htmlFiles);
            }

            IEnumerable<ExtractFileInfo> zipFiles = zp.ExtractZip(defaultDirectory, toExtract, ".zip");
            if (zipFiles.Count() > 0)
            {
                SaveFiles(zipFiles);
                _tempFilesToRemove.AddRange(zipFiles);
            }
            IEnumerable<ExtractFileInfo> otherFiles = zp.ExtractZip(defaultDirectory, toExtract, "");
            if (otherFiles.Count() > 0)
                SaveFiles(otherFiles);

            foreach (ExtractFileInfo z in zipFiles)
            {
                UnzipFiles(logger, holderFolder, defaultDirectory, toExtract, z.File_Path, ref version);
            }
        }

        private void ParseHTML(CommandLineLogger logger, ParserVersionEnum version, string caseName, ref bool hasPreservation)
        {
            Stopwatch stopWatch = new Stopwatch();
            logger.LogInfo("Begin parsing files.");
            stopWatch.Start();
            foreach (ExtractFileInfo htmlx in _htmlToParse)
            {
                ParserManager parser = new ParserManager(logger)
                {
                    CaseNumber = caseName,
                    HtmlToLoad = htmlx.File_Path,
                    HtmlToRecord = htmlx.IsTemporary ? htmlx.ParentFile_Path : htmlx.File_Path,
                    IsPreservation = false,
                    DatabasePreservationNoPrefix = string.Empty,
                    Version = version,
                    DefaultDirectory = _defaultDirectory
                };

                logger.LogInfo("Processing " + htmlx.File_Path);
                try
                {
                    if (htmlx.File_Path.ToLower().Contains("preservation"))
                    {
                        hasPreservation = true;
                        /*Preservation-1, Preservation-2, Preservation-3  */
                        parser.IsPreservation = true;
                        DirectoryInfo di = new DirectoryInfo(htmlx.File_Path);
                        string p = di.Parent.Name;   //goes up to parent directory, preservation
                        if (!p.ToLower().Contains("preservation"))
                            p = di.Parent.Parent.Name; //goes up to parent directory, preservation\folderX\index.html
                        parser.DatabasePreservationNoPrefix = p.Replace("-", "_");      //sqllite doesn't like sql queries ref tables with a '-', change to '_'
                    }

                    if (!htmlx.File_Name.ToUpper().Contains("PRESERVATION"))
                    {
                        switch (htmlx.File_Name.ToUpper().Trim())
                        {
                            case "ABOUT_ME.HTML":
                                parser.AboutMeParse(htmlx);
                                break;
                            case "ACCOUNT_STATUS_HISTORY.HTML":
                                parser.AccountStatusHistoryParse(htmlx);
                                break;
                            case "COMMENTS.HTML":
                                parser.CommentsParse(htmlx);
                                break;
                            case "DEVICES.HTML":
                                parser.DevicesParse(htmlx);
                                break;
                            case "DIRECT_SHARES.HTML":
                                parser.DirectSharesParse(htmlx);
                                break;
                            case "DIRECT_STORIES.HTML":
                                parser.DirectStoriesParse(htmlx);
                                break;
                            case "FOLLOWERS.HTML":
                                parser.FollowersParse(htmlx);
                                break;
                            case "FOLLOWING.HTML":
                                parser.FollowingParse(htmlx);
                                break;
                            case "GENDER.HTML":
                                parser.GenderParse(htmlx);
                                break;
                            case "INCOMING_FOLLOW_REQUESTS.HTML":
                                parser.IncomingFollowRequestsParse(htmlx);
                                break;
                            case "INDEX.HTML":
                                parser.IndexParse(htmlx);
                                break;
                            case "LIKES.HTML":
                                parser.LikesParse(htmlx);
                                break;
                            case "LINKED_ACCOUNTS.HTML":
                                parser.LinkedAccountsParse(htmlx);
                                break;
                            case "LIVE_VIDEOS.HTML":
                                parser.LiveVideosParse(htmlx);
                                break;
                            case "UNIFIED_MESSAGES.HTML":
                                parser.UnifiedMessagesParse(htmlx);
                                break;
                            case "NAME_CHANGES.HTML":
                                parser.NameChangesParse(htmlx);
                                break;
                            case "NCMEC_REPORTS.HTML":
                                parser.NcmecReportsParse(htmlx);
                                break;
                            case "PHOTOS.HTML":
                                parser.PhotosParse(htmlx);
                                break;
                            case "POPULAR_BLOCK.HTML":
                                parser.PopularBlockParse(htmlx);
                                break;
                            case "PRIVACY_SETTINGS.HTML":
                                parser.PrivacySettingsParse(htmlx);
                                break;
                            case "PROFILE_PICTURE.HTML":
                                parser.ProfilePictureParse(htmlx);
                                break;
                            case "VANITY_CHANGES.HTML":
                                parser.VanityChangesParse(htmlx);
                                break;
                            case "VIDEOS.HTML":
                                parser.VideosParse(htmlx);
                                break;
                            case "WEBSITE.HTML":
                                parser.WebsiteParse(htmlx);
                                break;
                            default:
                                logger.LogWarning("Unknown Section - \"Unknown section:" + htmlx.File_Name + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                throw new ApplicationException(htmlx.File_Name);
#endif
                                break;
                        }
                    }
                    if (parser.LocationData != null && parser.LocationData.Any())
                        _locationData.AddRange(parser.LocationData);
                    AddSectionToPreservationList(htmlx.File_Name, parser.PreservationQueries);
                }
                catch (SectionEmptyException ex)
                {
                    logger.LogWarning("Parsing " + ex.Message + " section complete - section contains no data: Excluding from database.");
                }
                catch (MissingTestDataException ex)
                {
                    logger.LogWarning("Parsing " + ex.Message + " section skipped - parser not implemented: No test data available.");
                }
                catch (NotImplementedException ex)
                {
                    logger.LogError("Parsing " + ex.Message + " section failed: parser not implemented.", ex);
                }
            }
            WriteLocationData(caseName);
            stopWatch.Stop();
            logger.LogInfo("Parsing files complete (Time: " + stopWatch.Elapsed.GetFormattedElapsedTime() + ")... ");
        }

        private void WriteLocationData(string caseName)
        {
            DataTable table = LocationDataPoint.GenerateLocationTable(_locationData);
            if (table.Rows.Count > 0)
                DataAccess.CreateDatabase(_defaultDirectory, table, table.TableName, caseName);
        }

        private void AddSectionToPreservationList(string fileName, IEnumerable<PreservationQuery> preservationQueries)
        {
            if (!string.IsNullOrEmpty(fileName) && preservationQueries != null && preservationQueries.Any(x => x.HasData))
            {
                if (_preservationList == null)
                    _preservationList = new Dictionary<string, List<PreservationQuery>>();

                if (!string.IsNullOrEmpty(fileName))
                {
                    if (!_preservationList.ContainsKey(fileName))
                        _preservationList.Add(fileName, new List<PreservationQuery>());
                    _preservationList[fileName].AddRange(preservationQueries);
                }
            }
        }
    }
}
