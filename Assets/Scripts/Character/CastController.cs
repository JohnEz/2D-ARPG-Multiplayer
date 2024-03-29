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
    private BuffController _buffController;

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
        _buffController = GetComponent<BuffController>();
    }

    private void OnEnable() {
        _stateController.OnDeath += HandleCharacterDeath;
        _buffController.OnStunApplied += HandleCharacterStunApplied;
    }

    private void OnDisable() {
        _stateController.OnDeath -= HandleCharacterDeath;
        _buffController.OnStunApplied -= HandleCharacterStunApplied;
    }

    private void HandleCharacterStunApplied() {
        CancelCast();
    }

    private void HandleCharacterDeath() {
        CancelCast();
    }

    public void Cast(int abilityId, bool isUtilityAbility) {
        CastAbility(abilityId, isUtilityAbility, 0);
        NotifyCast(abilityId, isUtilityAbility, base.TimeManager.Tick);
    }

    private void CastAbility(int abilityId, bool isUtilityAbility, float passedTime) {
        castingAbility = isUtilityAbility ?
            _abilitiesController.GetUtilityAbility(abilityId) :
            _abilitiesController.GetAbility(abilityId);

        castTime = castingAbility.CastTime;

        CreateAbilityEffect(castingAbility);

        OnCastStart.Invoke(castTime, passedTime);

        StartCoroutine(Casting());
    }

    public void NotifyCast(int abilityId, bool isUtilityAbility, uint tick) {
        if (InstanceFinder.IsServer) {
            ObserverCast(abilityId, isUtilityAbility, tick);
        } else {
            ServerCast(abilityId, isUtilityAbility, tick);
        }
    }

    [ServerRpc]
    private void ServerCast(int abilityId, bool isUtilityAbility, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        CastAbility(abilityId, isUtilityAbility, passedTime);
        ObserverCast(abilityId, isUtilityAbility, tick);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void ObserverCast(int abilityId, bool isUtilityAbility, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        CastAbility(abilityId, isUtilityAbility, passedTime);
    }

    private void CreateAbilityEffect(Ability abilityToCreate) {
        // TODO these can potentially be moved to "Casting()"
        GameObject createdEffect = Instantiate(abilityToCreate.AbilityEffectPrefab);
        castingAbilityEffect = createdEffect.GetComponent<AbilityEffect>();
        castingAbilityEffect.Initialise(GetComponent<NetworkStats>());
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
                    LeapAbility leapAbility = castingAbilityEffect.GetComponent<LeapAbility>();
                    targetGraphicController.InitialiseLeapTarget(transform, leapAbility.MinDistance, leapAbility.MaxDistance, targetGraphic.scale);
                    break;

                    case TargetGraphicStyle.ARROW:
                    targetGraphicController.InitialiseArrowTarget(visuals, targetGraphic.scale);
                    break;
                }

                targetGraphics.Add(targetGraphicObject);
            });
        }

        GameObject castVFX = null;

        if (castingAbility.PrecastVFX) {
            castVFX = Instantiate(castingAbility.PrecastVFX, visuals.transform);
            castVFX.GetComponent<VisualEffect>().SetFloat("Duration", castTime);
        }

        if (castingAbility.PrecastSFX) {
            AudioManager.Instance.PlaySound(castingAbility.PrecastSFX, visuals.transform);
        }

        yield return new WaitUntil(() => castRequest == false);

        // TODO this is grim and is trying to avoid overwriting stunned

        targetGraphics
            .ForEach(targetObject => Destroy(targetObject));

        if (castVFX) {
            Destroy(castVFX);
        }

        if (castSuccess) {
            castingAbilityEffect.OnCastComplete(IsOwner || (IsServer && OwnerId == -1));
            castingAbility.OnCast();

            // global cast cooldown
            yield return new WaitForSeconds(0.1f);
        }

        if (_stateController.IsCasting()) {
            _stateController.State = CharacterState.Idle;
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