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
    private List<Resource<Projectile>> _projectileResources;

    private Dictionary<string, Projectile> _projectiles = new Dictionary<string, Projectile>();

    [SerializeField]
    private List<Resource<PredictedTelegraph>> _telegraphResources;

    private Dictionary<string, PredictedTelegraph> _telegraphs = new Dictionary<string, PredictedTelegraph>();

    [SerializeField]
    private List<Resource<PredictedSlash>> _slashResources;

    private Dictionary<string, PredictedSlash> _slashes = new Dictionary<string, PredictedSlash>();

    [SerializeField]
    private List<Resource<Buff>> _buffResources;

    private Dictionary<string, Buff> _buffs = new Dictionary<string, Buff>();

    [SerializeField]
    private List<Resource<PersistentAOE>> _persistentAOEResources;

    private Dictionary<string, PersistentAOE> _persistentAOEs = new Dictionary<string, PersistentAOE>();

    public void Awake() {
        _projectileResources.ForEach(resource => _projectiles.Add(resource.id, resource.prefab));
        _telegraphResources.ForEach(resource => _telegraphs.Add(resource.id, resource.prefab));
        _buffResources.ForEach(resource => _buffs.Add(resource.id, resource.prefab));
        _slashResources.ForEach(resource => _slashes.Add(resource.id, resource.prefab));
        _persistentAOEResources.ForEach(resource => _persistentAOEs.Add(resource.id, resource.prefab));
    }

    public Projectile GetProjectile(string id) {
        return _projectiles[id];
    }

    public PredictedTelegraph GetTelegraph(string id) {
        return _telegraphs[id];
    }

    public PredictedSlash GetSlash(string id) {
        return _slashes[id];
    }

    public Buff GetBuff(string id) {
        return _buffs[id];
    }

    public PersistentAOE GetPersistentAOE(string id) {
        return _persistentAOEs[id];
    }
}