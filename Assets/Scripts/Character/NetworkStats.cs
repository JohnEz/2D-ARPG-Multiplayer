using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;

public class NetworkStats : NetworkBehaviour {

    [HideInInspector]
    [SyncVar(OnChange = nameof(HandleCurrentHealthChange), WritePermissions = WritePermission.ServerOnly)]
    private int _currentHealth = 100;

    public int CurrentHealth { get { return _currentHealth; } }

    [SyncVar(OnChange = nameof(HandleMaxHealthChange), WritePermissions = WritePermission.ServerOnly)]
    private int _maxHealth = 100;

    public int MaxHealth { get { return _maxHealth; } }

    [SerializeField]
    private float _baseMoveSpeed = 4f;

    public float MoveSpeed { get { return _baseMoveSpeed; } }

    public override void OnStartServer() {
        _currentHealth = MaxHealth;
    }

    public event Action OnHealthDepleted;

    public event Action OnHealthReplenished;

    public event Action OnHealthChanged;

    public event Action<int, bool, bool> OnTakeDamage;

    public event Action<int> OnReceiveHealing;

    private void HandleCurrentHealthChange(int previousValue, int nextValue, bool asServer) {
        if (!asServer && InstanceFinder.IsHost) {
            // this is called for each client and for server, so host would call twice
            return;
        }

        OnHealthChanged?.Invoke();
        HandleHitPointsChanged(previousValue, nextValue);
    }

    private void HandleMaxHealthChange(int previousValue, int nextValue, bool asServer) {
        if (!asServer && InstanceFinder.IsHost) {
            // this is called for each client and for server, so host would call twice
            return;
        }

        OnHealthChanged?.Invoke();
        HandleHitPointsChanged(previousValue, nextValue);
    }

    private void HandleHitPointsChanged(int previousValue, int nextValue) {
        if (previousValue > 0 && nextValue <= 0) {
            // newly reached 0 HP
            OnHealthDepleted?.Invoke();
            //if (InstanceFinder.IsServer) {
            //    NetworkObject.Despawn();
            //}
        } else if (previousValue <= 0 && nextValue > 0) {
            // newly revived
            OnHealthReplenished?.Invoke();
        }
    }

    [Server]
    public void TakeDamageServer(int damage) {
        _currentHealth = Math.Max(0, _currentHealth - damage);
    }

    public void TakeDamage(int damage, bool sourceIsPlayer) {
        TakeDamageServer(damage);

        if (InstanceFinder.IsClient) {
            // TODO damage text needs more thinking as the value is calculated on the server
            // maybe i need an event for being hit and an event for taking numeric damage thats called in an observer? (not great for lag)
            OnTakeDamage.Invoke(damage, false, sourceIsPlayer);
        }
    }

    [Server]
    public void ReceiveHealingServer(int healing) {
        _currentHealth = Math.Min(MaxHealth, _currentHealth + healing);
    }

    public void ReceiveHealing(int healing) {
        ReceiveHealingServer(healing);

        if (InstanceFinder.IsClient) {
            OnReceiveHealing.Invoke(healing);
        }
    }
}