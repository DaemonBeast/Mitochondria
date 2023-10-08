using Mitochondria.Api.Serialization;

namespace Mitochondria.Api.Networking;

public interface ISyncable : ISerializable
{
    public bool HostOnly => false;
}