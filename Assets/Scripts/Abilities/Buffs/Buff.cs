using UnityEngine;
using FishNet;
using System.Collections.Generic;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "New Buff", menuName = "2d RPG/New Buff")]
public class Buff : ScriptableObject {

    [SerializeField]
    private string _name;

    public string Name {
        get { return _name; }
        set { _name = value; }
    }

    [SerializeField]
    private float _maxDuration;

    public float MaxDuration {
        get { return _maxDuration; }
        set { _maxDuration = value; }
    }

    public float InitialDuration { get; set; }

    [SerializeField]
    private float _interval;

    public float Interval {
        get { return _interval; }
        set { _interval = value; }
    }

    [SerializeField]
    private BuffTickEffect _tickEffectPrefab;

    public float ElapsedTime { get; set; }

    private float _addedTime = 0f;

    public float AddedTime {
        get { return _addedTime; }
        set { SetAddedTime(value); }
    }

    protected NetworkStats targetCharacter;

    private int tickCounter = 0;

    [SerializeField]
    public Color TextColor = Color.white;

    [SerializeField]
    private bool _isAStun;

    public bool IsAStun {
        get { return _isAStun; }
    }

    [SerializeField]
    private bool _isPositive;

    public bool IsPositive {
        get { return _isPositive; }
    }

    [SerializeField]
    private bool _canBeDispelled = true;

    public bool CanBeDispelled {
        get { return _canBeDispelled; }
    }

    private BuffController _caster;

    public BuffController Caster {
        get { return _caster; }
    }

    [SerializeField]
    private AudioClip applySFX;

    [SerializeField]
    private AudioClip expireSFX;

    [SerializeField]
    private GameObject applyVFX;

    private GameObject _applyVFXInstance;

    [SerializeField]
    private GameObject expireVFX;

    [SerializeField]
    private List<StatModifier> _statMods = new List<StatModifier>();

    public List<StatModifier> StatMods { get { return _statMods; } }

    [SerializeField]
    private AbilityEffect _expireEffect;

    private bool _isApplied = false;

    private StatModifier _shieldMod;

    public bool HasShield { get { return _shieldMod != null; } }

    public string Description;

    public virtual void Initailise(BuffController caster, NetworkStats target, float initialDuration, float elapsedTime, float passedTime, float addedTime) {
        InitialDuration = initialDuration > 0 && initialDuration < MaxDuration ? initialDuration : MaxDuration;
        targetCharacter = target;
        ElapsedTime = elapsedTime + passedTime;
        AddedTime = addedTime;
        _caster = caster;

        NetworkStats casterStats = caster.GetComponent<NetworkStats>();

        _statMods.ForEach(statMod => {
            statMod.Initialise((int)casterStats.Power.CurrentValue);
        });

        _shieldMod = _statMods.Find(mod =>
            mod.Stat == StatType.SHIELD && mod.Type == StatModType.Flat
        );

        if (applySFX) {
            AudioManager.Instance.PlaySound(applySFX, targetCharacter.transform.position);
        }

        if (applyVFX) {
            _applyVFXInstance = Instantiate(applyVFX, target.transform);

            VisualEffect applyVFXVisualEffect = _applyVFXInstance.GetComponent<VisualEffect>();

            if (applyVFXVisualEffect != null && applyVFXVisualEffect.HasFloat("Duration")) {
                applyVFXVisualEffect.SetFloat("Duration", InitialDuration);
            }
        }
    }

    public virtual void ApplyEffects() {
        if (!InstanceFinder.IsServer || _isApplied) {
            return;
        }

        targetCharacter.ApplyStatMods(_statMods);
        _isApplied = true;
    }

    public virtual void RemoveEffects() {
        if (!InstanceFinder.IsServer || !_isApplied) {
            return;
        }

        targetCharacter.RemoveStatMods(_statMods);
        _isApplied = false;
    }

    public virtual void UpdateElapsedTime(float deltaTime) {
        ElapsedTime += deltaTime;

        int ticks = Mathf.FloorToInt(ElapsedTime / Interval);

        if (ticks > tickCounter) {
            OnTick();
            tickCounter++;
        }
    }

    public virtual bool HasExpired() {
        return RemainingTime() <= 0 || (HasShield && _shieldMod.Value <= 0);
    }

    public virtual void OnTick() {
        CreateTickEffect();
    }

    private void CreateTickEffect() {
        if (_tickEffectPrefab == null) {
            return;
        }

        BuffTickEffect tickEffect = Instantiate(_tickEffectPrefab, targetCharacter.transform);
        tickEffect.Initialise(_caster.GetComponent<NetworkStats>());
        tickEffect.OnTick(true, targetCharacter);
    }

    public virtual void OnExpire() {
        if (_applyVFXInstance) {
            Destroy(_applyVFXInstance);
        }

        if (expireSFX) {
            AudioManager.Instance.PlaySound(expireSFX, targetCharacter.transform.position);
        }

        if (expireVFX) {
            Instantiate(expireVFX, targetCharacter.transform);
        }

        if (_expireEffect) {
            AbilityEffect expireEffect = Instantiate(_expireEffect);
            //TODO this should probably be the initial caster
            expireEffect.Initialise(targetCharacter);
            expireEffect.OnCastComplete(true);
        }
    }

    public virtual bool ShouldBeOverriden(Buff newBuff) {
        return newBuff.RemainingTime() > RemainingTime();
    }

    public float RemainingTime() {
        return Mathf.Min(InitialDuration - ElapsedTime + AddedTime, MaxDuration);
    }

    public void SetAddedTime(float newAddedTime) {
        float maxAddedTime = MaxDuration - (InitialDuration - ElapsedTime);
        float actualAddedTime = Mathf.Min(newAddedTime, maxAddedTime);

        _addedTime = actualAddedTime;
    }
}