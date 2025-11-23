using NUnit.Framework.Internal.Commands;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStatSO : UpgradeSO {

    [SerializeField] Dictionary<UpgradeType,float> KeyValueMap;

    public override void ApplyStat( StatManager statManager ) {
        foreach ( var kvp in KeyValueMap ) {

            statManager.ModifyStat(kvp.Key, kvp.Value);

            //TOOD : add processing for upgrade
            if ( kvp.Key == UpgradeType.CritChance ) {
                
                
            }

        }


    }

    public Dictionary<UpgradeType, float> GetKeyValueMap() {
        return KeyValueMap;
    }

}
