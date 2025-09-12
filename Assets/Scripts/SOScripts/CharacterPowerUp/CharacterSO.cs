using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "CharacterSO", menuName = "ScriptableObjects/CharacterSO")]
public class CharacterSO : ScriptableObject {

    public enum UpgradeType {
        ExtraBalls, // Add extra balls
        Crit,
        Fire,
        Lightning,
        // Add more effect types as needed
    }

    public string characterName;
    public Sprite icon;
    public string description;

    public UpgradeSO upgrade;

    public void Apply(BallManager ballManager) {
       upgrade.Apply(ballManager);
    }


}
