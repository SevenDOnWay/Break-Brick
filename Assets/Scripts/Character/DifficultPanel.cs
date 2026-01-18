using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class DifficultPanel : MonoBehaviour {
    SelectCharacter selectCharacter;

    //[SerializeField] GameObject difficultText;
    TextMeshProUGUI t_difficult;
    Image t_difficultColor;

    string[] difficultyStrings = { "Easy", "Normal", "Hard" };
    string[] diffcultyColors = { "#69B578", "#FFD25A", "#DB162F" };

    [Inject]
    private void Constructor( SelectCharacter selectCharacter ) {
        this.selectCharacter = selectCharacter;
    }

    void Start() {
        t_difficult = transform.Find("t_difficult").GetComponent<TextMeshProUGUI>();
        t_difficultColor = transform.Find("t_difficultColor").GetComponent<Image>();

        UpdateDifficultyText(selectCharacter.GetCurrentDifficultyIndex());

        //if ( playerDataManager == null ) Debug.LogError("playerDataManager is null in DifficultPanel");
        //else Debug.Log("playerDataManager is not null in DifficultPanel");

        selectCharacter.OnDifficultChange += UpdateDifficultyText;
    }

    void UpdateDifficultyText( int index ) {


        bool unlocked = false;

        switch ( index ) {
            case 0: // Easy
                unlocked = true;
                break;
            case 1: // Normal
                unlocked = PlayerDataManager.Instance.playerData.timeWinEsayMode >= 1;
                break;
            case 2: // Hard
                unlocked = PlayerDataManager.Instance.playerData.timeWinNormalMode >= 1;
                break;
            case 3: // Insane (optional)
                unlocked = PlayerDataManager.Instance.playerData.timeWinHardMode >= 3;
                break;
        }

        if ( !unlocked ) {
            t_difficultColor.color = Color.gray;
            t_difficult.text = $"{difficultyStrings[index]} (Locked)";
            return;
        }


        if ( ColorUtility.TryParseHtmlString(diffcultyColors[index], out Color color) )
            t_difficultColor.color = color;

        t_difficult.text = difficultyStrings[index];
    }

}
