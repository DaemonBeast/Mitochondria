namespace Mitochondria.Api.Networking;

public interface ISyncable : ISerializable
{
    public string Id { get; }

    public bool HostOnly => false;
}