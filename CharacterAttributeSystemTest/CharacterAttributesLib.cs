namespace CharacterAttributeSystemTest;
// For simplicity all attributes are put into one file.
public sealed class Strength : ACharacterAttribute
{
    public Strength(ITimeManager timeManager, int startingValue = ACharacterAttribute.DefaultValue) : base(timeManager,
        startingValue)
    {
        PushTag(CharacterAttributeTags.Physical);
        PushTag(CharacterAttributeTags.Strength);
    }

    public override string GetName() => "Strength";
}

public sealed class Dexterity : ACharacterAttribute
{
    public Dexterity(ITimeManager timeManager, int startingValue = ACharacterAttribute.DefaultValue) : base(timeManager,
        startingValue)
    {
        PushTag(CharacterAttributeTags.Physical);
        PushTag(CharacterAttributeTags.Dexterity);
    }
    public override string GetName() => "Dexterity";
}

public sealed class Intellect : ACharacterAttribute
{
    public Intellect(ITimeManager timeManager, int startingValue = ACharacterAttribute.DefaultValue) : base(timeManager,
        startingValue)
    {
        PushTag(CharacterAttributeTags.Mental);
    }
    public override string GetName() => "Intellect";
}

public sealed class Corruption : ACharacterAttribute
{
    public Corruption(ITimeManager timeManager, int startingValue = ACharacterAttribute.DefaultValue) : base(timeManager,
        startingValue)
    {
        PushTag(CharacterAttributeTags.Eldritch);
    }
    public override string GetName() => "Corruption";
}
public sealed class Magic : ACharacterAttribute
{
    public Magic(ITimeManager timeManager, int startingValue = ACharacterAttribute.DefaultValue) : base(timeManager,
        startingValue)
    {
        PushTag(CharacterAttributeTags.Arcane);
        PushTag(CharacterAttributeTags.Mental);
    }
    public override string GetName() => "Magic";
}

public sealed class CharAttrMock  : ACharacterAttribute
{
    public CharAttrMock(ITimeManager timeManager, int startingValue = ACharacterAttribute.DefaultValue) : base(timeManager,
        startingValue)
    {
        PushTag(CharacterAttributeTags.Physical);
        PushTag(CharacterAttributeTags.Arcane);
        PushTag(CharacterAttributeTags.Mental);
    }
    public override string GetName() => "TEST";
}