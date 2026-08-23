/*
    Using teh Reader and Writer Lock mechanism 
    - Reader: multiple thead can read without blocking each other, but they will block the writer
    - Writer: only one thread can write at a time, and it will block all the readers
*/

namespace ReaderWriterLock;

public record User(Guid Id, string Name);

public sealed class UserBuilder
{
    private Guid UserId = Guid.NewGuid();
    private string Name = string.Empty;

    public UserBuilder AddName(string name)
    {
        Name = name;
        return this;
    }

    public UserBuilder AddUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }

    public User Build()
    {
        return new User(UserId, Name);
    }
}

public sealed class ThreadSafeCache
{
    // Reader, Writer Lock
    private readonly ReaderWriterLockSlim _lockReadWrite = new ReaderWriterLockSlim();

    // Simulation on cache where multiple threads accessing into
    private Dictionary<string, User> _cache { get; } = new Dictionary<string, User>();

    // Synchronous: Add or Update 1 value
    public void AddOrUpdate(string key, User value, int timeout = 1000)
    {
        try
        {
            // Write thread will wait here
            _lockReadWrite.TryEnterWriteLock(timeout);
            if (!_cache.TryAdd(key, value))
            {
                _cache[key] = value;
            }
        }
        finally
        {
            _lockReadWrite.ExitWriteLock();
        }
    }

    // Synchronous: Get 1 value
    public User? Get(string key)
    {
        _lockReadWrite.EnterReadLock();
        _cache.TryGetValue(key, out User? value);
        _lockReadWrite.ExitReadLock();

        return value;
    }

    // Parallel: Add Everything in parallel
    public void AddRandomMassParallel(int upperBound)
    {
        Parallel.For(0, upperBound, i =>
        {
            UserBuilder userBuilder = new UserBuilder();
            userBuilder.AddName("RandomName" + i);
            var newUser = userBuilder.Build();
            AddOrUpdate(newUser.Id.ToString(), newUser);
        });
    }

    // Parallel: Reading
    public void GetMassParallel()
    {
        // Get All Keys
        // - Since reading from the critical section, capture inside the Read Lock
        _lockReadWrite.EnterReadLock();
        var keys = _cache.Keys;
        _lockReadWrite.ExitReadLock();

        Parallel.ForEach(keys, key =>
        {
            var user = Get(key);
            Console.WriteLine($"Reader {Thread.CurrentThread.ManagedThreadId} -> {user?.Name}");
        });
    }
}

public class Program
{
    private static void Main(string[] args)
    {
        var readWriteScenario = new ThreadSafeCache();

        // Mass Insertion parallel
        readWriteScenario.AddRandomMassParallel(100);

        // Mass Reading parallel


    }
}