using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

public class CharacterManager : MonoBehaviour {

    //[Inject] CharacterEntry characterEntry;

    const string characterLabel = "Character Powerup";


    List<CharacterSO> characters = new List<CharacterSO>();
    private Task<List<CharacterSO>> loadingTask;

    public async Task<List<CharacterSO>> GetCharacters() {
        if ( characters.Count > 0 )
            return characters;

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
        characters.AddRange(result);
        Debug.Log($"[CharacterManager] Loaded {characters.Count} CharacterSO assets.");
        return characters;
    }

}
