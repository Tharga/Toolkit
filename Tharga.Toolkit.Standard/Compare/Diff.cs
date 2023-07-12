namespace Tharga.Toolkit.Compare
{
    public class Diff : IDiff
    {
        public Diff(string objectName, string otherObjectName, string message, int? index)
        {
            ObjectName = objectName ?? "N/A";
            OtherObjectName = otherObjectName ?? "N/A";
            Message = message;
            Index = index;

            if (ObjectName.Contains("[]") && index != null)
            {
                ObjectName = ObjectName.Replace("[]", $"[{index}]");
            }
        }

        public string ObjectName { get; }
        public int? Index { get; }
        public string OtherObjectName { get; }
        public string Message { get; }
    }
}