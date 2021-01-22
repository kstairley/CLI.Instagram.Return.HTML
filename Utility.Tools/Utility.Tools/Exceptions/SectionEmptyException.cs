using System;

namespace TechShare.Utility.Tools.Exceptions
{
    public class SectionEmptyException : Exception
    {
        public SectionEmptyException()
        {
        }

        public SectionEmptyException(string message)
            : base(message)
        {
        }

        public SectionEmptyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
