using UnityEngine;

public interface IBehavior {

    public UpgradeBehaviourType Type { get; }

    public void Apply( UpgradeManager upgradeManager );

}