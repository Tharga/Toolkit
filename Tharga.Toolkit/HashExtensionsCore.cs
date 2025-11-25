using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Tharga.Toolkit;

public static class HashExtensionsCore
{
    /// <summary>
    /// Compute a hash from a stream, discarding the data.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static async Task<string> ToHash(this Stream stream, HashFormat format)
    {
        using var md5 = MD5.Create();
        await using var crypto = new CryptoStream(stream, md5, CryptoStreamMode.Read);
        var buffer = new byte[81920];
        while (await crypto.ReadAsync(buffer, 0, buffer.Length) > 0)
        {
        }

        var hash = md5.Hash.Format(format);
        return hash;
    }

    /// <summary>
    /// Compute a hash from a stream while copying its data to another stream.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static async Task<string> ToHashAndCopyTo(this Stream input, Stream output, HashFormat format)
    {
        using var md5 = MD5.Create();
        await using var crypto = new CryptoStream(Stream.Null, md5, CryptoStreamMode.Write);

        var buffer = new byte[81920];
        int read;

        while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            crypto.Write(buffer, 0, read);
            await output.WriteAsync(buffer, 0, read);
        }

        await crypto.FlushFinalBlockAsync();

        return md5.Hash.Format(format);
    }
}