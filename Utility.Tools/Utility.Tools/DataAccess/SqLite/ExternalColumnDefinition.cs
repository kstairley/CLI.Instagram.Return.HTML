namespace TechShare.Utility.Tools.DataAccess.SqLite
{
    public class ExternalColumnDefinition
    {
        public ExternalColumnDefinition(int sequence, string name, string type)
        {
            Sequence = sequence;
            ColumnName = name;
            DisplayColumnName = name;
            ColumnType = type;
        }
        public int Sequence { get; private set; }
        public string ColumnName { get; private set; }
        public string DisplayColumnName { get; private set; }
        public string ColumnType { get; private set; }
    }
}
