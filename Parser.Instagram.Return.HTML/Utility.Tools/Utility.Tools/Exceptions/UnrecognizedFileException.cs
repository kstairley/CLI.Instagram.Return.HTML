using System;

namespace TechShare.Utility.Tools.Exceptions
{
    public class UnrecognizedFileException : Exception
    {
        //private static readonly string MESSAGE = "Unrecognized node encountered: {0} - {1}.  Please contact NDCAC with category name and test data to improve parsing functionality";
        private static readonly string MESSAGE = "Unknown File in section: {0} - Filename: {1}.  Please contact NDCAC with category name and test data to improve parsing functionality";
        private static readonly string MESSAGE_FILEONLY = "Unknown File encountered - Filename: {0}.  Please contact NDCAC with the file containing test data to improve parsing functionality";
        public UnrecognizedFileException()
        {
        }

        public UnrecognizedFileException(string fileName)
            : base(string.Format(MESSAGE_FILEONLY, fileName))
        {
        }

        public UnrecognizedFileException(string fileName, Exception inner)
           : base(string.Format(MESSAGE_FILEONLY, fileName), inner)
        {
        }


        public UnrecognizedFileException(string dataSection, string fileName)
            : base(string.Format(MESSAGE, dataSection, fileName))
        {
        }

        public UnrecognizedFileException(string dataSection, string fileName, Exception inner)
            : base(string.Format(MESSAGE, dataSection, fileName), inner)
        {
        }
    }
}
