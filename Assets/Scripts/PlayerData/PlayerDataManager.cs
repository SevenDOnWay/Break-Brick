using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour {

    public PlayerData playerData;

    string savePath;

    public async void Awake() {

        savePath = Application.persistentDataPath + "/playerdata.sav";

        await Load();
    }


    async Task Save() {
        await Task.Run(() => {
            BinaryFormatter formatter = new BinaryFormatter();
            using ( FileStream stream = new FileStream(savePath, FileMode.Create) ) {
                formatter.Serialize(stream, playerData);
            }
        });

        Debug.Log($"Player data save as! {savePath}");
    }

    async Task Load() {
        if ( File.Exists(savePath) ) {
            playerData = await Task.Run(() => {
                BinaryFormatter formatter = new BinaryFormatter();
                using ( FileStream stream = new FileStream(savePath, FileMode.Open) ) {
                    return formatter.Deserialize(stream) as PlayerData;
                }
            });

            Debug.Log("Player data loaded async!");
        }
        else {
            Debug.LogWarning("No save file found, creating new data...");
            // Create new player data
            playerData = new PlayerData();
        }


    }



}
