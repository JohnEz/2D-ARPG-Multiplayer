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
    private TargetGraphicStyle myStyle;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Sprite _circleSprite;

    [SerializeField]
    private Sprite _arrowSprite;

    [SerializeField]
    private Transform _graphicTransform;

    public void InitialiseSelfTarget(Transform parent, float scale) {
        myStyle = TargetGraphicStyle.SELF;

        _spriteRenderer.sprite = _circleSprite;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        SetScale(scale);
    }

    public void InitialiseFollowMouseTarget(float scale) {
        myStyle = TargetGraphicStyle.FOLLOW_MOUSE;

        _spriteRenderer.sprite = _circleSprite;
        FollowMouse followScript = AddScript(typeof(FollowMouse)) as FollowMouse;

        SetScale(scale);
    }

    public void InitialiseLeapTarget(Transform startTransform, float maxDistance, float scale) {
        myStyle = TargetGraphicStyle.LEAP;
        LeapTarget leapScript = AddScript(typeof(LeapTarget)) as LeapTarget;

        _spriteRenderer.sprite = _circleSprite;

        leapScript.StartTransform = startTransform;
        leapScript.MaxDistance = maxDistance;
        SetScale(scale);
    }

    public void InitialiseArrowTarget(Transform visuals, float maxDistance) {
        myStyle = TargetGraphicStyle.SELF;
        _graphicTransform.localPosition = new Vector3(0f, 8f, 0);
        _spriteRenderer.sprite = _arrowSprite;

        transform.SetParent(visuals, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        SetScale(1f);
    }

    private Component AddScript(System.Type component) {
        return gameObject.AddComponent(component);
    }

    private void SetScale(float scale) {
        transform.localScale = new Vector3(scale, scale, scale);
    }
}