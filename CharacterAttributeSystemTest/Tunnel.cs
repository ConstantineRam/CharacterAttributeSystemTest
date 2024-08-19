using System.Collections.Concurrent;

namespace CharacterAttributeSystemTest;
// simple combo of concurrent queue and semaphore so modifiers will not overwrite each other. 
// Mods has short life span (inside an attribute object) and there is no need for anything fancier. 
public sealed class Tunnel<T> 
{
    
    private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>(); 
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
    
    ~Tunnel ()
    {
        this._semaphore?.Dispose();
    }

    public bool Any()
        => !_queue.IsEmpty;
        
    public bool HasInQueue(in T value)
        =>  value != null && this._queue.Contains(value);
        

    public void Write(in T value)
    {
        if (value == null) return;
        
        _queue.Enqueue(value);
        _semaphore.Release();
    }

    public async Task<T?> ReadASync(CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct).ConfigureAwait(false); 
  
        if (_queue.IsEmpty) return default;
        var  gotResult = _queue.TryDequeue(out var item);
        
        return item;   
    }
    
   
}