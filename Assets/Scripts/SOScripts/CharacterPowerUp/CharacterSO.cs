using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "CharacterSO", menuName = "ScriptableObjects/CharacterSO")]
public class CharacterSO : ScriptableObject {


    [SerializeField] private string characterName;
    [SerializeField] private Sprite icon;
    [SerializeField] private UpgradeSO upgrade;

    public string GetCharacterName() => characterName;
    public Sprite GetIcon() => icon;
    public UpgradeSO GetUpgrade() => upgrade;

    public void Apply(BallManager ballManager) {
       upgrade.Apply(ballManager);
    }

    public CharacterUpgradeData ToData() => new CharacterUpgradeData(this);

    public CharacterSO FromData(CharacterUpgradeData data) {
        var so = CreateInstance<CharacterSO>();
        so.name = $"Runtime_{data.characterName}";
        so.characterName = data.characterName;
        so.icon = null; // optional, handle sprite restore separately
        so.upgrade = data.ToRuntimeSO();
        return so;
    }


}
