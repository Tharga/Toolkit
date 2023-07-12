using System;

namespace Tharga.Toolkit.Compare
{
    public class DifferentTypes : IDiff
    {
        public DifferentTypes(string objectName, Type type, Type otherType, int? index)
        {
            ObjectName = objectName;
            Message = string.Format("The types differs. One type is {0} and the other is {1} in object {2}.", type, otherType, objectName);
            Index = index;
        }

        public string Message { get; }
        public string ObjectName { get; }
        public string OtherObjectName => string.Empty;
        public int? Index { get; }
    }
}