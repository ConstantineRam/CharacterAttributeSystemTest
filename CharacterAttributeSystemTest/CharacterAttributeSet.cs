using System.Collections;
using System.Collections.Immutable;

namespace CharacterAttributeSystemTest;

public class CharacterAttributeSet
{
    private readonly List<ICharacterAttribute> _attributeStorage = [];
    
    public CharacterAttributeSet(ITimeManager timeManager)
    {
        _attributeStorage.Add(new Strength(timeManager));
        _attributeStorage.Add(new Dexterity(timeManager));
        _attributeStorage.Add(new Intellect(timeManager));
        _attributeStorage.Add(new Corruption(timeManager));
        _attributeStorage.Add(new Magic(timeManager));
    }

    /// <summary>
    /// Update the character attribute set by calling the Update method on each attribute in the set.
    /// </summary>
    /// <param name="ct">The cancellation token to check if cancellation is requested.</param>
    /// <returns>A task representing the asynchronous updating of the character attribute set.</returns>
    public async Task Update(CancellationToken ct)
    {
        foreach (var attribute in _attributeStorage)
        {
            if (ct.IsCancellationRequested) return;
            await attribute.Update(ct);
        }   
    }

    /// <summary>
    /// Applies the provided modifier to all attributes in the attribute set.
    /// </summary>
    /// <param name="modifier">The modifier to be applied to the attributes.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ApplyModifier(IModifier modifier, CancellationToken ct)
    {
        foreach (var attribute in _attributeStorage)
        {
            if (ct.IsCancellationRequested) return;
            await attribute.Accept(modifier, ct);
        }   
    }

    /// <summary>
    /// Converts the list of character attributes to an immutable list of CharacterAttributeData.
    /// </summary>
    /// <returns>An immutable list of CharacterAttributeData.</returns>
    public IImmutableList<CharacterAttributeData> ToImmutableList()
        =>_attributeStorage.Select(attribute => attribute.ToImmutableStruct()).ToImmutableList();

    /// <summary>
    /// Returns a string representation of the CharacterAttributeSet for debugging purposes.
    /// </summary>
    /// <returns>A string representation of the CharacterAttributeSet.</returns>
    public string ToDebugString()
        => _attributeStorage.Aggregate($"Attributes: ", (current, attribute) => current + (attribute.ToDebugString()));
    
}