using TMPro;
using UnityEngine;

public class DifficultPanel : MonoBehaviour {
    [SerializeField] TextMeshProUGUI difficultyText;
    string[] difficultyStrings = { "Easy", "Normal", "Hard" };

    SelectPlayer selctPlayer;


    void Start() {
        selctPlayer = GetComponentInParent<SelectPlayer>();

        if ( selctPlayer == null ) {
            Debug.LogError("SelectPlayer component not found in parent.");
            return;
        }

        // Initialize the difficulty text with the current difficulty

        UpdateDifficultyText(selctPlayer.GetCurrentDifficultyIndex());

        selctPlayer.OnDifficultChange += UpdateDifficultyText;
    }

    void UpdateDifficultyText( int index ) {
        difficultyText.text = difficultyStrings[index];
    }
}
