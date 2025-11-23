using System.Collections.Generic;
using UnityEngine;

public class UpgradeBehaviorSO : UpgradeSO {

    [SerializeField] List<IBehavior> behaviors;

    public override void ApplyBehavior( UpgradeManager upgradeManager ) {
        foreach ( var behavior in behaviors ) {
            behavior.Apply(upgradeManager);

        }
    }
}