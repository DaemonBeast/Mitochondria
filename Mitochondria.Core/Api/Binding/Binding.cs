namespace Mitochondria.Core.Api.Binding;

public abstract class Binding<T1, T2> : IBinding<T1, T2>
    where T1 : class where T2 : class
{
    public T1 Obj { get; internal set; } = default!;

    public T2 Other { get; internal set; } = default!;

    public object BoxedObj
    {
        get => Obj;
        set => Obj = (T1) value;
    }

    public object BoxedOther
    {
        get => Other;
        set => Other = (T2) value;
    }

    public virtual void Init()
    {
    }

    public virtual void BindEvents()
    {
    }

    public virtual void Update()
    {
    }

    public bool IsInvalid()
        => Obj == null! || Other == null!;

    public virtual void Dispose()
        => GC.SuppressFinalize(this);
}

public interface IBinding<T1, T2> : IBinding
    where T1 : class where T2 : class
{
    public T1? Obj { get; }

    public T2? Other { get; }
}

public interface IBinding : IDisposable
{
    public object BoxedObj { get; internal set; }

    public object BoxedOther { get; internal set; }

    public void Init();

    public void BindEvents();

    public void Update();

    public bool IsInvalid();
}