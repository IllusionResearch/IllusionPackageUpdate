using Microsoft.Win32;

namespace IllusionPackageUpdate;

internal class Config
{
    internal static string PackagesUrl { get; } = "https://illusionresearch.github.io/IllusionPackageProvider/packages.json";
    internal static string GameDir { get; private set; } = string.Empty;
    internal static string PackagesPath { get; private set; } = string.Empty;

    static Config()
    {
        GameDir = GetGameDir();
        PackagesPath = Path.Combine(GameDir, "packages.json");
    }

    private static string GetGameDir()
    {

        using var key = Registry.CurrentUser.OpenSubKey(@"Software\illusion\Koikatu\Koikatu");
        if (key is null) throw new InvalidOperationException("No regystry record found");

        var regInstallDir = key.GetValue("INSTALLDIR");
        if (regInstallDir is not string installDir) throw new InvalidOperationException("No installdir found");

        return installDir;
    }
}
