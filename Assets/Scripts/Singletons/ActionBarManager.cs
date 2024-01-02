using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionBarManager : Singleton<ActionBarManager> {
    private AbilitiesController _myAbilities;

    [SerializeField]
    private List<AbilityIcon> _abilityIcons;

    [SerializeField]
    private List<AbilityIcon> _utilityAbilityIcons;

    private void Start() {
        _abilityIcons.ForEach(abilityIcon => {
            abilityIcon.gameObject.SetActive(false);
        });

        _utilityAbilityIcons.ForEach(abilityIcon => {
            abilityIcon.gameObject.SetActive(false);
        });
    }

    public void SetCharacter(AbilitiesController abilities) {
        _myAbilities = abilities;

        int abilityIndex = 0;
        _myAbilities.AbilityList.ForEach(ability => {
            _abilityIcons[abilityIndex].gameObject.SetActive(true);
            _abilityIcons[abilityIndex].SetAbility(ability);
            abilityIndex++;
        });

        int utilityAbilityIndex = 0;
        _myAbilities.UtilityAbilityList.ForEach(utilityAbility => {
            _utilityAbilityIcons[utilityAbilityIndex].gameObject.SetActive(true);
            _utilityAbilityIcons[utilityAbilityIndex].SetAbility(utilityAbility);
            utilityAbilityIndex++;
        });
    }
}