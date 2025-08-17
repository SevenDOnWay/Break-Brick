using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class SelectCharacter : MonoBehaviour {
    [Inject] CharacterEntry characterEntry;
    private int currentCharacterIndex = 0;

    [SerializeField] Button nextDifficulty;
    [SerializeField] Button previousDifficulty;
    private int currentDifficultIndex = 0;
    int maxDifficultIndex = 2; // hardcode for now 1 esay, 2 normal, 3 hard

    public event Action<int> OnDifficultChange;
    public event Action<int> OnCharacterChange;
    public event Action OnPlay;


    void Awake() {
        //TODO : Load the current player and difficulty from playerdata or settings
        SetActiveCharacter(currentCharacterIndex);
        CheckButtonDifficulty();
    }

    public void OnClicNextDifficulty() {
        OnDifficultChange?.Invoke(++currentDifficultIndex);
        CheckButtonDifficulty();
    }

    public void OnClickPreviousDifficulty() {
        OnDifficultChange?.Invoke(--currentDifficultIndex);
        CheckButtonDifficulty();
    }

    public void OnClickNextCharacter() {
        currentCharacterIndex = (currentCharacterIndex + 1) % characterEntry.characters.Length;
        SetActiveCharacter(currentCharacterIndex);
        OnCharacterChange?.Invoke(currentCharacterIndex);
        Debug.Log($"Current Player Index: {currentCharacterIndex}"); // Debug log to check the current player index
    }

    public void OnClickPreviousCharacter() {
        currentCharacterIndex = (currentCharacterIndex - 1 + characterEntry.characters.Length) % characterEntry.characters.Length;
        SetActiveCharacter(currentCharacterIndex);
        OnCharacterChange?.Invoke(currentCharacterIndex);
    }

    public void OnClickPlay() {
        //TODO: Implement the logic to start the game with the selected player and difficulty
        OnPlay?.Invoke();


        //TODO: Save the current player and difficulty to playerdata or settings
        SceneManager.LoadScene(2); 
    }

    void SetActiveCharacter( int index ) {
        for ( int i = 0; i < characterEntry.characters.Length; i++ ) {
            //characterEntry.characters[i].SetActive(i == index);
        }
        CheckButtonDifficulty();
    }

    void CheckButtonDifficulty() {
        nextDifficulty.interactable = currentDifficultIndex < maxDifficultIndex;
        previousDifficulty.interactable = currentDifficultIndex > 0;
    }

    public int GetCurrentPlayerIndex() => currentCharacterIndex;
    public int GetCurrentDifficultyIndex() => currentDifficultIndex;
}
