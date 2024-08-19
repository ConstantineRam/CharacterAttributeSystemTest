using CharacterAttributeSystemTest;

using var cts = new CancellationTokenSource();

var timeManager = new TimeManagerMock(0);
var attributeSet = new CharacterAttributeSet(timeManager);
var modStorage = new AttributeModifiersStorage(timeManager);
await modStorage.Initialize(cts.Token);

const int testIterations = 100;


for (var i = 0; i < testIterations; i++)
{
    if (cts.IsCancellationRequested) break;
    Console.WriteLine($" Tick: {timeManager.CurrentTick()}");
    Console.WriteLine(attributeSet.ToDebugString());
    var newMod = modStorage.GetRandom();
    if (newMod is INullObject {IsNullObject: true})
    {
        Console.WriteLine($"Error; Mod storage provided null object.");    
    }
    Console.WriteLine($"{newMod.Name} was selected.");
    await attributeSet.ApplyModifier(newMod, cts.Token);
    await attributeSet.Update(cts.Token);
    Console.WriteLine();
    
    timeManager.Inc();
}
