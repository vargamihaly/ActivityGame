
using System.Reflection;

namespace ActivityGameBackend.Common.Core.Reflection;
public static class AssemblyScanner
{
    public static IEnumerable<Assembly> GetAssemblies(string assemblyPrefix)
    {
        var path = AppContext.BaseDirectory;
        var directory = new DirectoryInfo(path);

        if (!directory.Exists)
            throw new InvalidOperationException($"FATAL error: directory at path '{path}' does not exist");

        // Logging the files in the directory
        Console.WriteLine($"Scanning directory: {path}");

        foreach (var file in directory.GetFiles("*.dll"))
        {
            Console.WriteLine($"Found DLL: {file.Name}");
        }

        var assemblyList = directory
            .GetFiles($"{assemblyPrefix}*.dll")  // Adjust this pattern as needed
            .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file.FullName).ToString())).ToList();

        if (assemblyList.Count == 0)
        {
            Console.WriteLine("No assemblies found with the specified prefix.");
        }

        return assemblyList;
    }

    public static IEnumerable<(Assembly Assembly, string ResourceName)> GetResourceDetailsFromAssemblies(string assemblyPrefix, Predicate<string> resourceFilter)
    {
        var containerAssemblies = GetAssemblies(assemblyPrefix)
            .Where(x => x.GetManifestResourceNames()
            .Any(y => resourceFilter(y)));

        foreach (var containerAssembly in containerAssemblies)
        {
            var resourceName = containerAssembly.GetManifestResourceNames().FirstOrDefault(x => resourceFilter(x));
            if (!string.IsNullOrWhiteSpace(resourceName))
                yield return (containerAssembly, resourceName);
        }
    }

    public static string? GetResourceFromAssemblies(string assemblyPrefix, Predicate<string> resourceFilter)
    {
        var containerAssemblies = GetAssemblies(assemblyPrefix)
            .Where(x => x.GetManifestResourceNames()
            .Any(y => resourceFilter(y)));

        var containerAssembly = containerAssemblies.FirstOrDefault();
        if (containerAssembly == null) return null;

        var resourceName = containerAssembly.GetManifestResourceNames().FirstOrDefault(x => resourceFilter(x));
        if (resourceName == null) return null;

        var resourceStream = containerAssembly.GetManifestResourceStream(resourceName);
        if (resourceStream == null) return null;

        using var streamReader = new StreamReader(resourceStream);
        return streamReader.ReadToEnd();
    }
}