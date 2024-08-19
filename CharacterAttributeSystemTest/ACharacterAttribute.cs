
using System.Runtime.CompilerServices;

namespace CharacterAttributeSystemTest;

/// <summary>
/// Abstract base class for character attributes. This object is not designed to be visible outside its assembly and CharacterAttributeSet
/// All attribute data provided from it is presented as Immutable Struct.
/// </summary>
public abstract class ACharacterAttribute(ITimeManager timeManager, in int startingValue = ACharacterAttribute.DefaultValue) : ICharacterAttribute
{

    private readonly ITimeManager _timeManager = timeManager;
    /// <summary>
    /// Value before any mods applied.
    /// We save it, so we don't need to recalculate it every time We need to send it to UI.
    /// For the purpose of this test ability to ++/-- core value is omitted.
    /// </summary>
    private int _coreValue = startingValue;

    /// <summary>
    /// Actual value of the character attribute.
    /// </summary>
    private int _currentValue = startingValue;

    /// <summary>
    /// Represents a tunnel that holds a queue of actions to be executed asynchronously.
    /// </summary>
    private readonly Tunnel<Func<Task>> _tunnel = new Tunnel<Func<Task>>();

    /// <summary>
    /// Represents a set of tags associated with a character attribute.
    /// </summary
    private readonly HashSet<string> _tags = [];
    
    /// <remarks>
    /// The queue is used to keep track of the expiration tokens for modifiers that affect the character attribute.
    /// </remarks>
    private readonly List<ExpirationToken> _expirationQueue = [];

    /// <summary>
    /// The default starting value for a character attribute.
    /// </summary>
    protected const int DefaultValue = 10;

    /// <summary>
    /// Gets the name of the character attribute.
    /// </summary>
    /// <returns>The name of the character attribute.</returns>
    public abstract string GetName();

    /// <summary>
    /// Gets the current value of the character attribute.
    /// </summary>
    /// <returns>The current value of the attribute.</returns>
    public int GetCurrentValue()
    {
        return _currentValue;
    }

    /// <summary>
    /// Adds a task to the queue for processing.
    /// </summary>
    /// <param name="action">The task to be added to the queue.</param
    public void PushTaskIntoQueue(Func<Task> action)
    {
        _tunnel.Write(action);
    }


    /// <summary>
    /// Updates the character attribute by processing tasks in the queue and removing expired modifiers.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Update(CancellationToken ct)
    {
        
        while (_tunnel.Any())
        {
            if (ct.IsCancellationRequested) return;
            var toProcess = await _tunnel.ReadASync(ct);
            if (toProcess is not null)
            { 
                await toProcess.Invoke().ConfigureAwait(false);
            }
        }

        foreach (var token in _expirationQueue.ToArray())
        {
            if (!token.IsExpired()) continue;
            Console.WriteLine($"    {token.GetModifier().Name} is ready to expire from {this.GetName()}.");
            _expirationQueue.Remove(token);

           if (token.GetModifier() is IRemovable removable) this.PushTaskIntoQueue( async() => await removable.Remove(this, ct));
        }
        
    }

    /// <summary>
    /// Accepts a modifier and applies it to the character attribute.
    /// </summary>
    /// <param name="modifier">The modifier to be accepted and applied.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Accept(IModifier modifier, CancellationToken ct)
    {
        if (ReferenceEquals(null, modifier))
        {
            // Error processing
            return;
        }

        if (modifier is ITargetsModifiers && HasTokesWithIntersectedTag(modifier.GetTags()))
        {
            Console.WriteLine($"    {modifier.Name} directed to {this.GetName()}.");
            PushTaskIntoQueue(async() => await modifier.Visit(this, ct)); 
            return;
        }

        if (modifier.HasTag(CharacterAttributeTags.All) || HasIntersectedTag(modifier.GetTags()))
        { 
            Console.WriteLine($"    {modifier.Name} directed to {this.GetName()}.");
            PushTaskIntoQueue(async() => await modifier.Visit(this, ct));   
        }
        
    }

    /// <summary>
    /// Applies a modifier value to the current value of the character attribute. The modifier value is added to the current value.
    /// </summary>
    /// <param name="modifier">The value of the modifier to be applied.</param>
    public void ApplyToValue(int modifier)
    {
        _currentValue += modifier;
    }

    /// <summary>
    /// Adds an expiration token to the expiration queue.
    /// </summary>
    /// <param name="token">The expiration token to be added to the queue.</param>
    public void AddToExpirationQueue(ExpirationToken token)
    {
        _expirationQueue.Add(token);
    }

    /// <summary>
    /// Determines whether the specified modifier is present in the character attribute.
    /// </summary>
    /// <param name="modifier">The modifier to check.</param>
    /// <returns>True if the modifier is present, otherwise false.</returns>
    public bool HasModifier(IModifier modifier)
        => !GetTokenForModifier(modifier).IsNullObject;

    /// <summary>
    /// Retrieves the expiration token associated with the given modifier.
    /// </summary>
    /// <param name="modifier">The modifier to retrieve the token for.</param>
    /// <returns>The expiration token associated with the modifier. If no token is found, a null object is returned.</returns>
    public ExpirationToken GetTokenForModifier(IModifier modifier)
    {
        
        try
        {
         return _expirationQueue.First(x => x.GetModifier().Equals(modifier) );
        }
        catch (Exception e)
        {
            return ExpirationToken.CreateNullObject(_timeManager);
        }
    }


    /// <summary>
    /// Converts the character attribute to an immutable struct.
    /// </summary>
    /// <returns>The immutable struct representing the character attribute.</returns>
    public CharacterAttributeData ToImmutableStruct()
        => new CharacterAttributeData(name:GetName(), coreValue: _coreValue, currentValue: _currentValue);

    /// <summary>
    /// Retrieves all the tags associated with the character attribute.
    /// </summary>
    /// <returns>An enumerable collection of strings representing the tags.</returns>
    public IEnumerable<string> GetTags()
        => _tags.ToArray();

    /// <summary>
    /// Determines whether the character attribute has the specified tag.
    /// </summary>
    /// <param name="tag">The tag to check.</param>
    /// <returns>True if the character attribute has the specified tag; otherwise, false.</returns>
    public bool HasTag(string tag)
        => _tags.Any(s => s == tag);

    /// <summary>
    /// Checks if the character attribute has any tags that intersect with the given tags.
    /// </summary>
    /// <param name="tags">The tags to compare with the attribute tags.</param>
    /// <returns>True if there is an intersection between the tags, otherwise false.</returns>
    public bool HasIntersectedTag(IEnumerable<string> tags)
        => _tags.Intersect(tags).Any();

    /// <summary>
    /// Pushes a tag into the attribute's tag collection.
    /// </summary>
    /// <param name="tag">The tag to be pushed.</param>
    protected void PushTag (string tag)
    {
        if (_tags.Add(tag)) return;
        Console.WriteLine($"Attempt to write {tag} to attribute {this.GetType()}, but it already exists.");
    }

    /// <summary>
    /// Returns a debug string representation of the character attribute.
    /// </summary>
    /// <returns>A string representing the attribute value and name.</returns>
    public string ToDebugString()
    {
        var data = ToImmutableStruct();
        var attValue = data.CurrentValue.ToString();
        if (data.IsModified())
        {
            attValue = $"{data.CurrentValue}/{data.CoreValue}";
        }

        return $" {data.Name}: {attValue};";
    }

    /// <summary>
    /// Returns the list of tokens with the specified tag from the expiration queue.
    /// </summary>
    /// <param name="tag">The tag to filter the tokens by.</param>
    /// <returns>The list of tokens with the specified tag.</returns>
    public IEnumerable<ExpirationToken> GetTokensWithTag(string tag)
        => _expirationQueue.Where(t => t.GetModifier().HasTag(tag));

    /// <summary>
    /// Checks if any expiration token in the attribute's expiration queue has intersected tags with the provided tags.
    /// </summary>
    /// <param name="tags">The tags to check for intersection.</param>
    /// <returns>True if any expiration token has intersected tags with the provided tags, false otherwise.</returns>
    public bool HasTokesWithIntersectedTag(IEnumerable<string> tags)
        => _expirationQueue.Any(token => tags.Intersect(token.GetModifier().GetTags()).Any());


    /// <summary>
    /// Retrieves all expiration tokens associated with the specified tags.
    /// </summary>
    /// <param name="tags">The tags to filter the expiration tokens by.</param>
    /// <returns>An enumerable collection of expiration tokens.</returns>
    public IEnumerable<ExpirationToken> GetTokensWithTags(IEnumerable<string> tags)
    {
        var result = new HashSet<ExpirationToken>();
        foreach (var tag in tags)
        {
            var found = GetTokensWithTag(tag);
            foreach (var token in found)
            {
                result.Add(token);
            }
        }

        return result;
    }
    
}