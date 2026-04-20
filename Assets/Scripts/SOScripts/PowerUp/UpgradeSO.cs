using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public abstract class UpgradeSO : ScriptableObject {

    [SerializeField] Sprite icon;
    [SerializeField] string upgradeName;
    [Tooltip("DO NOT Assign, use for checking id"), SerializeField] string upgradeId;
    [TextArea, SerializeField] string description;

    public Sprite GetIcon() => icon;
    public string GetUpgradeName() => upgradeName;

    public string GetUpgradeId() => upgradeId;

    public string GetDescription() => description;

#if UNITY_EDITOR
    public void OnValidate() {

        if ( string.IsNullOrEmpty(upgradeId) ) {
            // Use the internal Unity GUID for this asset file
            string path = AssetDatabase.GetAssetPath(this);
            upgradeId = AssetDatabase.AssetPathToGUID(path);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

    }
#endif

    public virtual IReadOnlyList<UpgradeStatSO.UpgradePair> ApplyStat( StatManager statManager ) {
        return System.Array.Empty<UpgradeStatSO.UpgradePair>();
    }

    public virtual void ApplyBehavior( UpgradeManager upgradeManager ) { }

}

