namespace CharacterAttributeSystemTest;

/// <summary>
/// Represents a null object implementation of an attribute modifier.
/// </summary>
/// <remarks>
/// The AttributeModifierNullObject class is used when there is no actual attribute modifier defined.
/// It serves as a placeholder or default value when there is no valid modifier for an attribute.
/// </remarks>
public class AttributeModifierNullObject : AttributeModifierParent
{
    /// The AttributeModifierNullObject class represents a null object for attribute modifiers.
    /// It extends the AttributeModifierParent class and implements the INullObject interface.
    /// /
    public AttributeModifierNullObject(ITimeManager timeManager) : base(timeManager, null,Modifiers.Infinite, "NULL", "NULL")
    {
        IsNullObject = true;
    }

    /// <summary>
    /// Verifies the integrity of the attribute modifier. Always false for NullObject.
    /// </summary>
    /// <returns>Always false.</returns>
    public override bool VerifyIntegrity()
        => false;
}