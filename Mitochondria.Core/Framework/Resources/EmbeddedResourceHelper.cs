using System.Reflection;

namespace Mitochondria.Core.Framework.Resources;

public static class EmbeddedResourceHelper
{
    public static string TryGetMatchingName(
        Assembly sourceAssembly,
        string pathToMatch,
        bool searchRecursively = true)
    {
        var numMatches = GetMatchingPaths(
            sourceAssembly,
            pathToMatch,
            searchRecursively,
            out var matchingResourceNames);

        return numMatches switch
        {
            0 => throw new FileNotFoundException(
                $"The embedded resource \"{pathToMatch}\" was not found in the assembly \"{sourceAssembly}\""),
            1 => matchingResourceNames[0],
            _ => throw new Exception(
                $"The embedded resource \"{pathToMatch}\" has 2 or more matches in the assembly \"{sourceAssembly}\"")
        };
    }

    public static int GetMatchingPaths(
        Assembly sourceAssembly,
        string pathToMatch,
        bool searchRecursively,
        out string[] matchingResourceNames,
        bool endsWithPath = true)
    {
        var resourceNames = sourceAssembly.GetManifestResourceNames();
        
        var stringToMatch = pathToMatch.Replace('/', '.').Replace('\\', '.');

        var resourceName = resourceNames.FirstOrDefault(n => stringToMatch == n);
        if (resourceName != null)
        {
            matchingResourceNames = new[] { resourceName };
            return 1;
        }

        Func<string, bool> matches = endsWithPath
            ? name => name.EndsWith(stringToMatch)
            : name => name.Contains(stringToMatch);

        if (searchRecursively)
        {
            matchingResourceNames = resourceNames.Where(n => matches(n)).ToArray();
            return matchingResourceNames.Length;
        }
        
        matchingResourceNames = Array.Empty<string>();
        return 0;
    }
}