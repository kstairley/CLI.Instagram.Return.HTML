using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechShare.Utility.Tools.Extensions;

namespace TechShare.Utility.Tools.HtmlParse
{
    public class ParseDocument
    {
        #region Variables and Properties
        private List<ParseDataItem> _items = null;
        public IEnumerable<ParseDataItem> Items { get { return _items != null && _items.Any(x => x.HasData) ? _items.Where(x => x.HasData) : null; } }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public bool HasData { get { return Items != null && Items.Any(); } }
        #endregion

        #region Functions
        public void Load(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            FilePath = Path.GetFullPath(filePath);

            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        bool insideBody = false;
                        while (sr.Peek() >= 0)
                        {
                            string tag = string.Empty;
                            if (!insideBody)
                            {
                                do
                                {
                                    tag = StreamReaderExtensions.ReadUntil(sr, '>');
                                    if (tag.ToLower().Contains("<body"))
                                        insideBody = true;
                                } while (!insideBody);
                            }

                            ParseDataItem dataItem = new ParseDataItem(sr, this, null);
                            if (dataItem.HasData)
                                AddItem(dataItem);
                        }
                        sr.Dispose();
                    }
                    bs.Dispose();
                }
                fs.Dispose();
            }
        }

        private void AddItem(ParseDataItem item)
        {
            if (_items == null)
                _items = new List<ParseDataItem>();
            _items.Add(item);
        }
        #endregion
    }
}
