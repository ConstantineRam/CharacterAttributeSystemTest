namespace CharacterAttributeSystemTest;

public class ExpirationToken : INullObject
{
    private readonly IModifier _modifier;
    private int _tickStart;
    private readonly ITimeManager _timeManager;
    private bool _markedForForcedExpiration = false;

    public ExpirationToken(IModifier modifier, ITimeManager timeManager)
    {
        _modifier = modifier;
        _timeManager = timeManager;
        _tickStart = _timeManager.CurrentTick();
    }

    /// <summary>
    /// Calculates the remaining ticks until the expiration of the modifier.
    /// </summary>
    /// <returns>The number of ticks remaining until the modifier expires.</returns>
    public int RemainingTicks()
        => _modifier.TicksToExpire - _timeManager.ElapsedTicks(_tickStart);

    /// <summary>
    /// Checks if the current ExpirationToken is expired.
    /// </summary>
    /// <returns>
    /// Returns a boolean value indicating whether the ExpirationToken is expired or not.
    /// </returns>
    public bool IsExpired()
        => _markedForForcedExpiration || (!_modifier.IsInfinite() && _timeManager.ElapsedTicks(_tickStart) >= _modifier.TicksToExpire);

    /// <summary>
    /// Retrieves the modifier associated with the ExpirationToken.
    /// </summary>
    /// <returns>The modifier associated with the ExpirationToken.</returns>
    public IModifier GetModifier() => _modifier;

    /// <summary>
    /// Resets the expiration time of the token.
    /// </summary>
    /// <remarks>
    /// This method is used to reset the tick start time of the token to the current tick.
    /// <para>
    /// The tick start time is used to calculate the remaining ticks and check if the token has expired.
    /// </para>
    /// </remarks>
    public void Reset()
    {
        _tickStart = _timeManager.CurrentTick();
    }

    /// <summary>
    /// Marks the expiration token for forced expiration.
    /// </summary>
    /// <remarks>
    /// When the expiration token is marked for forced expiration, it will be considered expired regardless of its remaining ticks or expiration conditions.
    /// </remarks>
    public void ForceExpiration()
        => _markedForForcedExpiration = true;

    #region NullObject

    /// <summary>
    /// Determines if an object is a null object.
    /// </summary>
    public bool IsNullObject { get; protected init; } = false;

    /// <summary>
    /// Creates a null object instance of ExpirationToken.
    /// This null object is used in cases where an actual ExpirationToken cannot be created due to exceptions.
    /// </summary>
    /// <param name="timeManager">The time manager instance used for tracking ticks.</param>
    /// <returns>A null object instance of ExpirationToken.</returns>
    public static ExpirationToken CreateNullObject(ITimeManager timeManager)
        => new ExpirationTokenNullObject(timeManager);
    #endregion

    /// <summary>
    /// Prints out the debug information of the ExpirationToken.
    /// </summary>
    /// <returns>A string containing the debug information.</returns>
    public string Debug_PrintOut()
    {
        return $" infinite: {_modifier.IsInfinite()}, Current tick: {_timeManager.CurrentTick()}, Elapsed Since start: {_timeManager.ElapsedTicks(_tickStart)}, TimeToExpire {_modifier.TicksToExpire} ";
    }
    
    
}