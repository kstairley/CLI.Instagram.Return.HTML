namespace TechShare.Utility.Tools.TableDefinitions
{
    public class FilterValue
    {
        public FilterComparisonEnum ComparisonType { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            return Value != null ? Value.ToString() : base.ToString();
        }
    }
}
