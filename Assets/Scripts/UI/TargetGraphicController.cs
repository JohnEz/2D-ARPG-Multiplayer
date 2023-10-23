using UnityEngine;
using System.Collections;
using System;

public enum TargetGraphicStyle {
    FOLLOW_MOUSE,
    LEAP,
    SELF,
    ARROW
}

[Serializable]
public struct TargetGraphic {

    [SerializeField]
    public GameObject prefab;

    [SerializeField]
    public float scale;

    [SerializeField]
    public TargetGraphicStyle myStyle;
}

public class TargetGraphicController : MonoBehaviour {
    private const float ARROW_BODY_SIZE = 0.5f;
    private const float ARROW_HEAD_SIZE = 1f;
    private const float DISTANCE_TO_UNIT_SCALE = 0.5f;

    private TargetGraphicStyle myStyle;

    [SerializeField]
    private Transform _graphicTransform;

    [SerializeField]
    private Transform _arrowHeadTransform;

    [SerializeField]
    private Transform _arrowBodyTransform;

    public void InitialiseSelfTarget(Transform parent, float scale) {
        myStyle = TargetGraphicStyle.SELF;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        SetScale(scale);
    }

    public void InitialiseFollowMouseTarget(float scale) {
        myStyle = TargetGraphicStyle.FOLLOW_MOUSE;

        FollowMouse followScript = AddScript(typeof(FollowMouse)) as FollowMouse;

        SetScale(scale);
    }

    public void InitialiseLeapTarget(Transform startTransform, float minDistance, float maxDistance, float scale) {
        myStyle = TargetGraphicStyle.LEAP;
        LeapTarget leapScript = AddScript(typeof(LeapTarget)) as LeapTarget;

        leapScript.StartTransform = startTransform;
        leapScript.MinDistance = minDistance;
        leapScript.MaxDistance = maxDistance;
        SetScale(scale);
    }

    public void InitialiseArrowTarget(Transform visuals, float maxDistance) {
        myStyle = TargetGraphicStyle.SELF;
        _graphicTransform.localPosition = new Vector3(0f, 0f, 0);

        transform.SetParent(visuals, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        SetArrowLength(maxDistance);
    }

    private Component AddScript(System.Type component) {
        return gameObject.AddComponent(component);
    }

    private void SetScale(float scale) {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void SetArrowLength(float length) {
        float unitLength = length * DISTANCE_TO_UNIT_SCALE;

        float requiredHeadDistance = Mathf.Max(unitLength - ARROW_HEAD_SIZE, 0);

        _arrowHeadTransform.localPosition = new Vector3(requiredHeadDistance + (ARROW_HEAD_SIZE / 2), 0, 0);

        float bodyScale = requiredHeadDistance / ARROW_BODY_SIZE;

        _arrowBodyTransform.localScale = new Vector3(bodyScale, 1f, 1f);
        _arrowBodyTransform.localPosition = new Vector3(bodyScale * (1 - ARROW_BODY_SIZE) / 2, 0, 0);
    }
}