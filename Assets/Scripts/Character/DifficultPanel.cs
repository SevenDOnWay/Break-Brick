using TMPro;
using UnityEngine;
using VContainer;

public class DifficultPanel : MonoBehaviour {
    [Inject] SelectCharacter selectCharacter;

    [SerializeField] TextMeshProUGUI difficultyText;

    string[] difficultyStrings = { "Easy", "Normal", "Hard" };

    void Start() {
        UpdateDifficultyText(selectCharacter.GetCurrentDifficultyIndex());

        selectCharacter.OnDifficultChange += UpdateDifficultyText;
    }

    void UpdateDifficultyText( int index ) {
        difficultyText.text = difficultyStrings[index];
    }
}
