using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TechShare.Parser.Instagram.Return.HTML.Support
{
    public class InstagramObject
    {
        public InstagramObject(string initValue)
        {
            if (!string.IsNullOrEmpty(initValue))
            {
                string decodedValue = initValue;

                int displayNameIndex = decodedValue.LastIndexOf("[");
                if (displayNameIndex != -1)
                {
                    string tmpUsername = decodedValue.Substring(displayNameIndex);
                    DisplayName = tmpUsername.Replace("[", "").Replace("]", "");
                    if (!string.IsNullOrEmpty(tmpUsername))
                        decodedValue = decodedValue.Replace(tmpUsername, "");
                }

                int idIndex = decodedValue.LastIndexOf("(");
                if (idIndex != -1)
                {
                    Name = decodedValue.Substring(0, idIndex).Trim();
                    if (!string.IsNullOrEmpty(Name))
                        decodedValue = decodedValue.Replace(Name, "");

                    Id = decodedValue.Trim().Replace("(", "").Replace(")", "");
                }
                else
                {
                    Name = decodedValue.Trim();
                }
            }
        }
        public string Name { get; private set; }
        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public bool HasData
        {
            get
            {
                return (!string.IsNullOrEmpty(Name) ||
                    !string.IsNullOrEmpty(Id));
            }
        }
        public override string ToString()
        {
            string retVal = string.Empty;

            retVal += this.Name.Trim();

            if (!string.IsNullOrEmpty(Id))
            {
                if (!string.IsNullOrEmpty(retVal))
                    retVal += " ";
                retVal += "(" + Id + ")";
            }

            if (!string.IsNullOrEmpty(DisplayName))
            {
                if (!string.IsNullOrEmpty(retVal))
                    retVal += " ";
                retVal += "[" + DisplayName + "]";
            }

            return retVal;
        }
    }
}
