using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Resource<T> {
    public string id;
    public T prefab;
}

public class ResourceManager : Singleton<ResourceManager> {

    [SerializeField]
    private List<Resource<PredictedProjectile>> _projectileResources;

    private Dictionary<string, PredictedProjectile> _projectiles = new Dictionary<string, PredictedProjectile>();

    [SerializeField]
    private List<Resource<PredictedTelegraph>> _telegraphResources;

    private Dictionary<string, PredictedTelegraph> _telegraphs = new Dictionary<string, PredictedTelegraph>();

    [SerializeField]
    private List<Resource<Buff>> _buffResources;

    private Dictionary<string, Buff> _buffs = new Dictionary<string, Buff>();

    public void Awake() {
        _projectileResources.ForEach(resource => _projectiles.Add(resource.id, resource.prefab));
        _telegraphResources.ForEach(resource => _telegraphs.Add(resource.id, resource.prefab));
        _buffResources.ForEach(resource => _buffs.Add(resource.id, resource.prefab));
    }

    public PredictedProjectile GetProjectile(string id) {
        return _projectiles[id];
    }

    public PredictedTelegraph GetTelegraph(string id) {
        return _telegraphs[id];
    }

    public Buff GetBuff(string id) {
        return _buffs[id];
    }
}