using System.Text.Json;

namespace CharacterAttributeSystemTest;
using System.Reflection;

/// <summary>
/// Class to load attribute modifiers for a given type.
/// </summary>
public class AttributeModifierLoader
{
    /// <summary>
    /// Loads and returns a collection of attribute modifiers of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of attribute modifier to load.</typeparam>
    /// <param name="timeManager">The time manager used for timing calculations.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A collection of attribute modifiers of type <typeparamref name="T"/>.</returns>
    public static async Task<IEnumerable<IModifier>> LoadForType<T>(ITimeManager timeManager, CancellationToken ct)
        where T : IModifier
    {
        var directory = GetModifierDirectoryPath<T>();
        if (!Directory.Exists(directory))
        {
            Console.WriteLine($"Directory {directory} doesn't exits");
            return Array.Empty<IModifier>();
        }

        try
        {
            return await LoadModifiersFromDirectory(directory, timeManager);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static string GetModifierDirectoryPath<T>()
    {
        var directory = "";
        var directoryAttribute = typeof(T).GetCustomAttribute<DirectoryPathAttribute>();
        if (directoryAttribute != null)
        {
            directory = $"{directoryAttribute.DirectoryPath}/";
        }

        return directory;
    }

    private static async Task<IEnumerable<IModifier>> LoadModifiersFromDirectory(string directory,
        ITimeManager timeManager)
    {
        var result = new List<IModifier>();
        foreach (var fileName in Directory.EnumerateFiles(directory))
        {
            Console.WriteLine(fileName);
            await using var fileStream = File.OpenRead(fileName);
            var loaded = JsonSerializer.Deserialize<AttributeModifierParent>(fileStream);
            if (loaded == null)
            {
                Console.WriteLine($"Failed to load file {fileName}");
                continue;
            }

            if (!loaded.VerifyIntegrity()) continue;
            if (loaded is ITimeManagerInjectee ij)
            {
                ij.InjectTimeManager(timeManager);
            }

            result.Add(loaded);
        }

        return result;
    }
   
}