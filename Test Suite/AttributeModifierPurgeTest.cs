namespace Test_Suit;
using CharacterAttributeSystemTest;


public class AttributeModifierPurgeTest
{
    private TimeManagerMock _timeManagerMock;
    [SetUp]
    public void SetUp()
    {
        _timeManagerMock = new TimeManagerMock(10);
    }
    private const string BuffString = """
                                      {
                                        "$type": "ChangeValue",
                                        "appliedValue": 5,
                                        "Id": "f1cda830-dac2-4d43-b320-83b7fb91a5e6",
                                        "name": "Blessing Of Cat",
                                        "description": "The Best Creature Gave You Grace",
                                        "ticksToExpire": 0,
                                        "tags": [
                                          "All",
                                          "Buff"
                                        ]
                                      }
                                      """;  
    private const string CurseString = """
                                      {
                                        "$type": "ChangeValue",
                                        "appliedValue": 5,
                                        "Id": "a499fcc9-d3da-4a8e-87ca-6dfc6a8299aa",
                                        "name": "Evil Curse",
                                        "description": "Evil Curse",
                                        "ticksToExpire": 0,
                                        "tags": [
                                          "All",
                                          "Curse"
                                        ]
                                      }
                                      """;    
    private const string BuffPurge = """
                                       {
                                         "$type": "Purge",
                                         "Id": "a0901449-d6e6-4fe5-b34d-c9caf0bfed49",
                                         "name": "Remove Buffs",
                                         "description": "",
                                         "ticksToExpire": 0,
                                         "tags": [
                                           "Buff"
                                         ]
                                       }
                                       """; 
    private const string CursePurge = """
                                       {
                                         "$type": "Purge",
                                         "Id": "de967b3d-cfc1-4a90-9473-d1a718f3decd",
                                         "name": "Remove Curse",
                                         "description": "",
                                         "ticksToExpire": 0,
                                         "tags": [
                                           "Curse"
                                         ]
                                       }
                                       """; 
    [Test]
    public async Task AttributeModifierPurge_RemovesModifiersWithTagsItHas()
    {
      var charAttribute = new CharAttrMock(_timeManagerMock, 10);
      var buff = AttributeModifierParent.FromString(BuffString, _timeManagerMock);
      await charAttribute.Accept(buff, CancellationToken.None);
      await charAttribute.Update(CancellationToken.None);
      
      var buffPurge = AttributeModifierParent.FromString(BuffPurge, _timeManagerMock);
      await charAttribute.Accept(buffPurge, CancellationToken.None);
      await charAttribute.Update(CancellationToken.None);
      
      Assert.That(!charAttribute.HasModifier(buff));
    }
    
    [Test]
    public async Task AttributeModifierPurge_DoesNotRemoveModifiersWithTagsItHasNot()
    {
      var charAttribute = new CharAttrMock(_timeManagerMock, 10);
      var buff = AttributeModifierParent.FromString(BuffString, _timeManagerMock);
      await charAttribute.Accept(buff, CancellationToken.None);
      await charAttribute.Update(CancellationToken.None);
      
      var cursePurge = AttributeModifierParent.FromString(CursePurge, _timeManagerMock);
      await charAttribute.Accept(cursePurge, CancellationToken.None);
      await charAttribute.Update(CancellationToken.None);
      
      Assert.That(charAttribute.HasModifier(buff));
    }
}