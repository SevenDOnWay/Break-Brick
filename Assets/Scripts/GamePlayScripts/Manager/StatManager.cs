using System.Collections.Generic;
using UnityEngine;

public class StatManager {
    private Dictionary<UpgradeType, float> baseStat = new Dictionary<UpgradeType, float> {
        { UpgradeType.Speed, 5f },
        { UpgradeType.CritChance, 0f },
        { UpgradeType.CritMultiplier, 2f },
        { UpgradeType.FireChance, 0f },
        { UpgradeType.LightningChance, 0f },
        { UpgradeType.LightningBounces, 3f },
        { UpgradeType.ExplosionChance, 0f },
        { UpgradeType.ExplosionRadius, 0f },
        { UpgradeType.PoisonChance, 0f },
        { UpgradeType.PoisonDuration, 3f },
        { UpgradeType.FreezeChance, 0f },
        { UpgradeType.FreezeDuration, 2f },
        { UpgradeType.SniperInterval, 5f },
        { UpgradeType.ShockwaveChance, 0f },
        { UpgradeType.RallyBonus, 1f },
        { UpgradeType.ExpBonus, 0f },
    };

    private static readonly Dictionary<UpgradeType, float> s_softCaps = new Dictionary<UpgradeType, float> {
        { UpgradeType.CritChance,       0.80f },
        { UpgradeType.ExplosionChance,  0.40f },
        { UpgradeType.LightningChance,  0.35f },
        { UpgradeType.PoisonChance,     0.50f },
        { UpgradeType.FreezeChance,     0.30f },
        { UpgradeType.ShockwaveChance,  0.50f },
        { UpgradeType.ExplosionRadius,  3.0f  },
        { UpgradeType.ExpBonus,         2.0f  },
    };

    private Dictionary<UpgradeType, float> finalStat = new Dictionary<UpgradeType, float>();

    public StatManager() {
        foreach ( var pair in baseStat ) {
            finalStat[pair.Key] = pair.Value;
        }
    }

    public Dictionary<UpgradeType, float> GetAllStats() => finalStat;

    public float GetStat( UpgradeType type ) => finalStat.ContainsKey(type) ? finalStat[type] : 0f;

    public void ModifyStat( UpgradeType type, float value ) {
        if ( !finalStat.ContainsKey(type) ) {
            Debug.LogWarning($"Property {type} not found in statsManager, adding it.");
            finalStat[type] = value;
        }
        else {
            finalStat[type] += value;
        }

        if ( s_softCaps.TryGetValue(type, out float cap) ) {
            finalStat[type] = Mathf.Min(finalStat[type], cap);
        }
    }

    public void ResetStats() {
        finalStat.Clear();
        foreach ( var pair in baseStat )
            finalStat[pair.Key] = pair.Value;
    }
}

