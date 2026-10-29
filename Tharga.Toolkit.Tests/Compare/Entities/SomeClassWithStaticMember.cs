namespace Tharga.Toolkit.Tests.Compare.Entities
{
    public class SomeClassWithStaticMember
    {
        public static string Shared => "one";
        public string Data { get; set; }
    }

    public class OtherClassWithStaticMember
    {
        public static string Shared => "other";
        public string Data { get; set; }
    }
}
