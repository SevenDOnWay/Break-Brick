using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

[System.Serializable]
public class RunData {

    public bool isContinuing = false;

    [JsonInclude] int difficult;
    [JsonInclude] int waveIndex;
    [JsonInclude] int ballCount;
    [JsonInclude] float ballPosX;
    [JsonInclude] float ballPosY;

    [JsonInclude] List<BrickData> bricksData;

    [JsonInclude] CharacterUpgradeData characterUpgradeData;

    public RunData(int difficult, CharacterUpgradeData characterUpgradeData ) {
        this.difficult = difficult;
        this.characterUpgradeData = characterUpgradeData;
    }
    
    public int GetDifficultIndex() => difficult;
    public int GetWaveIndex() => waveIndex;
    public int GetBallCount() => ballCount;
    public Vector2 GetBallPos() => new Vector2(ballPosX, ballPosY);
    public List<BrickData> GetBricksData() => bricksData;
    public CharacterUpgradeData GetCharacterUpgradeData() => characterUpgradeData;

    public void OverwriteBricksData( List<BrickData> bricksData ) {
        this.bricksData = bricksData;
    }

    public void OverwriteBallCount( int ballCount ) {
        this.ballCount = ballCount;
    }

    public void OverwriteBallPos( Vector2 ballPos ) {
        this.ballPosX = ballPos.x;
        this.ballPosY = ballPos.y;
    }

    public void OverwriteWaveIndex( int waveIndex ) {
        this.waveIndex = waveIndex;
    }


    //TODO: add to this

    //public UpgradeSO upgradeSO;
    //public int Level;
}
