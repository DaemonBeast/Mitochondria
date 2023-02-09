namespace Mitochondria.Api.UI.Flex;

public interface IFlexFactory<in T> : IFlexFactory
{
    public bool Matches(T obj);

    public IFlex Create(T obj);
    
    public delegate IFlex FlexFactoryHandler(T obj);
}

public interface IFlexFactory
{
}