using System.Text.Json.Serialization;

namespace CharacterAttributeSystemTest;

/// <summary>
/// AttributeModifierPurge class represents a modifier that purges (expires) tokens with specific tags from a character attribute.
/// </summary>
[DirectoryPath("Purge")]
public class AttributeModifierPurge : AttributeModifierParent, ITargetsModifiers
{

    [JsonConstructor]
    private AttributeModifierPurge(Guid id, HashSet<string>? tags, int ticksToExpire, string name, string description) : base(id, tags, ticksToExpire,  name, description)
    {
    }
    
    public AttributeModifierPurge(ITimeManager timeManager, HashSet<string>? tags, int ticksToExpire, string name, string description) : base (timeManager,tags, ticksToExpire, name, description)
    {
    }

    /// <summary>
    /// Method to visit a character attribute and remove all tokens with specific tags.
    /// </summary>
    /// <param name="characterAttribute">The character attribute to visit.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task Visit(ICharacterAttribute characterAttribute, CancellationToken ct)
    {
        if (ReferenceEquals(null, characterAttribute))
        {
            Console.WriteLine($"Unexpected error; null at character attribute.");
            return;
        }

        foreach (var tag in GetTags())
        {
            foreach (var token in characterAttribute.GetTokensWithTag(tag))
            {
                token.ForceExpiration();
                Console.WriteLine($"    {token.GetModifier().Name} was marked for expiration from {characterAttribute.GetName()} by {Name}.");
            }  
        } 

    }

    /// <summary>
    /// Method to verify the integrity of an attribute modifier.
    /// </summary>
    /// <returns>
    /// True if the attribute modifier passes the integrity check, false otherwise.
    /// </returns>
    public override bool VerifyIntegrity()
    {
        if (this.Tags.Count >= 1) return true;
        Console.WriteLine($"Error. '{GetType()}' '{Name}' modifier has no tags. Ignored.");
        return false;

    }
}