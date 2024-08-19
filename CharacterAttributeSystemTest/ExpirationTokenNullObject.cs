namespace CharacterAttributeSystemTest;

public class ExpirationTokenNullObject : ExpirationToken
{
    public ExpirationTokenNullObject( ITimeManager timeManager) : base(AttributeModifierParent.CreateNullObject(timeManager), timeManager)
    {
        IsNullObject = true;
    }
}