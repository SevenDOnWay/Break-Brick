using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class RunDataManager : MonoBehaviour {

    public RunData runData;

    string savePath;
    string historyDirectory;

    JsonSerializerOptions option;

    void Awake() {

        savePath = Path.Combine(Application.persistentDataPath, "rundata.dat");
        historyDirectory = Path.Combine(Application.persistentDataPath, "RunHistory");

        option = new JsonSerializerOptions {
            WriteIndented = true,
            IncludeFields = true, // IMPORTANT so private fields get deserialized
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
            await SaveHistoryJson(json);
            Debug.Log($"RunData saved at: {savePath}");
        }
        catch ( Exception e ) {
            Debug.LogError($"Failed to save RunData: {e.Message}");
        }
    }

    /// <summary>
    /// Writes an unencrypted, analysis-friendly copy of the current run. These files are
    /// intentionally retained after DeleteRun so external balance tools can inspect them.
    /// </summary>
    public async Task SaveHistoryJson() {
        if ( runData == null ) return;

        try {
            string json = JsonSerializer.Serialize(runData, option);
            await SaveHistoryJson(json);
        }
        catch ( Exception e ) {
            Debug.LogError($"Failed to save run history JSON: {e.Message}");
        }
    }

    async Task SaveHistoryJson( string json ) {
        Directory.CreateDirectory(historyDirectory);
        string historyPath = Path.Combine(historyDirectory, $"run-history-{runData.GetRunId()}.json");
        await File.WriteAllTextAsync(historyPath, json, Encoding.UTF8);
        Debug.Log($"Run history JSON saved at: {historyPath}");
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
