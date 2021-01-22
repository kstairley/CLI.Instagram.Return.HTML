using System;
using System.Collections.Generic;
using System.Linq;
using TechShare.Utility.Tools.Conversations;

namespace TechShare.Utility.Tools.TableDefinitions
{
    public class ColumnDefinition
    {
        public ColumnDefinition(string name, string displayName, bool display, Type dataType, int index, KeyTypeEnum? keyType, FilterTypeEnum? filterType, ChatColumnTypeEnum? chatColumnType)//, bool isFilterableByKeyword = false, bool isFilterableByDate = false)
        {
            Name = name;
            DisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            Display = display;
            DataType = dataType;
            Index = index;
            KeyType = keyType;
            FilterType = filterType;
            ChatColumnType = chatColumnType;
        }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public bool Display { get; private set; }
        public Type DataType { get; private set; }
        public int Index { get; private set; }
        public KeyTypeEnum? KeyType { get; private set; }
        public FilterTypeEnum? FilterType { get; private set; }
        public ChatColumnTypeEnum? ChatColumnType { get; private set; }
        public override bool Equals(object obj)
        {
            ColumnDefinition toCompare = obj as ColumnDefinition;
            if (toCompare == null)
                return false;

            return ChatColumnType == toCompare.ChatColumnType &&
                DataType == toCompare.DataType &&
                Display == toCompare.Display &&
                DisplayName == toCompare.DisplayName &&
                FilterType == toCompare.FilterType &&
                Index == toCompare.Index &&
                Name == toCompare.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ColumnDefinition col1, ColumnDefinition col2)
        {
            if ((object)col1 == null && (object)col2 == null)
                return true;
            else if ((object)col1 != null && (object)col2 == null)
                return false;
            else if ((object)col1 == null && (object)col2 != null)
                return false;
            else if ((object)col1 != null && (object)col2 != null)
                return col1.Equals(col2);
            return false;
        }
        public static bool operator !=(ColumnDefinition col1, ColumnDefinition col2)
        {
            if ((object)col1 == null && (object)col2 == null)
                return false;
            else if ((object)col1 != null && (object)col2 == null)
                return true;
            else if ((object)col1 == null && (object)col2 != null)
                return true;
            else if ((object)col1 != null && (object)col2 != null)
                return !col1.Equals(col2);
            return false;
        }

        public static bool IsEqual(IEnumerable<ColumnDefinition> colList1, IEnumerable<ColumnDefinition> colList2)
        {
            if (colList1 == null && colList2 == null)
                return true;
            else if (colList1 != null && colList2 == null)
                return false;
            else if (colList1 == null && colList2 != null)
                return false;
            else if (colList1 != null && colList2 != null)
            {
                if (colList1.Count() != colList2.Count())
                    return false;
                else
                {
                    foreach (ColumnDefinition colDef in colList1)
                    {
                        if (colList2.Where(x => x == colDef).Count() != 1)
                            return false;
                    }

                    foreach (ColumnDefinition colDef in colList2)
                    {
                        if (colList1.Where(x => x == colDef).Count() != 1)
                            return false;
                    }
                }
            }
            return true;
        }
    }
}
