using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "ScriptableObjects/PowerUp")]
public class UpgradeSO : ScriptableObject {



    public enum UpgradeType {
        ExtraBalls, // Add extra balls
        Speed,
        Crit,
        Fire,
        Lightning,
        // Add more effect types as needed
    }


    [SerializeField] string upgradeName;
    [SerializeField] UpgradeType upgradeType;
    [SerializeField] float primaryValue;
    [SerializeField] float secondaryValue;

    public virtual void Apply( BallManager ballManager ) {
        switch ( upgradeType ) { 
            case (UpgradeType.ExtraBalls):
                ballManager.AddBall((int)primaryValue);
                break;
            case (UpgradeType.Crit):
                ballManager.ModifyProperty("CritChance", primaryValue);
                ballManager.ModifyProperty("CritMultiplier", secondaryValue);
                break;
            case UpgradeType.Fire:
                ballManager.ModifyProperty("FireChance", primaryValue);
                break;

            case UpgradeType.Lightning:
                ballManager.ModifyProperty("LightningChance", primaryValue);
                break;

            case UpgradeType.Speed:
                ballManager.ModifyProperty("Speed", primaryValue);
                break;
        }        
    }

}
