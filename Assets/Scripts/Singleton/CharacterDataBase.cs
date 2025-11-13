using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

public class CharacterDataBase : MonoBehaviour {

    //[Inject] CharacterEntry characterEntry;

    const string characterLabel = "Character Powerup";


    List<CharacterSO> characterSOs = new List<CharacterSO>(); 
    private Task<List<CharacterSO>> loadingTask;

    public async Task<List<CharacterSO>> GetCharacters() {
        if ( characterSOs.Count > 0 )
            return characterSOs;

        // if another call already started loading, wait for it
        if ( loadingTask != null )
            return await loadingTask;

        // otherwise start new load
        loadingTask = LoadCharactersAsync();
        return await loadingTask;

    }

    async Task<List<CharacterSO>> LoadCharactersAsync() {
        var handle = Addressables.LoadAssetsAsync<CharacterSO>(characterLabel, null);
        var result = await handle.Task;
        characterSOs.AddRange(result);
        Debug.Log($"[CharacterDataBase] Loaded {characterSOs.Count} CharacterSO assets.");
        return characterSOs;
    }

}
