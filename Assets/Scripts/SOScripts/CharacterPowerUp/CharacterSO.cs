using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "CharacterSO", menuName = "ScriptableObjects/CharacterSO")]
public class CharacterSO : ScriptableObject {

    [SerializeField] string characterName;
    [Tooltip("DO NOT Assign, use for checking id"), SerializeField] string characterId;
    [SerializeField] Sprite icon; //MAYBE: change to video
    [TextArea, SerializeField] string description;

    [Header("UpgradeSO")]
    [SerializeField] UpgradeStatSO UpgradeStatSO;
    [SerializeField] UpgradeBehaviorSO UpgradeBehaviorSO;

    public string GetCharacterName() => characterName;
    public string GetCharacterId() => characterId;
    public Sprite GetIcon() => icon;
    public string GetDescription() => description;


#if UNITY_EDITOR
    public void OnValidate() {

        if ( string.IsNullOrEmpty(characterId) ) {
            characterId = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            Debug.Log($"[Auto-ID] Generated new ID for {name}: {characterId}");
        }

    }
#endif

    public void Apply(StatManager statManager, UpgradeManager upgradeManager) {
        UpgradeStatSO?.ApplyStat(statManager, upgradeManager);
        UpgradeBehaviorSO?.ApplyBehavior(upgradeManager);
    }

    //public CharacterUpgradeData ToData() => new CharacterUpgradeData(this);

    //public CharacterSO FromData(CharacterUpgradeData data) {
    //    var so = CreateInstance<CharacterSO>();
    //    so.name = $"Runtime_{data.characterName}";
    //    so.characterName = data.characterName;
    //    so.icon = null; // optional, handle sprite restore separately
    //    so.upgrade = data.ToRuntimeSO();
    //    return so;
    //}


}
