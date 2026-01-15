using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

[System.Serializable]
public class RunData {
    [JsonInclude] int difficult;
    [JsonInclude] int waveIndex;
    [JsonInclude] int ballCount;
    [JsonInclude] float ballPosX;
    [JsonInclude] float ballPosY;

    [JsonInclude] List<BrickData> bricksData;

    [JsonInclude] string characterSOId;
    //[JsonInclude] list<string> upgradeSOId;

    bool isContinuing = false;

    public RunData( int difficult, string characterSOId ) {
        this.difficult = difficult;
        this.characterSOId = characterSOId;
    }

    public void SetIsContinuing( bool isContinuing ) {
        this.isContinuing = isContinuing;
    }

    public int GetDifficultIndex() => difficult;
    public int GetWaveIndex() => waveIndex;
    public int GetBallCount() => ballCount;
    public Vector2 GetBallPos() => new Vector2(ballPosX, ballPosY);
    public List<BrickData> GetBricksData() => bricksData;

    //TODO: implement character upgrade 
    //public CharacterUpgradeData GetCharacterUpgradeData() => characterUpgradeData;
    public bool GetIsContinuing() => isContinuing;

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
}
