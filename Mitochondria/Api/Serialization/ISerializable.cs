using Hazel;

namespace Mitochondria.Api.Serialization;

public interface ISerializable
{
    public string Id { get; }

    public void Serialize(MessageWriter writer);

    public void Deserialize(MessageReader reader);
}