using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour {

    [SerializeField]
    private List<Ability> _abilityPrefabList;

    private List<Ability> _instantiatedAbilityList;

    private void Awake() {
        _instantiatedAbilityList = new List<Ability>();

        _abilityPrefabList.ForEach((prefab) => {
            Ability instantiatedAbility = Instantiate(prefab);
            _instantiatedAbilityList.Add(instantiatedAbility);
        });
    }

    public List<Ability> GetAbilities() {
        return _instantiatedAbilityList;
    }

    public Ability GetAbility(int abilityId) {
        if (abilityId >= _instantiatedAbilityList.Count) {
            return null;
        }

        return _instantiatedAbilityList[abilityId];
    }
}