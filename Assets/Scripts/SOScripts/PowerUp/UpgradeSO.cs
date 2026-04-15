using UnityEditor;
using UnityEngine;


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
            upgradeId = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }

    }
#endif

    public virtual void ApplyStat( StatManager statManager, ProcessFactory processFactory, UpgradeManager upgradeManager ) {}

    public virtual void ApplyBehavior( UpgradeManager upgradeManager ) { }

}

