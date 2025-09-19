using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class RunDataManager : MonoBehaviour {

    public static RunDataManager Instance { get; private set; }

    public RunData runData;

    string savePath;

    JsonSerializerOptions option;

    private async void Awake() {
        if ( Instance != null && Instance != this ) {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "rundata.dat");

        option = new JsonSerializerOptions {
            WriteIndented = true,
            IncludeFields = true, // IMPORTANT so private fields get deserialized
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        runData = await Load();
        if ( runData != null ) {
            Debug.Log("Previous run loaded!");
        }
        else {
            Debug.Log("No previous run found.");
        }
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

    public async Task<RunData> Load() {
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

        // fallback: always return a fresh RunData so it's never null
        return null;
    }

    public async Task NewRun( int difficult, CharacterUpgradeData characterUpgradeData ) {

        runData = new RunData(difficult, characterUpgradeData);

        await Save();
    }

    public async Task NewRun( int difficult, CharacterSO characterSO ) {

        runData = new RunData(difficult, new CharacterUpgradeData(characterSO));

        await Save();
    }

    public void ClearRun() {
        runData = null;
        if ( File.Exists(savePath) ) File.Delete(savePath);
        Debug.Log("RunData cleared.");
    }

}
