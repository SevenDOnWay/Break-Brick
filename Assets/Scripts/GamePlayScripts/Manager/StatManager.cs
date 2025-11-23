using System.Collections.Generic;
using UnityEngine;

public class StatManager {
    private Dictionary<UpgradeType, float> baseStat = new Dictionary<UpgradeType, float> {
        { UpgradeType.Speed, 5f },
        { UpgradeType.CritChance, 0f },
        { UpgradeType.CritMultiplier, 2f },
        { UpgradeType.FireChance, 0f },
        { UpgradeType.LightningChance, 0f }
    };

    private Dictionary<UpgradeType, float> finalStat = new Dictionary<UpgradeType, float>();

    public StatManager() {
        // Initialize finalStat with baseStat values
        foreach ( var pair in baseStat ) {
            finalStat[pair.Key] = pair.Value;
        }
    }

    public IReadOnlyDictionary<UpgradeType, float> GetAllStats() => finalStat;

    public float GetStat( UpgradeType type ) => finalStat.ContainsKey(type) ? finalStat[type] : 0f;

    public void ModifyStat( UpgradeType type, float value ) {
        if ( !finalStat.ContainsKey(type) ) {
            Debug.LogWarning($"Property {type} not found in statsManager, adding it.");
            finalStat[type] = value;
        }
        else {
            finalStat[type] += value;
        }
    }

    public void ResetStats() {
        finalStat.Clear();
        foreach ( var pair in baseStat )
            finalStat[pair.Key] = pair.Value;
    }
}
