using Hazel;

namespace Mitochondria.Api.Networking;

public interface ISerializable
{
    public void Serialize(MessageWriter writer);

    public void Deserialize(MessageReader reader);
}