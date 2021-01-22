using System;

namespace TechShare.Utility.Tools.Exceptions
{
    public class MissingTestDataException : Exception
    {
        public MissingTestDataException()
        {
        }

        public MissingTestDataException(string message)
            : base(message)
        {
        }

        public MissingTestDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
