namespace CharacterAttributeSystemTest;

public readonly struct CharacterAttributeData(string name, int currentValue, int coreValue)
{
    public string Name { get; } = name;
    public int CurrentValue { get; } = currentValue;
    public int CoreValue { get; } = coreValue;
}