using IllusionPackageUpdate;
using System.IO.Compression;

internal static class ZipArchiveEntryExtension
{
    internal static bool IsRegularFile(this ZipArchiveEntry entry) =>
        !Path.EndsInDirectorySeparator(entry.FullName);
}

internal sealed class PackageUtils
{
    internal async ValueTask<IEnumerable<string>> Install(string url, CancellationToken cancellationToken = default)
    {
        var stream = await _client.GetStreamAsync(url, cancellationToken);
        var archive = new ZipArchive(stream);

        archive.ExtractToDirectory(Config.GameDir, true);
        
        return archive.Entries.Where(entry => entry.IsRegularFile()).Select(entry => entry.FullName);
    }

    public PackageUtils(HttpClient client) => _client = client;

    private readonly HttpClient _client;
}
