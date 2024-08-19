namespace CharacterAttributeSystemTest;

// System designed with assumption that game has time frame specified in ticks instead of system time.
// This allows us to be flexible and update our objects with different level of priority.
// For example low prio item updated every 100 ticks, high prio every 10.
// In some degree it emulates elapsed system time, but gives us more flexibility for test and in general.
//
// This mock doesn't actually do any timing / loops etc and just returns fake ticks assigned to it.

/// <summary>
/// The ITimeManager interface provides methods to manage the game time.
/// It enables update of objects based on a specified time frame in ticks.
/// </summary>
public interface ITimeManager
{
    /// <summary>
    /// Calculates the number of elapsed ticks since a specified starting tick.
    /// </summary>
    /// <param name="since">The starting tick value.</param>
    /// <returns>The number of elapsed ticks since the starting tick.</returns>
    int ElapsedTicks(int since);

    /// <summary>
    /// Gets the current tick of the TimeManager.
    /// </summary>
    /// <returns>The current tick.</returns>
    int CurrentTick();
}

/// <summary>
/// Represents a mock implementation of the ITimeManager interface.
/// </summary>
public class TimeManagerMock : ITimeManager
{
    /// <summary>
    /// Represents the current tick value of the game.
    /// </summary>
    private int _currentTick;

    /// <summary>
    /// This mock class represents a time manager in a game that has a time frame specified in ticks.
    /// It allows for updating objects with different levels of priority based on elapsed ticks.
    /// </summary>
    public TimeManagerMock(int currentTick)
    {
        _currentTick = currentTick;
    }

    /// <summary>
    /// Calculates the number of elapsed ticks since a specified tick value.
    /// </summary>
    /// <param name="since">The tick value since which the elapsed ticks will be calculated.</param>
    /// <returns>The number of elapsed ticks since the specified tick value.</returns>
    public int ElapsedTicks(int since)
        => CurrentTick() - since;

    /// <summary>
    /// Returns the current tick value.
    /// </summary>
    /// <returns>The current tick value.</returns>
    public int CurrentTick()
        => _currentTick;

    /// <summary>
    /// Sets the current tick of the TimeManagerMock.
    /// </summary>
    /// <param name="value">The new value for the current tick.</param>
    public void SetCurrentTick(int value)
        => _currentTick = value;

    /// <summary>
    /// Increments the current tick of the TimeManagerMock object by 1.
    /// </summary>
    public void Inc()
    {
        SetCurrentTick(CurrentTick()+1);
    }
}