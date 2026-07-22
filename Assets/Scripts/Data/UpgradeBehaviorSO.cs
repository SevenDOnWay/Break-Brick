using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviorUpgrade", menuName = "ScriptableObjects/BehaviorUpgrade")]
public class UpgradeBehaviorSO : UpgradeSO {

    [SerializeField] List<UpgradeBehaviourType> behaviorTypes;

    public IReadOnlyList<UpgradeBehaviourType> GetBehaviorTypes() => behaviorTypes;
}
