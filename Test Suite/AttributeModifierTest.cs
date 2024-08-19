using CharacterAttributeSystemTest;


namespace Test_Suit;

public class AttributeModifierTest
{
    private TimeManagerMock _timeManagerMock;
    private const string JsonString = """
                              {
                                "$type": "ChangeValue",
                                "appliedValue": 5,
                                "Id": "9f7bdccb-fdb0-47dc-9de1-8f3c80dbdb7c",
                                "name": "Blessing Of Cat",
                                "description": "The Best Creature Gave You Grace",
                                "ticksToExpire": 10,
                                "tags": [
                                  "All",
                                  "Buff"
                                ]
                              }
                              """;    
    [SetUp]
    public void SetUp()
    {
        _timeManagerMock = new TimeManagerMock(10);
    }
    
    [Test]
    public void AttributeModifer_Create()
    {
        var attMod = new AttributeModifierChangeValue(new TimeManagerMock(1), null, 10, 200, nameof(AttributeModifierChangeValue), "");
    }

    [Test]
    public void AttributeModifier_AttemptToCreateWrongModifierReturnsNullObject()
    {
        const string jsonString = """
                                  {
                                    "$type": "ThisTypeDoesntExist",
                                    "appliedValue": -1,
                                    "Id": "this id is so wrong",
                                    "name": "I am error",
                                    "description": "Worst modifier possible",
                                    "ticksToExpire": 999
                                  }
                                  """;

        var brokenModifier = AttributeModifierParent.FromString(jsonString, _timeManagerMock);
        Assert.That(brokenModifier.IsNullObject);
    }

    [Test]
    public void AttributeModifier_CreatedFromJsonString()
    {


        var atrMod = AttributeModifierParent.FromString(JsonString, _timeManagerMock);
        if (!atrMod.IsNullObject)
        {
            Assert.That(atrMod.Serialize(), Is.EqualTo(JsonString));
            return;
        }
        Assert.Fail();
    }

    [Test]
    public void AttributeModifier_EquatingTwoDifferentInstancesWithSameIdReturnsTrue()
    {
        var mod0 = AttributeModifierParent.FromString(JsonString, _timeManagerMock);
        var mod1 = AttributeModifierParent.FromString(JsonString, _timeManagerMock);
        
        Assert.That(mod0, Is.EqualTo(mod1));
    }
}