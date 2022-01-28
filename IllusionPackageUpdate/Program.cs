using IllusionPackageCore;
using IllusionPackageUpdate;
using System.Text.Json;

var localPackages = await FS.GetLocalPackages();
var webPackages = await FS.GetWebPackages();

if (webPackages is not null && webPackages.TryGetValue(GameToken.Koikatsu, out var urls))
{
    Console.WriteLine($"Game dir: {Config.GameDir}");

    using var client = new HttpClient();
    var installer = new PackageUtils(client);

    await Parallel.ForEachAsync(urls, async (webPackage, cancellationToken) =>
    {
        if (localPackages.TryGetValue(webPackage.Key, out var localPackage))
        {
            if (webPackage.Value.Version == localPackage.Version)
            {
                Console.WriteLine($"> skipped [{webPackage.Key}] {webPackage.Value.Version} (already installed with same version)");
                return;
            }

            Console.WriteLine($"> update [{webPackage.Key}] {webPackage.Value.Version} != {localPackage.Version}");
            localPackage.Version = webPackage.Value.Version;

            foreach (var name in localPackage.Files)
            {
                File.Delete(Path.Combine(Config.GameDir, name));
            }

            var files = await installer.Install(webPackage.Value.Url, cancellationToken);
            localPackage.Files = files.ToArray();
        }
        else
        {
            var toInstall = new LocalPackage { Version = webPackage.Value.Version };
            if (!localPackages.TryAdd(webPackage.Key, toInstall))
            {
                Console.WriteLine($"> failed [{webPackage.Key}]");
                return;
            }

            Console.WriteLine($"> install [{webPackage.Key}] {webPackage.Value.Version}");

            var files = await installer.Install(webPackage.Value.Url, cancellationToken);
            toInstall.Files = files.ToArray();
        }
    });

    var path = Path.Combine(Config.GameDir, "packages.json");
    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(localPackages));
}
