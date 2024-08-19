using System.Text.Json.Serialization;

namespace CharacterAttributeSystemTest;

/// <summary>
/// Represents an attribute modifier that changes the value of a character attribute.
/// </summary>
[DirectoryPath("ChangeValue")]
public sealed class AttributeModifierChangeValue : AttributeModifierParent, IRemovable
{
    /// <summary>
    /// Represents an attribute modifier that changes the applied value of a character attribute.
    /// </summary>
    [JsonInclude][JsonPropertyName("appliedValue")]
    private int AppliedValue { get; }

    [JsonConstructor]
    private AttributeModifierChangeValue(Guid id, HashSet<string>? tags, int appliedValue, int ticksToExpire, string name, string description) : base(id, tags, ticksToExpire,  name, description)
    {
        AppliedValue = appliedValue; 
    }

    public AttributeModifierChangeValue(ITimeManager timeManager, HashSet<string>? tags, int appliedValue, int ticksToExpire, string name, string description) : base (timeManager,tags, ticksToExpire, name, description)
    {
        AppliedValue = appliedValue;  
    }

    /// <summary>
    /// Visits a character attribute and applies the self to it.
    /// </summary>
    /// <param name="characterAttribute">The character attribute to visit.</param>
    /// <param name="ct">The cancellation token.</param>
    public override async Task Visit(ICharacterAttribute characterAttribute, CancellationToken ct)
    {
        const string unexpectedNullError = "Unexpected error; null at character attribute.";
        const string ignoredExistenceFormat = "{0} ignored by {1} because already exists.";
        const string resetExistenceFormat = "{0} was reset for {1}.";
        const string appliedValueFormat = "{0} was applied to {1} {2}{3}.";

        if (characterAttribute == null)
        {
            Console.WriteLine(unexpectedNullError);
            return;
        }

        if (CheckAndWriteLogForTag(AttributeModifierRulesTags.IgnoreIfExists, characterAttribute,
                ignoredExistenceFormat))
        {
            return;
        }

        if (CheckAndWriteLogForTag(AttributeModifierRulesTags.ResetIfExists, characterAttribute, resetExistenceFormat))
        {
            characterAttribute.GetTokenForModifier(this).Reset();
            return;
        }

        characterAttribute.ApplyToValue(AppliedValue);
        var sign = AppliedValue < 0 ? "" : "+";
        Console.WriteLine(appliedValueFormat, Name, characterAttribute.GetName(), sign, AppliedValue);

        if (ct.IsCancellationRequested) return;

        characterAttribute.AddToExpirationQueue(new ExpirationToken(this, GetTimeManager));
    }

    private bool CheckAndWriteLogForTag(string tag, ICharacterAttribute characterAttribute, string messageFormat)
    {
        if (HasTag(tag) && characterAttribute.HasModifier(this))
        {
            Console.WriteLine(messageFormat, Name, characterAttribute.GetName());
            return true;
        }

        return false;
    }

    /// <summary>
    /// Verifies the integrity of the attribute modifier.
    /// </summary>
    /// <returns>
    /// Returns true if the attribute modifier is valid and has the required tags,
    /// otherwise returns false and prints an error message.
    /// </returns>
    public override bool VerifyIntegrity()
    {
        if (this.Tags.Count >= 1) return true;
        Console.WriteLine($"Error. '{GetType()}' '{Name}' modifier has no tags. Ignored.");
        return false;

    }

    /// <summary>
    /// Removes the modifier from the character attribute.
    /// </summary>
    /// <param name="characterAttribute">The character attribute to remove the modifier from.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous removal.</returns>
    public async Task Remove(ICharacterAttribute characterAttribute, CancellationToken ct)
    {
        if (ReferenceEquals(null, characterAttribute))
        {
            Console.WriteLine($"    {this.Name} got null at characterAttribute for Remove.");
            return;
        }

        characterAttribute.ApplyToValue(AppliedValue * -1);
        Console.WriteLine($"    {this.Name} was removed from {characterAttribute.GetName()}  {AppliedValue * -1}.");
    }


}