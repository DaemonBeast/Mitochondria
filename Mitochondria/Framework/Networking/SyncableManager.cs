using System.Diagnostics.CodeAnalysis;
using Mitochondria.Api.Networking;
using Mitochondria.Framework.Utilities;
using Mitochondria.Framework.Utilities.Extensions;
using Mitochondria.Rpcs;
using Reactor.Networking.Rpc;
using Reactor.Utilities;

namespace Mitochondria.Framework.Networking;

public class SyncableManager
{
    public static SyncableManager Instance => Singleton<SyncableManager>.Instance;
    
    public Dictionary<ulong, ISyncable> Syncables { get; }

    public SyncableManager()
    {
        Syncables = new Dictionary<ulong, ISyncable>();
    }

    public void Add(ISyncable syncable)
    {
        var syncableId = syncable.Id.Get64HashCode().ToUInt64();
        if (Syncables.ContainsKey(syncableId))
        {
            Logger<MitochondriaPlugin>.Error($"Attempted to add multiple Syncables with the same ID: {syncable.Id}");

            return;
        }

        Syncables.Add(syncableId, syncable);
    }

    public bool TryGet(ulong id, [NotNullWhen(true)] out ISyncable? syncable)
        => Syncables.TryGetValue(id, out syncable);

    public void Remove(ulong id)
        => Syncables.Remove(id);

    public void Sync(ISyncable syncable)
    {
        if (!Syncables.ContainsValue(syncable))
        {
            Logger<MitochondriaPlugin>.Error("Attempted to sync Syncable not added to SyncableManager");

            return;
        }

        if (!AmongUsClient.Instance ||
            syncable.HostOnly && !AmongUsClient.Instance.AmHost)
        {
            return;
        }

        Rpc<RpcSync>.Instance.Send(new RpcSync.Data(syncable), true);
    }

    public void SyncAll()
    {
        foreach (var syncable in Syncables.Values)
        {
            Sync(syncable);
        }
    }
}