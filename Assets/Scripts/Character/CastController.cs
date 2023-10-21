using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FishNet;
using FishNet.Object;
using UnityEngine.VFX;

public class CastController : NetworkBehaviour {
    private CharacterStateController _stateController;
    private AbilitiesController _abilitiesController;

    public Ability castingAbility;
    public AbilityEffect castingAbilityEffect;
    private bool castRequest, castSuccess;
    private float castTime = 0;

    public UnityEvent<float, float> OnCastStart = new UnityEvent<float, float>();
    public UnityEvent OnCastFail = new UnityEvent();
    public UnityEvent OnCastSuccess = new UnityEvent();

    [SerializeField]
    private Transform visuals; // so vfx can follow facing direction

    // how much lag we compensate for, probably should be global
    private const float MAX_PASSED_TIME = 0.3f;

    private void Awake() {
        _stateController = GetComponent<CharacterStateController>();
        _abilitiesController = GetComponent<AbilitiesController>();
    }

    private void OnEnable() {
        _stateController.OnDeath += HandleCharacterDeath;
    }

    private void OnDisable() {
        _stateController.OnDeath -= HandleCharacterDeath;
    }

    private void HandleCharacterDeath() {
        CancelCast();
    }

    public void Cast(int abilityId) {
        CastAbility(abilityId, 0);
        NotifyCast(abilityId, base.TimeManager.Tick);
    }

    private void CastAbility(int abilityId, float passedTime) {
        castingAbility = _abilitiesController.GetAbility(abilityId);
        castTime = castingAbility.CastTime;

        CreateAbilityEffect(abilityId);

        OnCastStart.Invoke(castTime, passedTime);

        StartCoroutine(Casting());
    }

    public void NotifyCast(int abilityId, uint tick) {
        if (InstanceFinder.IsServer) {
            ObserverCast(abilityId, tick);
        } else {
            ServerCast(abilityId, tick);
        }
    }

    [ServerRpc]
    private void ServerCast(int abilityId, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        CastAbility(abilityId, passedTime);
        ObserverCast(abilityId, tick);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void ObserverCast(int abilityId, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        CastAbility(abilityId, passedTime);
    }

    private void CreateAbilityEffect(int abilityId) {
        if (abilityId >= _abilitiesController.GetAbilities().Count) {
            return;
        }

        Ability abilityToCreate = _abilitiesController.GetAbility(abilityId);

        // TODO these can potentially be moved to "Casting()"
        GameObject createdEffect = Instantiate(abilityToCreate.AbilityEffectPrefab);
        castingAbilityEffect = createdEffect.GetComponent<AbilityEffect>();
        castingAbilityEffect.Initialise(GetComponent<CharacterController>());
    }

    private IEnumerator Casting() {
        _stateController.State = CharacterState.Casting;

        RequestCast();

        List<GameObject> targetGraphics = new List<GameObject>();

        if (IsOwner && castingAbility.TargetGraphics.Count > 0) {
            castingAbility.TargetGraphics.ForEach(targetGraphic => {
                // TODO move this out into targetGraphicController
                GameObject targetGraphicObject = Instantiate(targetGraphic.prefab);
                TargetGraphicController targetGraphicController = targetGraphicObject.GetComponent<TargetGraphicController>();

                switch (targetGraphic.myStyle) {
                    case TargetGraphicStyle.SELF:
                        targetGraphicController.InitialiseSelfTarget(transform, targetGraphic.scale);
                        break;

                    case TargetGraphicStyle.FOLLOW_MOUSE:
                        targetGraphicController.InitialiseFollowMouseTarget(targetGraphic.scale);
                        break;

                    case TargetGraphicStyle.LEAP:
                        targetGraphicController.InitialiseLeapTarget(transform, 10f, targetGraphic.scale);
                        break;

                    case TargetGraphicStyle.ARROW:
                        targetGraphicController.InitialiseArrowTarget(visuals, targetGraphic.scale);
                        break;
                }

                targetGraphics.Add(targetGraphicObject);
            });
        }

        GameObject castVFX = null;

        if (castingAbility.CastVFX) {
            castVFX = Instantiate(castingAbility.CastVFX, visuals.transform);
            castVFX.GetComponent<VisualEffect>().SetFloat("Duration", castTime);
        }

        if (castingAbility.CastSFX) {
            AudioManager.Instance.PlaySound(castingAbility.CastSFX, visuals.transform);
        }

        yield return new WaitUntil(() => castRequest == false);

        // TODO this is grim
        if (!_stateController.IsStunned()) {
            _stateController.State = CharacterState.Idle;
        }

        targetGraphics
            .ForEach(targetObject => Destroy(targetObject));

        if (castVFX) {
            Destroy(castVFX);
        }

        if (castSuccess) {
            castingAbility.OnCast();
            castingAbilityEffect.OnCastComplete(IsOwner || (IsServer && OwnerId == -1));
        }
    }

    public void ManualCancelCast() {
        CancelCast();
        //CancelCastServerRpc(true);
    }

    private void CancelCast() {
        if (!castRequest) {
            return;
        }

        CastFail();
    }

    private void RequestCast() {
        castRequest = true;
        castSuccess = false;
        Invoke("CastSuccess", castTime);
    }

    private void CastSuccess() {
        castRequest = false;
        castSuccess = true;
        OnCastSuccess.Invoke();
    }

    public void CastFail() {
        castRequest = false;
        castSuccess = false;
        CancelInvoke("CastSuccess");
        OnCastFail.Invoke();
    }
}