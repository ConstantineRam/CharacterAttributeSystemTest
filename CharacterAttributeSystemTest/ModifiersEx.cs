using System.Text.Json;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

namespace CharacterAttributeSystemTest;

/// <summary>
/// The ModifiersEx class provides extension methods for the AttributeModifierParent class and the IModifier interface.
/// </summary>
public static class ModifiersEx
{
    private static readonly JsonSerializerOptions SerializerOptions
        = new JsonSerializerOptions
        {
            WriteIndented = true
        };

    /// <summary>
    /// Checks if the given modifier is infinite.
    /// </summary>
    /// <param name="modifier">The modifier to check.</param>
    /// <returns>
    /// <c>true</c> if the modifier is infinite; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsInfinite(this IModifier modifier)
        => modifier.TicksToExpire == Modifiers.Infinite;

    /// <summary>
    /// Serializes the AttributeModifierParent object to a JSON string.
    /// </summary>
    /// <param name="modifier">The AttributeModifierParent object to serialize.</param>
    /// <returns>A JSON string representation of the AttributeModifierParent object.</returns>
    public static string Serialize(this AttributeModifierParent modifier)
    => JsonSerializer.Serialize(modifier, SerializerOptions);

    /// <summary>
    /// Serializes the provided <see cref="AttributeModifierParent"/> object to a file.
    /// </summary>
    /// <param name="modifier">The <see cref="AttributeModifierParent"/> object to serialize.</param>
    /// <param name="filename">The name of the file to write the serialized data to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SerializeToFile(this AttributeModifierParent modifier, string filename)
    {
        var directoryAttribute = modifier.GetType().GetCustomAttribute<DirectoryPathAttribute>();

        try
        {
            var directory = directoryAttribute == null ? "" : $"{directoryAttribute.DirectoryPath}/";
            Directory.CreateDirectory(directory);
            await using var fileStream = File.Create(directory + filename);
            await JsonSerializer.SerializeAsync(fileStream, modifier, SerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
    

}