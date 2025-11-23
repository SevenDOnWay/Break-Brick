using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows.Speech;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "ScriptableObjects/PowerUp")]
public abstract class UpgradeSO : ScriptableObject {

    [SerializeField] string upgradeName;
    [Tooltip("DO NOT Assign, use for checking id"), SerializeField] string upgradeId;

    [TextArea, SerializeField] string description;

    public string GetUpgradeName() => upgradeName;

    public string GetUpgradeId() => upgradeId;

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

    public virtual void ApplyStat( StatManager statManager ) {}

    public virtual void ApplyBehavior( UpgradeManager upgradeManager ) { }

}

