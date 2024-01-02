using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour {

    [SerializeField]
    private List<Ability> _abilityPrefabList;

    private List<Ability> _instantiatedAbilityList;

    public List<Ability> AbilityList {
        get { return _instantiatedAbilityList; }
    }

    [SerializeField]
    private List<Ability> _utilityAbilityPrefabList;

    private List<Ability> _instantiatedUtilityAbilityList;

    public List<Ability> UtilityAbilityList {
        get { return _instantiatedUtilityAbilityList; }
    }

    private void Awake() {
        _instantiatedAbilityList = new List<Ability>();
        _instantiatedUtilityAbilityList = new List<Ability>();

        _abilityPrefabList.ForEach((prefab) => {
            Ability instantiatedAbility = Instantiate(prefab);
            _instantiatedAbilityList.Add(instantiatedAbility);
        });

        _utilityAbilityPrefabList.ForEach((prefab) => {
            Ability instantiatedAbility = Instantiate(prefab);
            _instantiatedUtilityAbilityList.Add(instantiatedAbility);
        });
    }

    public Ability GetAbility(int abilityId) {
        if (abilityId >= _instantiatedAbilityList.Count) {
            return null;
        }

        return _instantiatedAbilityList[abilityId];
    }

    public Ability GetUtilityAbility(int abilityId) {
        if (abilityId >= _instantiatedUtilityAbilityList.Count) {
            return null;
        }

        return _instantiatedUtilityAbilityList[abilityId];
    }

    private void Update() {
        _instantiatedAbilityList.ForEach(ability => {
            ability.Update();
        });

        _instantiatedUtilityAbilityList.ForEach(ability => {
            ability.Update();
        });
    }
}