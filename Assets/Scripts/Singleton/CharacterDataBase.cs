using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class CharacterDataBase : MonoBehaviour {

    //[Inject] CharacterEntry characterEntry;

    const string characterLabel = "Character Upgrade";

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
        Debug.Log($"[characterDataBase] Loaded {characterSOs.Count} CharacterSO assets.");
        return characterSOs;
    }

    public async Task<CharacterSO> GetCharacterByID( string id ) {
        if( characterSOs.Count < 0 ) await GetCharacters();

        foreach ( var character in characterSOs ) {
            if ( character.GetCharacterId() == id ) return character;
        }

        Debug.LogWarning($"[characterDataBase] Character with ID {id} not found.");
        return null;
    }

}
