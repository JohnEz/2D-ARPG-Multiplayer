using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;

public class NetworkStats : NetworkBehaviour {

    [HideInInspector]
    [SyncVar(OnChange = nameof(HandleCurrentHealthChange))]
    private int _currentHealth = 100;

    public int CurrentHealth { get { return _currentHealth; } }

    [SyncVar(OnChange = nameof(HandleMaxHealthChange))]
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

    private void HandleCurrentHealthChange(int previousValue, int nextValue, bool isServer) {
        OnHealthChanged?.Invoke();
        HandleHitPointsChanged(previousValue, nextValue);
    }

    private void HandleMaxHealthChange(int previousValue, int nextValue, bool isServer) {
        OnHealthChanged?.Invoke();
        HandleHitPointsChanged(previousValue, nextValue);
    }

    private void HandleHitPointsChanged(int previousValue, int nextValue) {
        if (previousValue > 0 && nextValue <= 0) {
            // newly reached 0 HP
            OnHealthDepleted?.Invoke();
            //if (NetworkManager.Singleton.IsServer) {
            //    NetworkObject.Despawn();
            //}
        } else if (previousValue <= 0 && nextValue > 0) {
            // newly revived
            OnHealthReplenished?.Invoke();
        }
    }

    // TODO Pulls these out into damageable and healable scripts
    public void TakeDamageServer(int damage, bool hitShield) {
        if (!InstanceFinder.IsServer) {
            return;
        }

        _currentHealth = Math.Max(0, _currentHealth - damage);
    }

    public void TakeDamageClient(int damage, bool hitShield, bool sourceIsPlayer) {
        // i need to split combat text out somehow
        OnTakeDamage.Invoke(damage, hitShield, sourceIsPlayer);
    }

    [ServerRpc]
    public void ReceiveHealing(int healing) {
        _currentHealth = Math.Min(MaxHealth, _currentHealth + healing);
        ReceiveHealingClient(healing);
    }

    public void ReceiveHealingClient(int healing) {
        OnReceiveHealing.Invoke(healing);
    }
}