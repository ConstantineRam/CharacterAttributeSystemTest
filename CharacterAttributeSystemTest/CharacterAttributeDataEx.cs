namespace CharacterAttributeSystemTest;

public static class CharacterAttributeDataEx
{
    public static bool IsModified(this CharacterAttributeData data)
        => data.CoreValue != data.CurrentValue;
}