using System.Reflection;

namespace Mitochondria.Framework.Resources;

public static class EmbeddedResourceHelper
{
    public static string TryGetMatchingFullName(
        Assembly sourceAssembly,
        string pathToMatch,
        bool searchRecursively = true)
    {
        var numMatches =
            GetMatchingFullNames(sourceAssembly, pathToMatch, searchRecursively, out var matchingResourceNames);

        return numMatches switch
        {
            0 => throw new FileNotFoundException(
                $"The embedded resource \"{pathToMatch}\" was not found in the assembly \"{sourceAssembly}\""),
            1 => matchingResourceNames[0],
            _ => throw new Exception(
                $"The embedded resource \"{pathToMatch}\" has 2 or more matches in the assembly \"{sourceAssembly}\"")
        };
    }

    public static int GetMatchingFullNames(
        Assembly sourceAssembly,
        string pathToMatch,
        bool searchRecursively,
        out string[] matchingResourceNames)
    {
        var resourceNames = sourceAssembly.GetManifestResourceNames();
        
        var stringToMatch = pathToMatch.Replace('/', '.').Replace('\\', '.');

        var resourceName = resourceNames.FirstOrDefault(n => stringToMatch == n);
        if (resourceName != null)
        {
            matchingResourceNames = new[] { resourceName };
            return 1;
        }

        if (searchRecursively)
        {
            matchingResourceNames = resourceNames.Where(n => n.EndsWith(stringToMatch)).ToArray();
            return matchingResourceNames.Length;
        }
        
        matchingResourceNames = Array.Empty<string>();
        return 0;
    }
}