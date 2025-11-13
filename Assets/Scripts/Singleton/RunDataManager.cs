using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

[System.Serializable]
public class RunDataManager : MonoBehaviour {

    public RunData runData;

    string savePath;

    JsonSerializerOptions option;

    void Awake() {

        savePath = Path.Combine(Application.persistentDataPath, "rundata.dat");

        option = new JsonSerializerOptions {
            WriteIndented = true,
            IncludeFields = true, // IMPORTANT so private fields get deserialized
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        _ = InitializeAsync();

        Debug.Log($"RunDataManager Awake. SavePath: {savePath}");

        if ( runData != null ) {
            Debug.Log("Previous run loaded!");
        }
        else {
            Debug.Log("No previous run found.");
        }
    }

    private async Task InitializeAsync() {
        runData = await Load();

        if ( runData != null )
            Debug.Log("Previous run loaded!");
        else
            Debug.Log("No previous run found.");
    }

    public async Task Save() {
        if ( runData == null ) return;
        try {
            string json = JsonSerializer.Serialize(runData, option);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            byte[] encrypted = XorUtility.XorEncrypt(bytes);

            await File.WriteAllBytesAsync(savePath, encrypted);
            Debug.Log($"RunData saved at: {savePath}");

            // Also save a plain JSON version for debugging
            string debugPath = Path.Combine(Application.persistentDataPath, "rundata_debug.json");
            await File.WriteAllTextAsync(debugPath, json);
            Debug.Log($"RunData saved (plain JSON) at: {debugPath}");
        }
        catch ( Exception e ) {
            Debug.LogError($"Failed to save RunData: {e.Message}");
        }
    }

    public async Task<RunData> Load( CancellationToken token = default ) {
        try {
            if ( File.Exists(savePath) ) {
                RunData loaded = await Task.Run(() => {
                    byte[] encrypted = File.ReadAllBytes(savePath);
                    byte[] decrypted = XorUtility.XorEncrypt(encrypted);
                    string json = Encoding.UTF8.GetString(decrypted);

                    var options = new JsonSerializerOptions {
                        IncludeFields = true
                    };

                    return JsonSerializer.Deserialize<RunData>(json, options);
                });

                Debug.Log("RunData loaded async!");
                return loaded;
            }
            else {
                Debug.Log("No existing runData file found.");
            }
        }
        catch ( Exception e ) {
            Debug.LogWarning($"Failed to load RunData: {e.Message}");
        }

        // fallback
        return null;
    }

    //public async Task NewRun( int difficult, CharacterUpgradeData characterUpgradeData ) {
    //    runData = new RunData(difficult, characterUpgradeData);

    //    await Save();
    //}

    //public async Task NewRun( int difficult, CharacterSO characterSO ) {

    //    runData = new RunData(difficult, new CharacterUpgradeData(characterSO));

    //    await Save();
    //}


    public void CreateNewRun( int characterIndex, string characterId ) {
        runData = new RunData(characterIndex, characterId);
        Debug.Log("New RunData created.");
    }

    public void DeleteRun() {
        runData = null;
        if ( File.Exists(savePath) ) File.Delete(savePath);
        Debug.Log("RunData cleared.");
    }

}
