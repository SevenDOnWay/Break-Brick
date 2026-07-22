using System.Collections.Generic;

/// <summary>
/// Applies data-only upgrade definitions through Core contracts. It is the
/// gameplay-side interpreter for upgrade assets.
/// </summary>
public class UpgradeApplicationService {
    public void Apply( UpgradeSO upgrade, IUpgradeContext context ) {
        if ( upgrade == null || context == null ) {
            return;
        }

        switch ( upgrade ) {
            case UpgradeStatSO statUpgrade:
                ApplyStatUpgrade(statUpgrade, context);
                break;
            case UpgradeBehaviorSO behaviorUpgrade:
                ApplyBehaviorUpgrade(behaviorUpgrade, context);
                break;
        }
    }

    void ApplyStatUpgrade( UpgradeStatSO upgrade, IUpgradeContext context ) {
        IReadOnlyList<UpgradeStatSO.UpgradePair> pairs = upgrade.GetKeyValueMap();
        if ( pairs == null ) {
            return;
        }

        foreach ( UpgradeStatSO.UpgradePair pair in pairs ) {
            if ( pair.Type == UpgradeType.ExtraBalls ) {
                context.AddBall(pair.BallType, (int)pair.Value);
                continue;
            }

            context.ModifyStat(pair.Type, pair.Value);

            if ( TryGetProcessType(pair.Type, out ProcessType processType) ) {
                context.AddProcess(processType);
            }
        }
    }

    void ApplyBehaviorUpgrade( UpgradeBehaviorSO upgrade, IUpgradeContext context ) {
        IReadOnlyList<UpgradeBehaviourType> behaviorTypes = upgrade.GetBehaviorTypes();
        if ( behaviorTypes == null ) {
            return;
        }

        foreach ( UpgradeBehaviourType behaviorType in behaviorTypes ) {
            context.SetBehaviorActive(behaviorType);
        }
    }

    static bool TryGetProcessType( UpgradeType upgradeType, out ProcessType processType ) {
        processType = upgradeType switch {
            UpgradeType.CritChance => ProcessType.Crit,
            UpgradeType.ExplosionChance => ProcessType.Explosion,
            UpgradeType.LightningChance => ProcessType.Lightning,
            UpgradeType.PoisonChance => ProcessType.Poison,
            UpgradeType.FreezeChance => ProcessType.Freeze,
            UpgradeType.SniperInterval => ProcessType.Sniper,
            UpgradeType.ShockwaveChance => ProcessType.Shockwave,
            UpgradeType.RallyBonus => ProcessType.Rally,
            _ => default
        };

        return upgradeType is UpgradeType.CritChance
            or UpgradeType.ExplosionChance
            or UpgradeType.LightningChance
            or UpgradeType.PoisonChance
            or UpgradeType.FreezeChance
            or UpgradeType.SniperInterval
            or UpgradeType.ShockwaveChance
            or UpgradeType.RallyBonus;
    }
}
