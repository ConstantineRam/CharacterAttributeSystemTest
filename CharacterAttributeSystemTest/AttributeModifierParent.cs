using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CharacterAttributeSystemTest;

[JsonDerivedType(typeof(AttributeModifierChangeValue), "ChangeValue")]
[JsonDerivedType(typeof(AttributeModifierPurge), "Purge")]
public abstract class AttributeModifierParent : IModifier, INullObject, IEquatable<AttributeModifierParent>, ITimeManagerInjectee
{
    /// <summary>
    /// Represents the unique identifier for an attribute modifier.
    /// </summary>
    [JsonInclude] 
    private Guid Id { get;  }
    
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; private set; }

    
    [JsonInclude]
    [JsonPropertyName("description")]
    private string Description { get; set; }

    /// <summary>
    /// Gets the number of ticks remaining for the modifier to expire.
    /// </summary>
    /// <value>
    /// The number of ticks remaining for the modifier to expire.
    /// </value>
    [JsonInclude]
    [JsonPropertyName("ticksToExpire")]
    public int TicksToExpire { get; private set; }
    
    [JsonInclude]
    [JsonPropertyName("tags")]
    protected HashSet<string> Tags { get; private set; }
    
    private ITimeManager _timeManager;
    
    protected ITimeManager GetTimeManager => _timeManager;
    
    protected internal ITimeManager SetTimeManager
    {
        set => _timeManager = value;
    }

    /// <summary>
    /// Applies tags to the attribute modifier.
    /// </summary>
    /// <param name="targetTags">The set of tags to apply to the attribute modifier. If null, all tags will be set to CharacterAttributeTags.All.</param>
    private void ApplyTags(HashSet<string>? targetTags)
    {
        if (targetTags == null)
        {
            Tags =
            [
                CharacterAttributeTags.All
            ];
            return;
        }

        Tags = targetTags;
    }

    protected AttributeModifierParent(ITimeManager timeManager, HashSet<string>? tags, int ticksToExpire,
        string name, string description)
    {
        Id = Guid.NewGuid();
        SetTimeManager = timeManager;
        Name = name;
        Description = description;
        TicksToExpire = ticksToExpire;

        ApplyTags(tags);
    }
    
    [JsonConstructor]
    private protected AttributeModifierParent(Guid id, HashSet<string>? tags, int ticksToExpire, string name,
        string description)
    {
        Id = id;
        _timeManager = new TimeManagerMock(10); //
        Name = name;
        Description = description;
        TicksToExpire = ticksToExpire;

        ApplyTags(tags);
        
    }

    /// <summary>
    /// Performs a visit operation on the character attribute.
    /// </summary>
    /// <param name="characterAttribute">The character attribute to visit.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task Visit(ICharacterAttribute characterAttribute, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Converts a JSON string into an instance of AttributeModifierParent.
    /// </summary>
    /// <param name="dataString">The JSON string to deserialize.</param>
    /// <param name="timeManager">The ITimeManager instance used to inject time manager.</param>
    /// <returns>An instance of AttributeModifierParent if deserialization is successful; otherwise, returns a null object of AttributeModifierParent.</returns>
    public static AttributeModifierParent FromString([NotNull] in string dataString,
        [NotNull] in ITimeManager timeManager)
    {
        try
        {
            var result = JsonSerializer.Deserialize<AttributeModifierParent>(dataString);
            if (result == null) return CreateNullObject(timeManager);

            result.SetTimeManager = timeManager;
            return result;
        }
        catch (Exception e)
        {
            return CreateNullObject(timeManager);
        }
    }

    #region NullObject

    /// <summary>
    /// Determines if an object is a null object.
    /// </summary>
    [JsonIgnore] public bool IsNullObject { get; protected init; } = false;

    /// <summary>
    /// Creates a null object of type AttributeModifierParent using the provided time manager.
    /// </summary>
    /// <param name="timeManager">The time manager to be injected into the null object.</param>
    /// <returns>A null object of type AttributeModifierParent.</returns>
    public static AttributeModifierParent CreateNullObject(ITimeManager timeManager)
        => new AttributeModifierNullObject(timeManager);
    #endregion


    /// <summary>
    /// Determines whether the current AttributeModifierParent instance is equal to another AttributeModifierParent instance.
    /// </summary>
    /// <param name="other">The AttributeModifierParent instance to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the current AttributeModifierParent instance is equal to the other parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(AttributeModifierParent? other)
    {
        if (ReferenceEquals(other, null)) return false;
        return this.Id == other.Id;
    }

    /// <summary>
    /// Determines whether the current object is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// true if the specified object is equal to the current object; otherwise, false.
    /// </returns>
    public override bool Equals(object? obj)
        => obj is AttributeModifierParent atm && this.Equals(atm);
    
    public static bool operator ==(AttributeModifierParent? a, AttributeModifierParent? b)
    {
        return a switch
        {
            null when b is null => true,
            null => false,
            _ => a.Equals(b)
        };
    }

    public static bool operator !=(AttributeModifierParent a, AttributeModifierParent? b)
    {
        return a switch
        {
            null when b is null => false,
            null => true,
            _ => !a.Equals(b)
        };
    }

    public override int GetHashCode() => this.Id.GetHashCode();

    /// <summary>
    /// Used via interface to inject time manager, bc I am lazy to write custom deserializer
    /// </summary>
    /// <param name="timeManager">The time manager instance that will be injected</param>
    public void InjectTimeManager(in ITimeManager timeManager)
    {
        SetTimeManager = timeManager;
    }

    #region ITagged

    public IEnumerable<string> GetTags()
        => Tags.ToArray();

    /// <summary>
    /// Checks if the AttributeModifierParent instance has the specified tag.
    /// </summary>
    /// <param name="tag">The tag to check.</param>
    /// <returns>True if the instance has the tag; otherwise, false.</returns>
    public bool HasTag(string tag)
        => Tags.Any(s => s == tag);

    /// <summary>
    /// Checks if the attribute modifier has intersected a given set of tags.
    /// </summary>
    /// <param name="tags">The set of tags to check against.</param>
    /// <returns>True if the attribute modifier has intersected any of the tags, otherwise false.</returns>
    public bool HasIntersectedTag(IEnumerable<string> tags)
        => Tags.Intersect(tags).Any();
    #endregion

    /// <summary>
    /// Method to verify the integrity of an attribute modifier.
    /// </summary>
    /// <returns>
    /// True if the attribute modifier passes the integrity check, false otherwise.
    /// </returns>
    public abstract bool VerifyIntegrity();
}