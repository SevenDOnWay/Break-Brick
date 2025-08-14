using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectPlayer : MonoBehaviour {
    [SerializeField] public GameObject[] Characters;
    private int currentPlayerIndex = 0;

    [SerializeField] Button nextDifficulty;
    [SerializeField] Button previousDifficulty;
    private int currentDifficultIndex = 0;
    int maxDifficultIndex = 2; // hardcode for now 1 esay, 2 normal, 3 hard

    public event Action<int> OnDifficultChange;
    public event Action<int> OnCharacterChange;


    void Awake() {
        //TODO : Load the current player and difficulty from playerdata or settings
        SetActivePlayer(currentPlayerIndex);
        CheckButtonDifficulty();
    }

    public void OnClicNextkDifficulty() {
        OnDifficultChange?.Invoke(++currentDifficultIndex);
        CheckButtonDifficulty();
    }

    public void OnClickPreviousDifficulty() {
        OnDifficultChange?.Invoke(--currentDifficultIndex);
        CheckButtonDifficulty();
    }

    public void OnClickNextPlayer() {
        currentPlayerIndex = (currentPlayerIndex + 1) % Characters.Length;
        SetActivePlayer(currentPlayerIndex);
        OnCharacterChange?.Invoke(currentPlayerIndex);
        Debug.Log($"Current Player Index: {currentPlayerIndex}"); // Debug log to check the current player index
    }

    public void OnClickPreviousPlayer() {
        currentPlayerIndex = (currentPlayerIndex - 1 + Characters.Length) % Characters.Length;
        SetActivePlayer(currentPlayerIndex);
        OnCharacterChange?.Invoke(currentPlayerIndex);
    }

    public void OnClickPlay() {
        //TODO: Implement the logic to start the game with the selected player and difficulty



        //TODO: Save the current player and difficulty to playerdata or settings
        SceneManager.LoadScene(2); 
    }

    void SetActivePlayer( int index ) {
        for ( int i = 0; i < Characters.Length; i++ ) {
            Characters[i].SetActive(i == index);
        }
        CheckButtonDifficulty();
    }

    void CheckButtonDifficulty() {
        nextDifficulty.interactable = currentDifficultIndex < maxDifficultIndex;
        previousDifficulty.interactable = currentDifficultIndex > 0;
    }

    public int GetCurrentPlayerIndex() => currentPlayerIndex;
    public int GetCurrentDifficultyIndex() => currentDifficultIndex;
}
