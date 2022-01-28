
using IllusionPackageCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace IllusionPackageUpdate;

internal static class FS
{
    internal static async ValueTask<ConcurrentDictionary<string, LocalPackage>> GetLocalPackages(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(Config.PackagesPath)) return new();

        await using var stream = File.OpenRead(Config.PackagesPath);

        var result = await JsonSerializer.DeserializeAsync<ConcurrentDictionary<string, LocalPackage>>(stream, cancellationToken: cancellationToken);
        if (result is null) throw new InvalidOperationException($"{Config.PackagesPath} is invalid. Fix or remove file. Must be valid json");

        return result;
    }

    internal static async ValueTask<IReadOnlyDictionary<GameToken, IReadOnlyDictionary<string, WebPackage>>> GetWebPackages(CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        await using var stream = await client.GetStreamAsync(Config.PackagesUrl, cancellationToken);

        var result = await JsonSerializer.DeserializeAsync<IReadOnlyDictionary<GameToken, IReadOnlyDictionary<string, WebPackage>>>(stream, cancellationToken: cancellationToken);
        if (result is null) throw new InvalidOperationException("No packets");

        return result;
    }
}
