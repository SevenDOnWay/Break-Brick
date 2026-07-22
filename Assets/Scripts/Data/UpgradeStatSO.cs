using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "ScriptableObjects/StatUpgrade")]
public class UpgradeStatSO : UpgradeSO {

    [System.Serializable]
    public struct UpgradePair {
        [SerializeField] UpgradeType type;
        [SerializeField] float value;
        [SerializeField] BallType ballType;

        public UpgradeType Type => type;
        public float Value => value;
        public BallType BallType => ballType;
    }

    [SerializeField] List<UpgradePair> KeyValueMap;

    public IReadOnlyList<UpgradePair> GetKeyValueMap() => KeyValueMap;
}


