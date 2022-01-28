
using IllusionPackageCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace IllusionPackageUpdate;

internal static class FS
{
    internal static async ValueTask<ConcurrentDictionary<string, LocalPackage>> GetLocalPackages(CancellationToken cancellationToken = default)
    {
        var path = Path.Join(Config.GameDir, "packages.json");
        if (!File.Exists(path)) return new();

        await using var stream = File.OpenRead(path);

        var result = await JsonSerializer.DeserializeAsync<ConcurrentDictionary<string, LocalPackage>>(stream, cancellationToken: cancellationToken);
        if (result is null) throw new InvalidOperationException("No packets");

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
