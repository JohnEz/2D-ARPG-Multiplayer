using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FishNet;
using FishNet.Object;

public class CastController : NetworkBehaviour {
    private CharacterStateController _stateController;
    private AbilitiesController _abilitiesController;

    public Ability castingAbility;
    public AbilityEffect castingAbilityEffect;
    private bool castRequest, castSuccess;
    private float castTime = 0;

    private GameObject castVFX;
    public UnityEvent<float, float> OnCastStart = new UnityEvent<float, float>();
    public UnityEvent OnCastFail = new UnityEvent();
    public UnityEvent OnCastSuccess = new UnityEvent();

    // how much lag we compensate for, probably should be global
    private const float MAX_PASSED_TIME = 0.3f;

    private void Awake() {
        _stateController = GetComponent<CharacterStateController>();
        _abilitiesController = GetComponent<AbilitiesController>();
    }

    public void Cast(int abilityId) {
        CreateAbilityEffect(abilityId);

        castTime = castingAbility.castTime;
        OnCastStart.Invoke(castTime, 0);

        if (InstanceFinder.IsClient) {
            ServerCastStart(abilityId, base.TimeManager.Tick);
        } else if (InstanceFinder.IsServer) {
            ObserverCastStart(abilityId, base.TimeManager.Tick);
        }

        StartCoroutine(Casting());
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerCastStart(int abilityId, uint tick) {
        ObserverCastStart(abilityId, tick);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ObserverCastStart(int abilityId, uint tick) {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        castingAbility = _abilitiesController.GetAbility(abilityId);

        OnCastStart.Invoke(castingAbility.castTime, passedTime);
    }

    private void CreateAbilityEffect(int abilityId) {
        castingAbility = _abilitiesController.GetAbility(abilityId);

        // TODO these can potentially be removed
        GameObject createdEffect = Instantiate(castingAbility.abilityEffectPrefab);
        castingAbilityEffect = createdEffect.GetComponent<AbilityEffect>();
        castingAbilityEffect.Initialise(GetComponent<CharacterController>());
    }

    private IEnumerator Casting() {
        _stateController.State = CharacterState.Casting;

        RequestCast();

        List<GameObject> targetGraphics = new List<GameObject>();

        //if (IsOwner && castingAbility.targetGraphics.Count > 0) {
        //    castingAbility.targetGraphics.ForEach(targetGraphic => {
        //        // TODO move this out into targetGraphicController
        //        GameObject targetGraphicObject = Instantiate(targetGraphic.prefab);
        //        TargetGraphicController targetGraphicController = targetGraphicObject.GetComponent<TargetGraphicController>();

        //        switch (targetGraphic.myStyle) {
        //            case TargetGraphicStyle.SELF:
        //                targetGraphicController.InitialiseSelfTarget(transform, targetGraphic.scale);
        //                break;

        //            case TargetGraphicStyle.FOLLOW_MOUSE:
        //                targetGraphicController.InitialiseFollowMouseTarget(targetGraphic.scale);
        //                break;

        //            case TargetGraphicStyle.LEAP:
        //                targetGraphicController.InitialiseLeapTarget(transform, 10f, targetGraphic.scale);
        //                break;
        //        }

        //        targetGraphics.Add(targetGraphicObject);
        //    });
        //}

        //if (castingAbility.castVFX) {
        //    castVFX = Instantiate(castingAbility.castVFX, visuals.transform);
        //    castVFX.GetComponent<VisualEffect>().SetFloat("Duration", castingAbility.castTime);
        //}

        //if (castingAbility.castSFX) {
        //    AudioManager.Instance.PlaySound(castingAbility.castSFX, visuals.transform);
        //}

        yield return new WaitUntil(() => castRequest == false);

        // TODO this is grim
        if (!_stateController.IsStunned()) {
            _stateController.State = CharacterState.Idle;
        }

        targetGraphics
            .ForEach(targetObject => Destroy(targetObject));

        //if (castVFX) {
        //    Destroy(castVFX);
        //}

        if (castSuccess) {
            // TODO i should probably move casting ability effect into the casting ability?
            castingAbility.OnCast();
            castingAbilityEffect.OnCastComplete();
        }
    }

    public void ManualCancelCast() {
        CancelCast();
        //CancelCastServerRpc(true);
    }

    private void CancelCast() {
        if (!_stateController.IsCasting()) {
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