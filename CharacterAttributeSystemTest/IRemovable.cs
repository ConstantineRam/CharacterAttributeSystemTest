namespace CharacterAttributeSystemTest;

public interface IRemovable
{
    Task Remove(ICharacterAttribute characterAttribute, CancellationToken ct);
}