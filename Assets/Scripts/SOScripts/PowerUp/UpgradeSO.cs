using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "ScriptableObjects/PowerUp")]
public abstract class UpgradeSO : ScriptableObject {

    [SerializeField] string upgradeName;
    [Tooltip("DO NOT Assign, use for checking id"), SerializeField] string upgradeId;
    [SerializeField] UpgradeType upgradeType;
    [SerializeField] float primaryValue;
    [SerializeField] float secondaryValue;
    [SerializeField] string description;


    public string GetUpgradeName() => upgradeName;
    public UpgradeType GetUpgradeType() => upgradeType;
    public float GetPrimaryValue() => primaryValue;
    public float GetSecondaryValue() => secondaryValue;
    public string GetDescription() => description;

#if UNITY_EDITOR
    public void OnValidate() {

        if ( string.IsNullOrEmpty(upgradeId) ) {
            upgradeId = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            Debug.Log($"[Auto-ID] Generated new ID for {name}: {upgradeId}");
        }

    }
#endif

    public virtual void Apply( BallManager ballManager ) {

        //Debug.Log("upgrading");

        //switch ( upgradeType ) {
        //    case (UpgradeType.ExtraBalls):
        //        ballManager.RequestExtraBall((int)primaryValue);
        //        break;
        //    case (UpgradeType.Crit):
        //        ballManager.ModifyProperty("CritChance", primaryValue);
        //        ballManager.ModifyProperty("CritMultiplier", secondaryValue);
        //        break;
        //    case UpgradeType.Fire:
        //        ballManager.ModifyProperty("FireChance", primaryValue);
        //        break;

        //    case UpgradeType.Lightning:
        //        ballManager.ModifyProperty("LightningChance", primaryValue);
        //        break;

        //    case UpgradeType.Speed:
        //        ballManager.ModifyProperty("Speed", primaryValue);
        //        break;
        //}
    }

    //public UpgradeData ToData() => new UpgradeData(upgradeName, upgradeType, primaryValue, secondaryValue, description);

    // Create runtime SO from UpgradeData
    //public static UpgradeSO FromData( UpgradeData data ) {
    //    var so = CreateInstance<UpgradeSO>();
    //    so.name = $"Runtime_{data.upgradeName}";
    //    so.upgradeName = data.upgradeName;
    //    so.upgradeType = data.upgradeType;
    //    so.primaryValue = data.primaryValue;
    //    so.secondaryValue = data.secondaryValue;
    //    so.description = data.description;
    //    return so;
    //}

    //public static UpgradeSO FromData( CharacterUpgradeData data ) {
    //    var so = CreateInstance<UpgradeSO>();
    //    so.name = $"Runtime_{data.upgradeName}";
    //    so.upgradeName = data.upgradeName;
    //    so.upgradeType = data.upgradeType;
    //    so.primaryValue = data.primaryValue;
    //    so.secondaryValue = data.secondaryValue;
    //    so.description = data.description;
    //    return so;
    //}

}

public enum UpgradeType {
    ExtraBalls, // Add extra balls
    Speed,
    Crit,
    Fire,
    Lightning,
    // Add more effect types as needed
}
