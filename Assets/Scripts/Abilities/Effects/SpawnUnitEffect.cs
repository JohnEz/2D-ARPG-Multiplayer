using System.Collections;
using UnityEngine;

public class SpawnUnitEffect : AbilityEffect {

    [SerializeField]
    private string _spawnCharacterId;

    [SerializeField]
    private int _spawnCount = 1;

    public override void OnCastComplete(bool isOwner) {
        base.OnCastComplete(isOwner);

        if (isOwner) {
            for (int i = 0; i < _spawnCount; i++) {
                // TODO add some randomness to location
                CharacterManager.Instance.SpawnCharacter(_spawnCharacterId, _casterController.AimLocation);
            }
        }

        Destroy(gameObject);
    }
}