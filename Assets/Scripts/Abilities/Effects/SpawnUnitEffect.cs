using System.Collections;
using UnityEngine;

public class SpawnUnitEffect : AbilityEffect {

    [SerializeField]
    private string _spawnCharacterId;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            CharacterManager.Instance.SpawnCharacter(_spawnCharacterId, _casterController.AimLocation);
        }

        Destroy(gameObject);
    }
}