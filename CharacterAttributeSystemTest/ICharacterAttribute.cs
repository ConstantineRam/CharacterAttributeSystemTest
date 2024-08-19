namespace CharacterAttributeSystemTest;

public interface ICharacterAttribute : ITagged
{
    string GetName();


    int GetCurrentValue();

    void PushTaskIntoQueue(Func<Task> action);
    Task Update(CancellationToken ct);
    Task Accept(IModifier modifier, CancellationToken ct);

    void ApplyToValue(int modifier);
    void AddToExpirationQueue(ExpirationToken token);

    bool HasModifier(IModifier modifier);
    ExpirationToken GetTokenForModifier(IModifier modifier);
    CharacterAttributeData ToImmutableStruct();
    string ToDebugString();
    
    IEnumerable<ExpirationToken> GetTokensWithTag(string tag);

}