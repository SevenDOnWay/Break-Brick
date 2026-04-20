using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "ScriptableObjects/StatUpgrade")]
public class UpgradeStatSO : UpgradeSO {

    [System.Serializable]
    public struct UpgradePair {
        [SerializeField] UpgradeType type;
        [SerializeField] float value;

        public UpgradeType Type => type;
        public float Value => value;
    }

    [SerializeField] List<UpgradePair> KeyValueMap;

    public IReadOnlyList<UpgradePair> GetKeyValueMap() => KeyValueMap;

    public override IReadOnlyList<UpgradePair> ApplyStat( StatManager statManager ) {
        foreach ( var pair in KeyValueMap ) {
            if ( pair.Type == UpgradeType.ExtraBalls ) {
                continue;
            }

            statManager.ModifyStat(pair.Type, pair.Value);
        }

        return KeyValueMap;
    }
}


