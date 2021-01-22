namespace TechShare.Utility.Tools.TableDefinitions
{
    public class ForeignKey
    {
        public string PreservationPrefix { get; set; }
        public string TableNameTableName { get { return !string.IsNullOrEmpty(PreservationPrefix.Trim()) ? PreservationPrefix.Trim() + BaseTableName : BaseTableName; } }
        public string BaseTableName { get; set; }
        public string ColumnName { get; set; }
        public string FKTableName { get { return !string.IsNullOrEmpty(PreservationPrefix) ? PreservationPrefix.Trim() + BaseFKTableName : BaseFKTableName; } }
        public string BaseFKTableName { get; set; }
        public string FKColumnName { get; set; }
        public ForeignKey Clone()
        {
            return new ForeignKey()
            {
                PreservationPrefix = PreservationPrefix,
                FKColumnName = FKColumnName,
                BaseFKTableName = BaseFKTableName,
                BaseTableName = BaseTableName,
                ColumnName = ColumnName
            };
        }
    }
}
