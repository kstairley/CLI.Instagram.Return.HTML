using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TechShare.Utility.Tools.Extensions
{
    public static class StreamReaderExtensions
    {
        public static string ReadUntil(StreamReader reader, char delimiter)
        {
            string retVal = string.Empty;

            List<char> charList = new List<char>();
            while (reader.Peek() > 0)
            {
                char x = (char)reader.Read();
                if (!char.IsControl(x))
                    charList.Add(x);
                if (x.Equals(delimiter))
                    break;
            }

            if (charList.Any())
                retVal = string.Concat(charList);
            return retVal;
        }
    }
}
