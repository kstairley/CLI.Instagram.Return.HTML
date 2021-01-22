namespace TechShare.Utility.Tools.DataStructures
{
    public class KeyValueItem
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public bool HasAnyData
        {
            get
            {
                return !string.IsNullOrEmpty(Key) ||
                    (Value != null);
            }
        }
        public bool HasAllData
        {
            get
            {
                return !string.IsNullOrEmpty(Key) &&
                    Value != null;
            }
        }
    }
}
