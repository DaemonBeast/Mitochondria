using Hazel;
using Mitochondria.Core.Api.Networking;
using Mitochondria.Core.Framework.Networking;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;

namespace Mitochondria.Core.Rpcs;

[RegisterCustomRpc((uint) CustomRpcCalls.Sync)]
public class RpcSync : PlayerCustomRpc<MitochondriaPlugin, RpcSync.Data>
{
    public override RpcLocalHandling LocalHandling => RpcLocalHandling.None;

    private MessageReader? _reader;

    public RpcSync(MitochondriaPlugin plugin, uint id) : base(plugin, id)
    {
    }

    public override void Write(MessageWriter writer, Data data)
    {
        writer.Write(data.Syncable!.Id.Get64HashCode());
        
        var innerWriter = MessageWriter.Get();
        data.Syncable.Serialize(innerWriter);
        writer.WriteBytesAndSize(innerWriter.ToByteArray(false));
        innerWriter.Recycle();
    }

    public override Data Read(MessageReader reader)
    {
        var syncableId = reader.ReadBytes(8).ToUInt64();
        
        if (!SyncableManager.Instance.TryGet(syncableId, out var syncable))
        {
            Logger<MitochondriaPlugin>.Warning($"Received Sync RPC for unknown Syncable {syncableId}");
            
            return new Data(null);
        }

        _reader = MessageReader.Get(reader.ReadBytesAndSize());

        return new Data(syncable);
    }

    public override void Handle(PlayerControl innerNetObject, Data data)
    {
        if (data.Syncable == null)
        {
            return;
        }

        if (data.Syncable.HostOnly && !innerNetObject.IsHost())
        {
            Logger<MitochondriaPlugin>.Warning(
                $"Non-host player {innerNetObject.Data.PlayerName} attempted to sync host-only Syncable");
            
            _reader!.Recycle();

            return;
        }
        
        data.Syncable.Deserialize(_reader!);
        _reader!.Recycle();
    }

    public readonly record struct Data(ISyncable? Syncable);
}