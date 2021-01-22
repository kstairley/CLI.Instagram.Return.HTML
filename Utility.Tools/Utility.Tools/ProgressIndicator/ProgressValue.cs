namespace TechShare.Utility.Tools.ProgressIndicator
{
    public class ProgressValue
    {
        public int? PercentComplete { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            string retVal = string.Empty;

            if (!string.IsNullOrEmpty(Message))
                retVal += Message + "... ";

            if (PercentComplete.HasValue)
                retVal += PercentComplete.Value.ToString() + "%";

            return !string.IsNullOrEmpty(retVal) ? retVal : null;
        }
    }
}
