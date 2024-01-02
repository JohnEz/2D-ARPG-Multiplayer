using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityIcon))]
public class AbilityTooltip : TooltipGenerator {
    private AbilityIcon abilityIcon;

    protected override void Awake() {
        base.Awake();

        abilityIcon = GetComponent<AbilityIcon>();
    }

    public override void SetupTooltip() {
        base.SetupTooltip();

        Tooltip.Instance.SetWidth(500);

        Tooltip.Instance.AddTitle(abilityIcon.MyAbility.AbilityName);

        Tooltip.Instance.AddSpacer();

        Tooltip.Instance.AddText(abilityIcon.MyAbility.Description);

        abilityIcon.MyAbility.buffIds.ForEach(buffId => {
            Tooltip.Instance.AddSpacer();
            Tooltip.Instance.AddSpacer();

            Buff buff = ResourceManager.Instance.GetBuff(buffId);
            Tooltip.Instance.AddSubTitle(buff.Name);
            Tooltip.Instance.AddText(buff.Description);
        });

        Tooltip.Instance.AddSpacer();
        Tooltip.Instance.AddSpacer();

        if (abilityIcon.MyAbility.Cooldown > 0) {
            Tooltip.Instance.AddText($"Cooldown: {abilityIcon.MyAbility.Cooldown}s");
        }

        if (abilityIcon.MyAbility.MaxCharges > 1) {
            Tooltip.Instance.AddText($"Charges: {abilityIcon.MyAbility.MaxCharges}");
        }
    }
}