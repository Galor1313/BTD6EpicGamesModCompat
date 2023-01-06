using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BTD6EpicGamesModCompat;

internal static class Resources
{
    // The current running assembly of BTD6EpicGamesModCompat
    private static readonly Assembly thisAssembly = Assembly.GetExecutingAssembly();

    // The name of the current running assembly
    private static readonly string assemblyName = thisAssembly.GetName().Name.Replace(" ", "");

    // Gets all available resource names in the assembly
    private static readonly string[] resourceNames = thisAssembly.GetManifestResourceNames();

    // Gets a resource with the given name
    public static byte[] GetResource(string resourceName)
    {
        // Convert resource name to full
        var fullName = $"{assemblyName}.Resources.{resourceName}";

        // Checks if the resource does not exist
        if (!resourceNames.Contains(fullName))
            return Array.Empty<byte>();

        // Loads and returns resource
        using MemoryStream resourceStream = new();
        try
        {
            thisAssembly.GetManifestResourceStream(fullName).CopyTo(resourceStream);
            return resourceStream.ToArray();
        }
        catch
        {
            return null;
        }
    }
}
