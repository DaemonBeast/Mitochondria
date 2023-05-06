namespace Mitochondria.Api.Overrides;

public interface IOverride<TOverride> : IOverride
    where TOverride : IOverride<TOverride>
{
    public TOverride Override(TOverride otherOverride);
}

public interface IOverride
{
    public int Priority => 0;
}