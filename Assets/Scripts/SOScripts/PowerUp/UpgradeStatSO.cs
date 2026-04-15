using FMOD;
using NUnit.Framework.Internal.Commands;
using System;
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

    public override void ApplyStat( StatManager statManager, ProcessFactory processFactory, UpgradeManager upgradeManager ) {
        foreach ( var pair in KeyValueMap ) {

            if ( pair.Type == UpgradeType.ExtraBalls ) {
                upgradeManager.AddBall((int)pair.Value);
                continue;
            }

            statManager.ModifyStat(pair.Type, pair.Value);

            //TOOD : add processing for upgrade
            var process = processFactory.CreateProcess(pair.Type);
            if ( process != null ) {
                upgradeManager.ApplyProcess((Process)process);
            }


        }

    }
}


