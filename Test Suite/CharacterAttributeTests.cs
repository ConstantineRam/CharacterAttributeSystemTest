using CharacterAttributeSystemTest;
namespace Test_Suit;

public class CharacterAttributeTests
{
    private TimeManagerMock _timeManagerMock;
    
    [SetUp]
    public void SetUp()
    {
        _timeManagerMock = new TimeManagerMock(10);
    }
    

    [Test]
    public void CharacterAttribute_Created()
    {
        var charAttribute = new CharAttrMock(_timeManagerMock);
        Assert.That(charAttribute, Is.Not.Null);
    }
    
    [Test]
    public void CharacterAttribute_ReturnsAssignedValue()
    {
        var charAttribute = new CharAttrMock(_timeManagerMock,99);
        Assert.That(charAttribute.GetCurrentValue(), Is.EqualTo(99));
    }

    [Test]
    public async Task CharacterAttribute_ReceivesModifier()
    {
        var charAttribute = new CharAttrMock(_timeManagerMock);
        var addValue = new AttributeModifierChangeValue(_timeManagerMock, null,1, 100, nameof(AttributeModifierChangeValue), "");
        try
        {
            await charAttribute.Accept(addValue, CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Assert.Fail();
            
        }
        
        Assert.Pass();
    }

    [Test]
    [TestCase(30, 12 ,42)]
    public async Task CharacterAttribute_ModiferAppliedToValue(int startingValue, int addedValue, int expectedResult)
    {
        
        var charAttribute = new CharAttrMock(_timeManagerMock,startingValue);
        var addValue = new AttributeModifierChangeValue(_timeManagerMock, null, addedValue, 100, nameof(AttributeModifierChangeValue), "");

        await charAttribute.Accept(addValue, CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);

        Assert.That( charAttribute.GetCurrentValue() == expectedResult);
    }
    
    [Test]
    public async Task CharacterAttribute_ModiferAppliedToValueThreeTimes()
    {
        
        var charAttribute = new CharAttrMock(new TimeManagerMock(10),1);
        var addValue0 = new AttributeModifierChangeValue(_timeManagerMock, null,5, 100, nameof(AttributeModifierChangeValue), "");
        var removeValue = new AttributeModifierChangeValue(_timeManagerMock, null, -3, 100, nameof(AttributeModifierChangeValue), "");
        var addValue1 = new AttributeModifierChangeValue(_timeManagerMock, null,4, 100, nameof(AttributeModifierChangeValue), "");
        await charAttribute.Accept(addValue0, CancellationToken.None);
        await charAttribute.Accept(removeValue, CancellationToken.None);
        await charAttribute.Accept(addValue1, CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);

        Assert.That( charAttribute.GetCurrentValue(), Is.EqualTo(7));
    }   
    
    [Test]
    public async Task CharacterAttribute_ModiferRemovedWhenExpires()
    {

        var charAttribute = new CharAttrMock(_timeManagerMock,1);
        var addValue = new AttributeModifierChangeValue(_timeManagerMock, null,5, 100, nameof(AttributeModifierChangeValue), "");

        await charAttribute.Accept(addValue, CancellationToken.None);
        _timeManagerMock.SetCurrentTick(9999);
        await charAttribute.Update(CancellationToken.None);

        Assert.That(charAttribute.HasModifier(addValue));
    }   
    
    [Test]
    public async Task CharacterAttribute_AttributeReturnsToOldValuesWhenModifierExpires()
    {

        var charAttribute = new CharAttrMock(_timeManagerMock,10);
        var addValue0 = new AttributeModifierChangeValue(_timeManagerMock, null,5, 50, nameof(AttributeModifierChangeValue), "" );
        var addValue1 = new AttributeModifierChangeValue(_timeManagerMock, null,12, 100, nameof(AttributeModifierChangeValue), "");
        await charAttribute.Accept(addValue0, CancellationToken.None);
        await charAttribute.Accept(addValue1, CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);
        
        _timeManagerMock.SetCurrentTick(80);
        await charAttribute.Update(CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);
        
        Assert.That(charAttribute.GetCurrentValue(), Is.EqualTo(22));
    }  
    [Test]
    public async Task CharacterAttribute_AttemptToApplyModifierToAttributeWithWrongTagShouldFail()
    {

        var charAttribute = new CharAttrMock(_timeManagerMock,10);

        var modifierWithEldritchTag = new AttributeModifierChangeValue(_timeManagerMock, new HashSet<string>(){CharacterAttributeTags.Eldritch},100, 50, "Eldritch Tag", "" );
   
        await charAttribute.Accept(modifierWithEldritchTag, CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);
        
        Assert.That(charAttribute.GetCurrentValue(), Is.EqualTo(10));
    }
    
 
    [Test]
    public async Task CharacterAttribute_AttemptToApplyModifierToAttributeWithProperTagShouldSucceeds()
    {

        var charAttribute = new CharAttrMock(_timeManagerMock,10);

         var modifierWithPhusicalTag = new AttributeModifierChangeValue(_timeManagerMock, new HashSet<string>(){CharacterAttributeTags.Physical},100, 50, "Eldritch Tag", "" );
   
        await charAttribute.Accept(modifierWithPhusicalTag, CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);

        
    Assert.That(charAttribute.GetCurrentValue(), Is.EqualTo(110));
}
    
    [Test]
    public void CharacterAttribute_ReturnsImmutableStructWithData()
    {
        var charAttribute = new CharAttrMock(_timeManagerMock);
        var data = charAttribute.ToImmutableStruct();
        Assert.That(charAttribute, Is.Not.Null);
    }

    [Test] public async Task CharacterAttribute_PreventsAddingExtraModifierIfRulesProhibitIt()
    {
        var charAttribute = new CharAttrMock(_timeManagerMock, 10);
        var modifierThatProhibitsSame = new AttributeModifierChangeValue(_timeManagerMock, new HashSet<string>(){CharacterAttributeTags.All, AttributeModifierRulesTags.IgnoreIfExists},10, 50, "Eldritch Tag", "" );
        await charAttribute.Accept(modifierThatProhibitsSame, CancellationToken.None);
        await charAttribute.Accept(modifierThatProhibitsSame, CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);
        
        Assert.That(charAttribute.GetCurrentValue(), Is.EqualTo(20));
    }   
    
    [Test] public async Task CharacterAttribute_ResetsModifierIfRulesRequirIt()
    {
        var charAttribute = new CharAttrMock(_timeManagerMock, 10);
        var modifierWithReset = new AttributeModifierChangeValue(_timeManagerMock, new HashSet<string>(){CharacterAttributeTags.All, AttributeModifierRulesTags.ResetIfExists},10, 30, "Eldritch Tag", "" );
        await charAttribute.Accept(modifierWithReset, CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);
        
        _timeManagerMock.SetCurrentTick(15);
        var shouldBe25 = charAttribute.GetTokenForModifier(modifierWithReset).RemainingTicks();       
        await charAttribute.Accept(modifierWithReset, CancellationToken.None);
        await charAttribute.Update(CancellationToken.None);
        var shouldBe30 = charAttribute.GetTokenForModifier(modifierWithReset).RemainingTicks(); 
        Assert.That(shouldBe25 == 25 && shouldBe30 == 30);
    }       
}