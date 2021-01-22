using System.IO;

namespace TechShare.Utility.Tools.Zip
{
    public class ExtractFileInfo
    {
        public string File_Path { get; set; }
        public string File_Name { get { return Path.GetFileName(File_Path); } }
        public string File_Extension { get { return Path.GetExtension(File_Path); } }

        private string _category = string.Empty;
        public string Category
        {
            get
            {
                if (string.IsNullOrEmpty(_category))
                {
                    string calcPath = File_Path.Replace(Root_Path, "");
                    if (calcPath.StartsWith(@"\"))
                        calcPath = calcPath.Substring(1);

                    if (calcPath.IndexOf(@"\") > 0)
                        _category = calcPath.Substring(0, calcPath.IndexOf(@"\"));
                }
                return _category;
            }
        }
        public string OriginalFile_Path { get; set; }
        public string OriginalFile_Name { get { return Path.GetFileName(OriginalFile_Path); } }

        public string ParentFile_Path { get; set; }
        public string ParentFile_Name { get { return Path.GetFileName(ParentFile_Path); } }
        internal string Root_Path { get; set; }

        public bool IsTemporary
        {
            get
            {
                return !string.IsNullOrEmpty(ParentFile_Path);
            }
        }
    }
}
