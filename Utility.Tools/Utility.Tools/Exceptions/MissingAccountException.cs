using System;

namespace TechShare.Utility.Tools.Exceptions
{
    public class MissingAccountException : Exception
    {
        public MissingAccountException()
        {
        }

        public MissingAccountException(string message)
            : base(message)
        {
        }

        public MissingAccountException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
