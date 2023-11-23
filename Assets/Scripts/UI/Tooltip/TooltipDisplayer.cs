using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class TooltipDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    protected RectTransform _rect;

    protected virtual void Awake() {
        _rect = GetComponent<RectTransform>();
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        SetupTooltip();

        Tooltip.Instance.Show();
    }

    public virtual void SetupTooltip() {
        Tooltip.Instance.SetAnchor(_rect);
        Tooltip.Instance.SetWidth(300);
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        Tooltip.Instance.Hide();
    }
}
