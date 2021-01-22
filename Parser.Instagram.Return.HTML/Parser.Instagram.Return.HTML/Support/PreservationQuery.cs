using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechShare.Parser.Instagram.Return.HTML.Support
{
    public class PreservationQuery
    {
        public string PreservationTableName { get; set; }
        public string QueryText { get; set; }
        public bool HasData
        {
            get
            {
                return !string.IsNullOrEmpty(PreservationTableName) &&
                    !string.IsNullOrEmpty(QueryText);
            }
        }
    }
}
