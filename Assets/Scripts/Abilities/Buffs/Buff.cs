using UnityEngine;
using FishNet;

[CreateAssetMenu(fileName = "New Buff", menuName = "2d RPG/New Buff")]
public class Buff : ScriptableObject {

    [SerializeField]
    private string _name;

    public string Name {
        get { return _name; }
        set { _name = value; }
    }

    [SerializeField]
    private float _duration;

    public float Duration {
        get { return _duration; }
        set { _duration = value; }
    }

    [SerializeField]
    private float _interval;

    public float Interval {
        get { return _interval; }
        set { _interval = value; }
    }

    public float ElapsedTime { get; set; }

    private float _addedTime = 0f;

    public float AddedTime {
        get { return _addedTime; }
        set { SetAddedTime(value); }
    }

    protected NetworkStats targetCharacter;

    private int tickCounter = 0;

    [SerializeField]
    private bool _isAStun;

    public bool IsAStun {
        get { return _isAStun; }
        set { _isAStun = value; }
    }

    [SerializeField]
    private AudioClip applySFX;

    [SerializeField]
    private AudioClip expireSFX;

    [SerializeField]
    private GameObject applyVFX;

    [SerializeField]
    private GameObject expireVFX;

    public virtual void Initailise(NetworkStats target, float elapsedTime, float passedTime, float addedTime) {
        targetCharacter = target;
        ElapsedTime = elapsedTime + passedTime;
        AddedTime = addedTime;

        if (applySFX) {
            AudioManager.Instance.PlaySound(applySFX, targetCharacter.transform.position);
        }

        if (applyVFX) {
            Instantiate(applyVFX, target.transform);
        }
    }

    public virtual void UpdateElapsedTime(float deltaTime) {
        ElapsedTime += deltaTime;

        if (!InstanceFinder.IsServer) {
            return;
        }

        int ticks = Mathf.FloorToInt(ElapsedTime / Interval);

        if (ticks > tickCounter) {
            OnTick();
            tickCounter++;
        }
    }

    public virtual bool HasExpired() {
        return RemainingTime() <= 0;
    }

    public virtual void OnTick() {
    }

    public virtual void OnExpire() {
        if (expireSFX) {
            AudioManager.Instance.PlaySound(expireSFX, targetCharacter.transform.position);
        }

        if (expireVFX) {
            Instantiate(expireVFX, targetCharacter.transform);
        }
    }

    public virtual bool ShouldBeOverriden(Buff newBuff) {
        return newBuff.Duration > RemainingTime();
    }

    public float RemainingTime() {
        return Mathf.Min(Duration - ElapsedTime + AddedTime, Duration);
    }

    public void SetAddedTime(float newAddedTime) {
        float maxAddedTime = Duration - (Duration - ElapsedTime);
        float actualAddedTime = Mathf.Min(newAddedTime, maxAddedTime);

        _addedTime = actualAddedTime;
    }

    public void AddTime(float timeToAdd) {
        float actualAddedTime = Mathf.Min(timeToAdd, Duration - RemainingTime());

        _addedTime += actualAddedTime;
    }
}