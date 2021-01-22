namespace TechShare.Utility.Tools.TableDefinitions
{
    public class TabDefinition
    {
        public TabDefinition(object section, int? index, string name, string displayName)
        {
            Index = index;
            Name = name;
            DisplayName = displayName;
            Section = section;
        }
        public int? Index { get; set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public object Section { get; set; }

        public bool HasData
        {
            get
            {
                return !string.IsNullOrEmpty(DisplayName) ||
                    Index.HasValue ||
                    !string.IsNullOrEmpty(Name);
            }
        }
    }
}
