using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionBarManager : Singleton<ActionBarManager> {
    private AbilitiesController _myAbilities;

    [SerializeField]
    private List<AbilityIcon> _abilityIcons;

    private void Start() {
        foreach (AbilityIcon abilityIcon in _abilityIcons) {
            abilityIcon.gameObject.SetActive(false);
        }
    }

    public void SetCharacter(AbilitiesController abilities) {
        _myAbilities = abilities;

        int index = 0;
        foreach (Ability ability in _myAbilities.GetAbilities()) {
            _abilityIcons[index].gameObject.SetActive(true);
            _abilityIcons[index].SetAbility(ability);
            index++;
        }
    }
}