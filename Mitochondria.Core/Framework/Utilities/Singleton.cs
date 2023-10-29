namespace Mitochondria.Core.Framework.Utilities;

public static class Singleton<T>
    where T : class
{
    public static T Instance => _instance ??= Reset();

    public static bool Exists => _instance != null;

    private static T? _instance;

    public static void Use(T instance)
    {
        _instance = instance;
    }

    public static void Remove()
    {
        _instance = null;
    }

    public static T Reset()
    {
        return _instance = Activator.CreateInstance(typeof(T), true) as T ??
                           throw new Exception($"Cannot create singleton of {nameof(T)}");
    }
}