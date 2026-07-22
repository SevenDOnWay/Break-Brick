using UnityEngine;

[CreateAssetMenu(fileName = "MagnetBehavior", menuName = "ScriptableObjects/Behaviors/MagnetBehavior")]
public class MagnetBehavior : ScriptableObject, IBehavior {

    public UpgradeBehaviourType Type => UpgradeBehaviourType.Magnet;

    //TODO: Add magnet behavior here
    public void Apply( IUpgradeContext context ) {
        context?.SetBehaviorActive(Type);
    }
}
