using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviorUpgrade", menuName = "ScriptableObjects/BehaviorUpgrade")]
public class UpgradeBehaviorSO : UpgradeSO {

    [SerializeField] List<IBehavior> behaviors;

    public override void ApplyBehavior( UpgradeManager upgradeManager ) {
        foreach ( var behavior in behaviors ) {
            behavior.Apply(upgradeManager);

        }
    }
}