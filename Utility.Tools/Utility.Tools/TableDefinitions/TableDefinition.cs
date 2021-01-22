using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TechShare.Utility.Tools.Conversations;

namespace TechShare.Utility.Tools.TableDefinitions
{
    public class TableDefinition
    {
        #region Constructors
        public TableDefinition(string name, string displayName, string groupName, object section,
            bool display, bool isView = false, bool isConversationData = false, int? heightPX = null, string shortName = null)
        {
            IsView = isView;
            Height = heightPX;
            Display = display;
            GroupName = !string.IsNullOrEmpty(groupName) ? groupName.Trim() : null;
            BaseName = !string.IsNullOrEmpty(name) ? name.Trim() : null;
            ShortName = !string.IsNullOrEmpty(shortName) ? shortName.Trim() : null;
            DisplayName = !string.IsNullOrEmpty(displayName) ? displayName.Trim() : null;
            IsConversationData = isConversationData;
            PreservationPrefix = null;
            Section = section;
            ValidateName(this);
        }
        #endregion

        #region Properties
        public int? Height { get; private set; }
        public bool Display { get; private set; }


        public bool IsPreservation { get; private set; }
        public bool IsView { get; private set; }
        public bool IsConversationData { get; private set; }

        public string GroupName { get; private set; }

        public string PreservationPrefix { get; private set; }
        private string BaseName { get; set; }
        public string Name { get { return PreservationPrefix + BaseName; } }
        public string ShortName { get; private set; }
        public string DisplayName { get; private set; }
        public TabDefinition PreservationTab { get; private set; }

        private List<ColumnDefinition> _columns = null;
        public IEnumerable<ColumnDefinition> Columns { get { return _columns; } }
        public bool HasColumns { get { return Columns != null && Columns.Any(); } }
        public ColumnDefinition PrimaryKey
        {
            get
            {
                ColumnDefinition retVal = null;
                if (HasColumns)
                {
                    IEnumerable<ColumnDefinition> cols = Columns.Where(x => x.KeyType.HasValue && x.KeyType.Value == KeyTypeEnum.PRIMARY);
                    if (cols != null && cols.Count() == 1)
                    {
                        retVal = cols.FirstOrDefault();
                    }
                }
                return retVal;
            }
        }
        public string PrimaryKeyColumnName { get { return PrimaryKey != null ? PrimaryKey.Name : null; } }

        private List<ForeignKey> _foreignKeys = null;
        public IEnumerable<ForeignKey> ForeignKeys { get { return _foreignKeys; } }
        public bool HasForeignKeys { get { return ForeignKeys != null && ForeignKeys.Any(); } }

        public object Section { get; set; }
        public bool HasData
        {
            get
            {
                return !string.IsNullOrEmpty(Name);
            }
        }
        #endregion

        #region Public Functions
        public bool HasForeignKeyTo(string tableName)
        {
            return !string.IsNullOrEmpty(tableName) && HasForeignKeys && ForeignKeys.Any(x => !string.IsNullOrEmpty(x.FKTableName) && x.FKTableName.Equals(tableName, StringComparison.InvariantCultureIgnoreCase));
        }
        public void AddColumn(string name, string displayName = null, Type dataType = null, bool display = true, KeyTypeEnum? keyType = null, FilterTypeEnum? filterType = null, ChatColumnTypeEnum? chatColType = null)
        {
            if (_columns == null)
                _columns = new List<ColumnDefinition>();
            int index = _columns.Any() ? _columns.Max(x => x.Index) + 1 : 0;
            _columns.Add(new ColumnDefinition(name, displayName, display, dataType ?? typeof(System.String), index, keyType, filterType, chatColType));
        }
        public void AddForeignKey(string columnName, string fkTableName, string fkColumnName)
        {
            if (!string.IsNullOrEmpty(columnName) && !string.IsNullOrEmpty(fkTableName) && !string.IsNullOrEmpty(fkColumnName))
            {
                if (_foreignKeys == null)
                    _foreignKeys = new List<ForeignKey>();

                _foreignKeys.Add(new ForeignKey()
                {
                    BaseTableName = Name,
                    ColumnName = columnName,
                    BaseFKTableName = fkTableName,
                    FKColumnName = fkColumnName
                });
            }
        }
        public string GenerateSelectStatement()
        {
            string columnList = HasColumns ? string.Join(",", Columns.Where(x => !string.IsNullOrEmpty(x.Name)).OrderBy(x => x.Index).Select(x => "[" + x.Name + "]")) : "*";
            return !string.IsNullOrEmpty(Name) ? "SELECT " + columnList + " FROM [" + Name + "]" : null;
        }
        public DataTable GenerateDataTable()
        {
            DataTable retVal = null;
            if (!string.IsNullOrEmpty(Name) && Columns.Any())
            {
                retVal = new DataTable(Name);
                foreach (ColumnDefinition columnDef in Columns.OrderBy(x => x.Index))
                {
                    retVal.Columns.Add(columnDef.Name, columnDef.DataType);
                }
            }
            return retVal;
        }
        public IEnumerable<string> GetFilterColumns(FilterTypeEnum filterType)
        {
            return Columns != null ? Columns.Where(x => !string.IsNullOrEmpty(x.Name) && x.FilterType == filterType).Select(x => x.Name) : null;
        }
        public string GenerateFilterStatement(IEnumerable<FilterItem> filters)
        {
            string retVal = null;
            if (filters != null)
            {
                List<string> filterStrings = new List<string>();
                foreach (FilterItem filter in filters)
                {
                    List<string> valueStrings = new List<string>();
                    if (filter.ColumnNames != null)
                    {
                        foreach (string columnName in filter.ColumnNames)
                        {
                            if (filter.Values != null)
                            {
                                foreach (FilterValue value in filter.Values)
                                    valueStrings.Add("[" + columnName + "]" + filter.GetComparisonText(value));
                            }
                        }
                        if (valueStrings.Any())
                            filterStrings.Add(string.Join((" " + Enum.GetName(typeof(FilterValueOperatorEnum), filter.ValueOperator) + " "), valueStrings.Select(x => "(" + x + ")")));
                    }
                }
                if (filterStrings.Any())
                    retVal = string.Join(" AND ", filterStrings.Select(x => "(" + x + ")"));
            }

            return retVal;
        }

        public TableDefinition Clone()
        {
            TableDefinition retVal = new TableDefinition(Name, DisplayName, GroupName, Section,
                Display, IsView, IsConversationData, Height, ShortName);

            if (HasColumns)
            {
                foreach (ColumnDefinition col in this.Columns)
                    retVal.AddColumn(col.Name, col.DisplayName, col.DataType, col.Display, col.KeyType, col.FilterType, col.ChatColumnType);
            }

            if (HasForeignKeys)
            {
                foreach (ForeignKey fk in ForeignKeys)
                    retVal.AddForeignKey(fk.ColumnName, fk.FKTableName, fk.FKColumnName);
            }
            return retVal;
        }
        public override bool Equals(object obj)
        {
            bool isEqual = false;
            TableDefinition toCompare = obj as TableDefinition;
            if (toCompare != null)
            {
                isEqual = BaseName == toCompare.BaseName &&
                    ColumnDefinition.IsEqual(Columns, toCompare.Columns) &&
                    GroupName == toCompare.GroupName &&
                    Display == toCompare.Display &&
                    DisplayName == toCompare.DisplayName &&
                    Height == toCompare.Height &&
                    IsConversationData == toCompare.IsConversationData &&
                    IsPreservation == toCompare.IsPreservation &&
                    IsView == toCompare.IsView &&
                    PreservationPrefix == toCompare.PreservationPrefix &&
                    ShortName == toCompare.ShortName &&
                    Name == toCompare.Name;
            }
            return isEqual;
        }
        #endregion

        #region Static Functions
        public static bool operator ==(TableDefinition td1, TableDefinition td2)
        {
            if ((object)td1 == null && (object)td2 == null)
                return true;
            else if ((object)td1 == null && (object)td2 != null)
                return false;
            else if ((object)td1 != null && (object)td2 == null)
                return false;
            else if ((object)td1 != null && (object)td2 != null)
                return td1.Equals(td2);
            return false;
        }
        public static bool operator !=(TableDefinition td1, TableDefinition td2)
        {
            if ((object)td1 == null && (object)td2 == null)
                return false;
            else if ((object)td1 == null && (object)td2 != null)
                return true;
            else if ((object)td1 != null && (object)td2 == null)
                return true;
            else if ((object)td1 != null && (object)td2 != null)
                return !td1.Equals(td2);
            return true;
        }
        //public static IEnumerable<TabDefinition> GetDistinctTabs(IEnumerable<TableDefinition> tableDefs)
        //{

        //    //IEnumerable<string> tabNames = tableDefs.Where(x => x.HasTabInfo && !x.IsPreservation).Select(x => x.TabInfo.Name);
        //    //IEnumerable<TabDefinition> tabs = tableDefs.Where(x => x.HasTabInfo && !x.IsPreservation).Select(x => x.TabInfo).GroupBy(x => x.Name).Select(x => x.First());
        //    //IEnumerable<TabDefinition> preservationTabs = tableDefs.Where(x => x.HasTabInfo && x.IsPreservation)
        //    //                                    .Select(x => x.TabInfo).Where(x => !tabNames.Contains(x.BaseName)).GroupBy(x => x.Name).Select(x => x.First());

        //    //List<TabDefinition> retVal = new List<TabDefinition>();
        //    //retVal.AddRange(tabs);
        //    //retVal.AddRange(preservationTabs);
        //    return tableDefs != null && tableDefs.Any() ? tableDefs.Where(x => x.HasTabInfo).Select(x => x.TabInfo).GroupBy(x => x.Name).Select(x => x.First()) : null;
        //}
        public static IEnumerable<TableDefinition> GetTableDefinitionsForTab(TabDefinition tabDef, IEnumerable<TableDefinition> tableDefs)
        {
            return tableDefs.Where(x => x.Section.ToString().Equals(tabDef.Section.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }
        public static TableDefinition FindDefinitionByTableName(string tableName, IEnumerable<TableDefinition> tableDefs)
        {
            return tableDefs != null && tableDefs.Any(x => !string.IsNullOrEmpty(x.Name)) ?
                tableDefs.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() :
                null;
        }
        public static string CONVERSATION_BY_DATE_VIEWNAME { get { return "vw_ConversationsByDate"; } }
        public static string CONVERSATION_BY_CONVERSATION_VIEWNAME { get { return "vw_ConversationsByConversation"; } }
        public static string CONVERSATION_MESSAGES_VIEWNAME { get { return "vw_ConversationMessages"; } }
        public static string CONVERSATION_PARTICIPANTS_VIEWNAME { get { return "vw_ConversationParticipants"; } }
        public static TableDefinition CopyTableDefinitionForPreservation(TableDefinition primaryDef, string preservationTablePrefix, string preservationTabName, string preservationTabDisplayName)
        {
            TableDefinition retVal = null;
            if (!string.IsNullOrEmpty(preservationTablePrefix))
            {

                if (primaryDef != null)
                {
                    retVal = primaryDef.Clone();
                    if (retVal != null)
                    {
                        retVal.PreservationPrefix = preservationTablePrefix;
                        ValidateName(retVal);
                        retVal.PreservationTab = new TabDefinition(null, null, preservationTabName, preservationTabDisplayName);
                        retVal.IsPreservation = true;

                        if (retVal.ForeignKeys != null && retVal.ForeignKeys.Any())
                        {
                            foreach (ForeignKey fk in retVal.ForeignKeys)
                                fk.PreservationPrefix = preservationTablePrefix;
                        }
                    }
                }
            }
            return retVal;
        }
        private static void ValidateName(TableDefinition def)
        {
            if (!string.IsNullOrEmpty(def.Name) && def.Name.Length > 31 &&
                (string.IsNullOrEmpty(def.ShortName) || (!string.IsNullOrEmpty(def.ShortName) && def.ShortName.Length > 31)))
                throw new ApplicationException("Name length > 31 characters: ShortName with length <= 31 characters must be provided");
        }
        #endregion
    }
}
