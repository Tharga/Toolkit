using System.Collections.Generic;

namespace Tharga.Toolkit.Tests.Compare.Entities
{
    public struct SomeTestStruct
    {
        public string StringMember;
        public List<string> StringListMember;
        public string StringProperty { get; set; }
        public List<SomeTestStruct> StructListProperty { get; set; }
    }
}