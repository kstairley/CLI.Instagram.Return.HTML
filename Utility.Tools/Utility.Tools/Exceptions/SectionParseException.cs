using System;

namespace TechShare.Utility.Tools.Exceptions
{
    public class SectionParseException : Exception
    {
        public SectionParseException()
        {
        }

        public SectionParseException(string message)
            : base(message)
        {
        }

        public SectionParseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
