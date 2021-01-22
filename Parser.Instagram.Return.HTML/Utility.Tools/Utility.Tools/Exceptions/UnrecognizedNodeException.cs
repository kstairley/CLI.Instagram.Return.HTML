using System;
using System.Collections.Generic;
using System.Linq;

namespace TechShare.Utility.Tools.Exceptions
{
    public class UnrecognizedNodeException : Exception
    {
        private static readonly string MESSAGE = "Unrecognized node encountered: {0} - {1}.  Please contact NDCAC with category name and test data to improve parsing functionality";
        public UnrecognizedNodeException()
        {
        }

        public UnrecognizedNodeException(string section, string node)
            : base(string.Format(MESSAGE, section, node))
        {
        }

        public UnrecognizedNodeException(string section, string node, Exception inner)
           : base(string.Format(MESSAGE, section, node), inner)
        {
        }

        public UnrecognizedNodeException(List<string> dataSectionChain, Stack<string> keyChain)
            : base(string.Format(MESSAGE, dataSectionChain != null && dataSectionChain.Any() ? string.Join("\\", dataSectionChain) : "",
                keyChain != null && keyChain.Any() ? string.Join(":", keyChain) : ""))
        {
        }

        public UnrecognizedNodeException(List<string> dataSectionChain, Stack<string> keyChain, Exception inner)
            : base(string.Format(MESSAGE, dataSectionChain != null && dataSectionChain.Any() ? string.Join("\\", dataSectionChain) : "",
                keyChain != null && keyChain.Any() ? string.Join(":", keyChain) : ""), inner)
        {
        }
    }
}
