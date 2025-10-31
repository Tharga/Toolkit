namespace Tharga.Toolkit.Password;

public record ApiKeyOptions
{
    public int SaltSize { get; set; } = 16;
    public int HashSize { get; set; } = 20;
    public int Iterations { get; set; } = 10000;
}