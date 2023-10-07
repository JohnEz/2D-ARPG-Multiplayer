using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour {
    public List<Ability> AbilityList;

    public Ability GetAbility(int abilityId) {
        if (abilityId >= AbilityList.Count) {
            return null;
        }

        return AbilityList[abilityId];
    }
}