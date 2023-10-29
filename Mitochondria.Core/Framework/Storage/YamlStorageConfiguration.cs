using BepInEx;

namespace Mitochondria.Core.Framework.Storage;

public class YamlStorageConfiguration
{
    public string BasePath { get; }

    public string FileDir { get; }

    public string[] AltFileDirs { get; }

    public string? RequiredFileExtension { get; }

    public YamlStorageConfiguration(
        string fileDir,
        IEnumerable<string>? altFileDirs = null,
        string? basePath = null,
        string? requiredFileExtension = null)
    {
        FileDir = fileDir;
        AltFileDirs = altFileDirs?.ToArray() ?? Array.Empty<string>();
        BasePath = basePath ?? Paths.ConfigPath;

        RequiredFileExtension = requiredFileExtension ?? "yaml";
    }

    public string GetAbsoluteSavePath(string fileName)
        => Path.Combine(BasePath, FileDir, $"{fileName}.{RequiredFileExtension}");

    public IEnumerable<string> GetAbsoluteLoadPaths(string fileName, IEnumerable<string>? altFileNames = null)
    {
        var fileNames = altFileNames?.Prepend(fileName).ToArray() ?? new[] { fileName };

        var validExtensions = string.IsNullOrWhiteSpace(RequiredFileExtension)
            ? new[] { "", "yml", "yaml" }
            : new[] { RequiredFileExtension };

        foreach (var dir in AltFileDirs.Prepend(FileDir))
        {
            foreach (var file in fileNames)
            {
                foreach (var extension in validExtensions)
                {
                    yield return Path.Combine(BasePath, dir, $"{file}.{extension}");
                }
            }
        }
    }
}