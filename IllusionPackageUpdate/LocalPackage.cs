namespace IllusionPackageUpdate;

internal sealed record LocalPackage
{
    public Version Version { get; set; } = new Version();
    public IReadOnlyList<string> Files { get; set; } = Array.Empty<string>();
}
