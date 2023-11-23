using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class TooltipGenerator : TooltipDisplayer {

    [SerializeField]
    private string _tooltipText;

    public override void SetupTooltip() {
        base.SetupTooltip();

        Tooltip.Instance.AddTitle(_tooltipText);
    }
}
