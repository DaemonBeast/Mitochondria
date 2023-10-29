using Mitochondria.Core.Api.Serialization;

namespace Mitochondria.Core.Api.Networking;

public interface ISyncable : ISerializable
{
    public bool HostOnly => false;
}