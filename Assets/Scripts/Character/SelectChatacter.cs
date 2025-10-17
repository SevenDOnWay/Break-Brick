using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class SelectCharacter : MonoBehaviour {
    [Inject] CharacterManager characterManager;

    private int currentCharacterIndex = 0;

    [SerializeField] Button nextDifficulty;
    [SerializeField] Button previousDifficulty;
    private int currentDifficultIndex = 0;
    int maxDifficultIndex = 2; // hardcode for now 1 esay, 2 normal, 3 hard

    List<CharacterSO> characterSos;

    public event Action<int> OnDifficultChange;
    public event Action<int> OnCharacterChange;
    public event Action OnPlay;


    async void Awake() {
        characterSos = await characterManager.GetCharacters();

        //TODO : Load the current player and difficulty from playerdata or settings
        CheckButtonDifficulty();
    }


    #region Button_Difficulty_Logic
    public void OnClicNextDifficulty() {
        OnDifficultChange?.Invoke(++currentDifficultIndex);
        CheckButtonDifficulty();
    }

    public void OnClickPreviousDifficulty() {
        OnDifficultChange?.Invoke(--currentDifficultIndex);
        CheckButtonDifficulty();
    }
    #endregion

    #region Button_Character_Logic
    public void OnClickNextCharacter() {
        currentCharacterIndex = (currentCharacterIndex + 1) % characterSos.Count;
        OnCharacterChange?.Invoke(currentCharacterIndex);
    }

    public void OnClickPreviousCharacter() {
        currentCharacterIndex = (currentCharacterIndex - 1 + characterSos.Count) % characterSos.Count;
        OnCharacterChange?.Invoke(currentCharacterIndex);
    }
    #endregion

    public async void OnClickPlay() {
        OnPlay?.Invoke();

        await RunDataManager.Instance.NewRun(currentCharacterIndex, characterSos[currentCharacterIndex]);

        await SceneManager.LoadSceneAsync(2);
    }

    void CheckButtonDifficulty() {
        nextDifficulty.interactable = currentDifficultIndex < maxDifficultIndex;
        previousDifficulty.interactable = currentDifficultIndex > 0;
    }

    public int GetCurrentPlayerIndex() => currentCharacterIndex;
    public int GetCurrentDifficultyIndex() => currentDifficultIndex;
}
