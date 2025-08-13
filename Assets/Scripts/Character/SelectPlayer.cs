using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectPlayer : MonoBehaviour {
    [SerializeField] private GameObject[] players;
    private int currentPlayerIndex = 0;

    [SerializeField] private GameObject[] difficulty;
    [SerializeField] Button nextDifficulty;
    [SerializeField] Button previousDifficulty;
    private int currentDifficultyIndex = 0;



    private void Start() {
        //TODO : Load the current player and difficulty from playerdata or settings
        SetActivePlayer(currentPlayerIndex);
        SetActiveDifficulty(currentDifficultyIndex);
        CheckButtonDifficulty();
    }



    public void OnClicNextkDifficulty() {
        SetActiveDifficulty(currentDifficultyIndex + 1);
    }

    public void OnClickPreviousDifficulty() {
        SetActiveDifficulty(currentDifficultyIndex - 1);
    }

    public void OnClickNextPlayer() {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        SetActivePlayer(currentPlayerIndex);
    }

    public void OnClickPreviousPlayer() {
        currentPlayerIndex = (currentPlayerIndex - 1 + players.Length) % players.Length;
        SetActivePlayer(currentPlayerIndex);
    }

    public void OnClickPlay() {
        //TODO: Implement the logic to start the game with the selected player and difficulty

        //TODO: Save the current player and difficulty to playerdata or settings
        SceneManager.LoadScene(2); 
    }

    void SetActivePlayer( int index ) {
        for ( int i = 0; i < players.Length; i++ ) {
            players[i].SetActive(i == index);
        }
        CheckButtonDifficulty();
    }

    void CheckButtonDifficulty() {
        nextDifficulty.interactable = currentDifficultyIndex < difficulty.Length - 1;
        previousDifficulty.interactable = currentDifficultyIndex > 0;
    }

    void SetActiveDifficulty( int index ) {
        for ( int i = 0; i < difficulty.Length; i++ ) {
            difficulty[i].SetActive(i == index);
        }
    }

    public int GetCurrentPlayerIndex() => currentPlayerIndex;
    public int GetCurrentDifficultyIndex() => currentDifficultyIndex;
}
