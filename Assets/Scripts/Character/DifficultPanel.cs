using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class DifficultPanel : MonoBehaviour {
    [Inject] SelectCharacter selectCharacter;
    //[Inject] PlayerDataManager playerDataManager;

    //[SerializeField] GameObject difficultText;
    TextMeshProUGUI t_difficult;
    Image t_difficultColor;

    string[] difficultyStrings = { "Easy", "Normal", "Hard" };
    string[] diffcultyColors = { "#69B578", "#FFD25A", "#DB162F" };


    void Start() {
        t_difficult = transform.Find("t_difficult").GetComponent<TextMeshProUGUI>();
        t_difficultColor = transform.Find("t_difficultColor").GetComponent<Image>();

        UpdateDifficultyText(selectCharacter.GetCurrentDifficultyIndex());

        selectCharacter.OnDifficultChange += UpdateDifficultyText;
    }

    void UpdateDifficultyText( int index ) {

        //TODO: Unlock difficult based on player data

        //bool unlocked = false;

        //switch ( index ) {
        //    case 0: // Easy
        //        unlocked = true;
        //        break;
        //    case 1: // Normal
        //        unlocked = playerDataManager.playerData.timeWinEsayMode >= 1;
        //        break;
        //    case 2: // Hard
        //        unlocked = playerDataManager.playerData.timeWinNormalMode >= 1;
        //        break;
        //    case 3: // Insane (optional)
        //        unlocked = playerDataManager.playerData.timeWinHardMode >= 3;
        //        break;
        //}

        //if ( !unlocked ) {
        //    t_difficultColor.color = Color.gray;
        //    t_difficult.text = $"{difficultyStrings[index]} (Locked)";
        //    return;
        //}


        if ( ColorUtility.TryParseHtmlString(diffcultyColors[index], out Color color) )
            t_difficultColor.color = color;
        else Debug.LogError("Invalid color string: " + diffcultyColors[index]);

        t_difficult.text = difficultyStrings[index];
    }

}
