using NUnit.Framework.Internal.Commands;
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

    public override void ApplyStat( StatManager statManager, UpgradeManager upgradeManager ) {
        foreach ( var pair in KeyValueMap ) {

            if ( pair.Type == UpgradeType.ExtraBalls ) {
                upgradeManager.AddBall((int)pair.Value);
                continue;
            }

            statManager.ModifyStat(pair.Type, pair.Value);

            //TOOD : add processing for upgrade
            switch ( pair.Type ) {
                case UpgradeType.ExplosionChance:
                    upgradeManager.ApplyProcess(new ExplosionProcess());
                    break;
                case UpgradeType.CritChance:
                    upgradeManager.ApplyProcess(new CritProcess());
                    break;
                default:
                    break;
            }

        }

    }   

    //TODO: Add Process to upgrade manger when applying upgrade
}


