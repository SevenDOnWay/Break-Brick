using System.Text.Json.Serialization;

[System.Serializable]
public class UpgradeData {


    public string upgradeName;
    public UpgradeType upgradeType;
    public float primaryValue;
    public float secondaryValue;
    public string description;



    [JsonConstructor]
    public UpgradeData( string upgradeName, UpgradeType upgradeType, float primaryValue, float secondaryValue, string description ) {
        this.upgradeName = upgradeName;
        this.upgradeType = upgradeType;
        this.primaryValue = primaryValue;
        this.secondaryValue = secondaryValue;
        this.description = description;
    }

    public UpgradeData( UpgradeSO upgradeSO ) {
        this.upgradeName = upgradeSO.GetUpgradeName();
        this.upgradeType = upgradeSO.GetUpgradeType();
        this.primaryValue = upgradeSO.GetPrimaryValue();
        this.secondaryValue = upgradeSO.GetSecondaryValue();
        this.description = upgradeSO.GetDescription();
    }

}

[System.Serializable]
public class CharacterUpgradeData : UpgradeData {

    public string characterName;

    [JsonConstructor]
    public CharacterUpgradeData(
        string characterName,
        string upgradeName,
        UpgradeType upgradeType,
        float primaryValue,
        float secondaryValue,
        string description
    ) : base(upgradeName, upgradeType, primaryValue, secondaryValue, description) {
        this.characterName = characterName;
    }

    public CharacterUpgradeData( CharacterSO characterSO ) : base(characterSO.GetUpgrade()) {
        this.characterName = characterSO.GetCharacterName();
    }

    public UpgradeSO ToRuntimeSO() {
        return UpgradeSO.FromData(this);
    }
}
