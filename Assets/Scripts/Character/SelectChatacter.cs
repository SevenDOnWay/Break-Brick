using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class SelectCharacter : MonoBehaviour {
    RunDataManager runDataManager;
    CharacterDataBase characterDataBase;

    PlayerDataManager playerDataManager;

    private int currentCharacterIndex = 0;

    [SerializeField] Button nextDifficulty;
    [SerializeField] Button previousDifficulty;
    private int currentDifficultIndex = 0;
    const int maxDifficultIndex = 2; // hardcode for now 1 esay, 2 normal, 3 hard

    List<CharacterSO> characterSos;

    public event Action<int> OnDifficultChange;
    public event Action<int> OnCharacterChange;
    public event Action OnPlay;

    [Inject]
    void Constructor(
        RunDataManager runDataManager,
        CharacterDataBase characterManager,
        PlayerDataManager playerDataManager
    ) {
        this.runDataManager = runDataManager;
        this.characterDataBase = characterManager;
        this.playerDataManager = playerDataManager;
    }

    void Awake() {
        _ = LoadCharactersSOs();

        //TODO : Load the current player and difficulty from playerdata
        CheckButtonDifficulty();
    }

    private async Task LoadCharactersSOs() {
        characterSos = await characterDataBase.GetCharacters();
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

        runDataManager.runData = new RunData(currentCharacterIndex, characterSos[currentCharacterIndex].GetCharacterId());
        await runDataManager.Save();

        await SceneManager.LoadSceneAsync(2);
    }

    void CheckButtonDifficulty() {
        nextDifficulty.interactable = currentDifficultIndex < maxDifficultIndex;
        previousDifficulty.interactable = currentDifficultIndex > 0;
    }

    public int GetCurrentPlayerIndex() => currentCharacterIndex;
    public int GetCurrentDifficultyIndex() => currentDifficultIndex;
}
