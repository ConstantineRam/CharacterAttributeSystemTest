namespace CharacterAttributeSystemTest;

public class AttributeModifiersStorage
{

    private readonly HashSet<IModifier> _storage = [];

    /// <summary>
    /// Gets or sets a value indicating whether the object has been initialized.
    /// </summary>
    /// <remarks>
    /// The <see cref="IsInitialized"/> property specifies whether the object has been fully initialized.
    /// It returns <c>true</c> if the initialization process has completed and <c>false</c> otherwise.
    /// The property is initially set to <c>false</c> and is set to <c>true</c> when the <see cref="Initialize(ITimeManager, CancellationToken)"/> method completes successfully.
    /// </remarks>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the initialization process is in progress.
    /// </summary>
    /// <remarks>
    /// The <see cref="InitializationInProgress"/> property is used to determine whether the initialization process of the <see cref="AttributeModifiersStorage"/> class is currently in progress.
    /// This property is set to <c>true</c> when the initialization process starts, and it is set to <c>false</c> when the initialization process completes or is aborted.
    /// </remarks>
    private bool InitializationInProgress { get; set; }
    
    private readonly Random _randomized = new Random();
    
    private ITimeManager _timeManager;

    public AttributeModifiersStorage(ITimeManager timeManager)
    {
        _timeManager = timeManager;
    }

    /// <summary>
    /// Initializes the AttributeModifiersStorage.
    /// </summary>
    /// <param name="timeManager">The time manager.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task Initialize( CancellationToken ct)
    {
        if (IsInitialized || InitializationInProgress) return;
        InitializationInProgress = true;
       
        await LoadCollection<AttributeModifierPurge>(ct);
        await LoadCollection<AttributeModifierChangeValue>(ct);
        
        Console.WriteLine($"Loaded modifiers: {_storage.Count}");
        IsInitialized = true;
        InitializationInProgress = false;
    }

    /// <summary>
    /// Loads a collection of attribute modifiers of type T.
    /// </summary>
    /// <typeparam name="T">The type of attribute modifiers to load.</typeparam>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A Task representing the asynchronous operation.</returns
    private async Task LoadCollection<T>(CancellationToken ct) where T : AttributeModifierParent
    {
        foreach (var modifier in await AttributeModifierLoader.LoadForType<T>(_timeManager, ct))
        {
            if (ct.IsCancellationRequested) return;
            if (!_storage.Add(modifier))
            {
                Console.WriteLine($"Failed to add {modifier.Name} to storage. Probably duplicate id."); 
            }
        }  
    }

    /// <summary>
    /// Retrieves a random attribute modifier from the storage.
    /// </summary>
    /// <returns>
    /// The randomly selected attribute modifier. If the storage is not initialized, a null object of type AttributeModifierParent with the injected time manager will be returned.
    /// </returns>
    public IModifier GetRandom()
        => !IsInitialized ? AttributeModifierParent.CreateNullObject(_timeManager) : _storage.ElementAt(_randomized.Next(0, _storage.Count));
    

    

}