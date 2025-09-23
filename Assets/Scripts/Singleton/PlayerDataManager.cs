using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEngine;

[System.Serializable]
public class PlayerDataManager : MonoBehaviour {

    public static PlayerDataManager Instance { get; private set; }

    public PlayerData playerData;

    string savePath;
    JsonSerializerOptions option;

    private async void Awake() {
        if ( Instance != null && Instance != this ) {
            Destroy(this.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "playerdata.dat");

        option = new JsonSerializerOptions
                {
            WriteIndented = true,
            IncludeFields = true, // IMPORTANT so private fields get deserialized
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        playerData = await Load();

        // If no existing data, create new and save it immediately
        if ( playerData == null ) {
            Debug.Log("No save found → creating new PlayerData.");
            playerData = new PlayerData();
            await Save();
        }

    }


    public async Task Save() {
        try {
            string json = JsonSerializer.Serialize(playerData, option);
            byte[] data = Encoding.UTF8.GetBytes(json);

            // Encrypt using reusable XorUtility
            byte[] encrypted = XorUtility.XorEncrypt(data);

            await File.WriteAllBytesAsync(savePath, encrypted);
            Debug.Log($"PlayerData saved at {savePath}");
        }
        catch ( Exception e ) {
            Debug.LogError($"Failed to save PlayerData: {e.Message}");
        }
    }


    async Task<PlayerData> Load() {
        try {
            if ( File.Exists(savePath) ) {
                byte[] encrypted = await File.ReadAllBytesAsync(savePath);

                // Decrypt using reusable XorUtility
                byte[] data = XorUtility.XorEncrypt(encrypted);

                string json = Encoding.UTF8.GetString(data);
                return JsonSerializer.Deserialize<PlayerData>(json, option);
            }
        }
        catch ( Exception e ) {
            Debug.LogWarning($"Failed to load PlayerData: {e.Message}");
        }
        return null;
    }
}

