using System.Diagnostics.CodeAnalysis;
using Mitochondria.Core.Api.Networking;
using Mitochondria.Core.Framework.Utilities;
using Mitochondria.Core.Framework.Utilities.Extensions;
using Mitochondria.Core.Rpcs;
using Reactor.Networking.Rpc;
using Reactor.Utilities;

namespace Mitochondria.Core.Framework.Networking;

public class SyncableManager
{
    public static SyncableManager Instance => Singleton<SyncableManager>.Instance;

    public Dictionary<ulong, WeakReference<ISyncable>> Syncables { get; }

    public SyncableManager()
    {
        Syncables = new Dictionary<ulong, WeakReference<ISyncable>>();
    }

    public void Add(ISyncable syncable)
    {
        var syncableId = syncable.Id.Get64HashCode().ToUInt64();
        if (Syncables.TryGetValue(syncableId, out var syncableRef) && syncableRef.TryGetTarget(out _))
        {
            Logger<MitochondriaPlugin>.Warning($"Overriding Syncable with the same ID: {syncable.Id}");
        }

        Syncables[syncableId] = new WeakReference<ISyncable>(syncable);
    }

    public bool TryGet(ulong id, [NotNullWhen(true)] out ISyncable? syncable)
    {
        if (Syncables.TryGetValue(id, out var syncableRef))
        {
            if (syncableRef.TryGetTarget(out syncable))
            {
                return true;
            }

            Syncables.Remove(id);
        }

        syncable = null;
        return false;
    }

    public void Remove(ulong id)
        => Syncables.Remove(id);

    public void Sync(ISyncable syncable)
    {
        if (!Syncables.Values.Any(r => r.TryGetTarget(out var s) && s == syncable))
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
        foreach (var syncableRef in Syncables.Values)
        {
            if (syncableRef.TryGetTarget(out var syncable))
            {
                Sync(syncable);
            }
        }
    }
}