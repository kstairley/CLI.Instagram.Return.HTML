using System;
using System.Collections.Generic;
using System.Linq;

namespace TechShare.Utility.Tools.TableDefinitions
{
    public static class TableDefinitionExtensionMethods
    {
        public static IEnumerable<TableDefinition> GetChildren(this TableDefinition parent, IEnumerable<TableDefinition> tableDefs)
        {
            if (parent != null && tableDefs != null)
            {
                return tableDefs.Where(x => x.HasForeignKeys && x.ForeignKeys.Any(y => y.FKTableName.Equals(parent.Name, StringComparison.InvariantCultureIgnoreCase)));// !string.IsNullOrEmpty(x.ParentTable) && x.ParentTable.Equals(parent.Name, StringComparison.InvariantCultureIgnoreCase));
            }
            return null;
        }
        public static IEnumerable<TableDefinition> GetAllDescendants(this TableDefinition parent, IEnumerable<TableDefinition> tableDefs)
        {
            if (parent != null && tableDefs != null)
            {
                foreach (TableDefinition childDef in tableDefs.Where(x => x.HasForeignKeys && x.ForeignKeys.Any(y => y.FKTableName.Equals(parent.Name, StringComparison.InvariantCultureIgnoreCase))))
                {
                    yield return childDef;

                    foreach (TableDefinition grandChildDef in childDef.GetAllDescendants(tableDefs))
                        yield return grandChildDef;
                }
            }
        }
    }
}
