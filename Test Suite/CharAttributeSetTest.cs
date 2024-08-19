using CharacterAttributeSystemTest;
namespace Test_Suit;

public class CharAttributeSetTest
{   
    private TimeManagerMock _timeManagerMock;
    
    [SetUp]
    public void SetUp()
    {
        _timeManagerMock = new TimeManagerMock(10);
    }
    
    [Test]
    public void AttributeSet_Created()
    {
        var attSet = new CharacterAttributeSet(_timeManagerMock);
        
        Assert.That(attSet, Is.Not.Null);
    }
    
    [Test]
    public void AttributeSet_ReturnsCollectionWithAttributes()
    {
        var attSet = new CharacterAttributeSet(_timeManagerMock);
        
        var collection = attSet.ToImmutableList();
        Assert.That(collection != null && collection.Any());
    }   
    
}