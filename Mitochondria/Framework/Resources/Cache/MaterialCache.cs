using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Mitochondria.Framework.Utilities;
using UnityEngine;

namespace Mitochondria.Framework.Resources.Cache;

public class MaterialCache
{
    public static MaterialCache Instance => Singleton<MaterialCache>.Instance;

    public ImmutableArray<Material?> Materials => _materials.Immutable;

    private readonly List<Material?> _actualMaterials;
    private readonly ImmutableArrayWrapper<Material?> _materials;

    private int _nextId;

    private MaterialCache()
    {
        _actualMaterials = new List<Material?>();
        _materials = new ImmutableArrayWrapper<Material?>(_actualMaterials);

        _nextId = 0;
    }

    public bool TryGet(int id, [NotNullWhen(true)] out Material? material)
    {
        material = Get(id);
        return material != null;
    }

    public Material? Get(int id)
    {
        if (id < 0 || id >= _materials.Length)
        {
            return null;
        }

        return _actualMaterials[id];
    }

    public void Set(int id, Material material)
    {
        if (id < 0 || id >= _actualMaterials.Count)
        {
            throw new ArgumentException($"{nameof(MaterialCache)} does not contain the ID {id}");
        }

        _actualMaterials[id] = material;
        _materials.MarkDirty();
    }

    public int Add(Material material)
    {
        _materials.Add(material);
        return _nextId++;
    }

    public int Reserve()
    {
        _materials.Add(null);
        return _nextId++;
    }
}