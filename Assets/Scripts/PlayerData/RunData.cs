using System.Collections.Generic;
using System;
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

    [JsonInclude] string runId;
    [JsonInclude] List<RunTurnData> turnHistory = new();

    bool isContinuing = false;

    public RunData( int difficult, string characterSOId ) {
        this.difficult = difficult;
        this.characterSOId = characterSOId;
        runId = Guid.NewGuid().ToString("N");
    }

    public void SetIsContinuing( bool isContinuing ) {
        this.isContinuing = isContinuing;
    }

    public int GetDifficultIndex() => difficult;
    public int GetWaveIndex() => waveIndex;
    public int GetBallCount() => ballCount;
    public Vector2 GetBallPos() => new Vector2(ballPosX, ballPosY);
    public List<BrickData> GetBricksData() => bricksData;
    public string GetCharacterSOId() => characterSOId;
    public string GetRunId() {
        EnsureHistory();
        return runId;
    }
    public IReadOnlyList<RunTurnData> GetTurnHistory() {
        EnsureHistory();
        return turnHistory;
    }

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

    public void BeginTurn( int waveIndex, int ballCount, IReadOnlyList<UpgradeSO> currentUpgrades ) {
        EnsureHistory();

        var turn = new RunTurnData {
            turnIndex = turnHistory.Count + 1,
            waveIndex = waveIndex,
            ballCount = ballCount,
        };

        if ( currentUpgrades != null ) {
            foreach ( UpgradeSO upgrade in currentUpgrades ) {
                if ( upgrade == null ) continue;
                turn.upgrades.Add(new UpgradeHistoryData {
                    upgradeId = upgrade.GetUpgradeId(),
                    upgradeName = upgrade.GetUpgradeName(),
                });
            }
        }

        turnHistory.Add(turn);
    }

    public void RecordDamage( DamageSource source, int amount ) {
        if ( amount <= 0 ) {
            return;
        }

        RunTurnData turn = GetLastTurn();
        if ( turn == null ) {
            return;
        }

        turn.damageDealt += amount;
        turn.RecordDamage(source, amount);
    }

    public void RecordBrickDestroyed() {
        RunTurnData turn = GetLastTurn();
        if ( turn != null ) {
            turn.bricksDestroyed++;
        }
    }

    void EnsureHistory() {
        if ( string.IsNullOrEmpty(runId) ) {
            runId = Guid.NewGuid().ToString("N");
        }

        turnHistory ??= new List<RunTurnData>();
    }

    RunTurnData GetLastTurn() {
        EnsureHistory();
        return turnHistory.Count > 0 ? turnHistory[turnHistory.Count - 1] : null;
    }
}

[System.Serializable]
public class RunTurnData {
    [JsonInclude] public int turnIndex;
    [JsonInclude] public int waveIndex;
    [JsonInclude] public int ballCount;
    [JsonInclude] public int damageDealt;
    [JsonInclude] public int bricksDestroyed;
    [JsonInclude] public List<DamageSourceData> damageBySource = new();
    [JsonInclude] public List<UpgradeHistoryData> upgrades = new();

    public void RecordDamage( DamageSource source, int amount ) {
        DamageSourceData sourceData = damageBySource.Find(entry => entry.source == source.ToString());
        if ( sourceData == null ) {
            sourceData = new DamageSourceData { source = source.ToString() };
            damageBySource.Add(sourceData);
        }

        sourceData.damageDealt += amount;
    }
}

[System.Serializable]
public class DamageSourceData {
    [JsonInclude] public string source;
    [JsonInclude] public int damageDealt;
}

[System.Serializable]
public class UpgradeHistoryData {
    [JsonInclude] public string upgradeId;
    [JsonInclude] public string upgradeName;
}
