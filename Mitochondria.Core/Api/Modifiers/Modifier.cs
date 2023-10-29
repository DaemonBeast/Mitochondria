namespace Mitochondria.Core.Api.Modifiers;

public abstract class Modifier : IModifier
{
    public int Priority => 0;

    public virtual void Initialize()
    {
    }

    public virtual void OnUpdate()
    {
    }
    
    public virtual void Dispose()
    {
    }
    
    public int CompareTo(IModifier? other)
        => Priority - other!.Priority;

    public int CompareTo(object? obj)
    {
        if (obj is not IModifier modifier)
        {
            throw new ArgumentException($"Object to compare must be of type {nameof(IModifier)}");
        }

        return CompareTo(modifier);
    }
}

public interface IModifier : IComparable<IModifier>, IComparable, IDisposable
{
    public int Priority => 0;

    public void Initialize();

    public void OnUpdate();
}