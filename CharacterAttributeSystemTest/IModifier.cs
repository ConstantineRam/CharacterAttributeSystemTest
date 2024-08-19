using System.Text.Json.Serialization;

namespace CharacterAttributeSystemTest;


public interface IModifier: ITagged
{
    string Name { get; }

    Task Visit(ICharacterAttribute characterAttribute, CancellationToken ct);

    int TicksToExpire { get; }
    
}