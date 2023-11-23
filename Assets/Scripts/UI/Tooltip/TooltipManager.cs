using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : Singleton<TooltipManager> {

    [SerializeField]
    private GameObject _tooltipPrefab;

    private Tooltip _tooltip;

    // TODO i want to get this programatically
    [SerializeField]
    private Canvas canvas;

    public void CreateTooltip() {
        if (_tooltip) {
            return;
        }

        _tooltip = Instantiate(_tooltipPrefab, canvas.transform).GetComponent<Tooltip>();
    }
}
