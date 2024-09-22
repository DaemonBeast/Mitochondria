namespace Mitochondria.Core.Utilities;

public static class Singleton<T>
{
    public static T Instance => _instance ??= (T) Activator.CreateInstance(typeof(T), true)!;

    private static T? _instance;
}
