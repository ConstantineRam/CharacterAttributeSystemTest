using CharacterAttributeSystemTest;

namespace Test_Suit;

public class ExpirationTokenTest
{
    private TimeManagerMock _timeManagerMock;
    
    [SetUp]
    public void SetUp()
    {
        _timeManagerMock = new TimeManagerMock(10);
    }

    [Test]
    public void ExpirationTokenReturnsTrueIfExpired()
    {
        var modifier = new AttributeModifierChangeValue(_timeManagerMock, null, 10, 100, nameof(AttributeModifierChangeValue), "");
        var et = new ExpirationToken(modifier, _timeManagerMock);
        _timeManagerMock.SetCurrentTick(120);
        
        Assert.That(et.IsExpired(), Is.EqualTo(true));
    }
    
    [Test]
    public void ExpirationTokenReturnsFalseIfNotExpired()
    {
        var modifier = new AttributeModifierChangeValue(_timeManagerMock, null,10, 100, nameof(AttributeModifierChangeValue), "");
        var et = new ExpirationToken(modifier, _timeManagerMock);
        _timeManagerMock.SetCurrentTick(11);
        
        Assert.That(et.IsExpired(), Is.EqualTo(false));
    }
    
    [Test]
    public void ExpirationToken_InfiniteNeverExpires()
    {
        var modifier = new AttributeModifierChangeValue(_timeManagerMock, null,10, Modifiers.Infinite, nameof(AttributeModifierChangeValue), "");
        var et = new ExpirationToken(modifier, _timeManagerMock);
        _timeManagerMock.SetCurrentTick(110);
        
        Assert.That(et.IsExpired(), Is.EqualTo(false));
    }
}