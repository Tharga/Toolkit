namespace Tharga.Toolkit.Compare
{
    public interface IDiff
    {
        string Message { get; }
        string ObjectName { get; }
        string OtherObjectName { get; }
        int? Index { get; }
    }
}