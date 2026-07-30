using System;
using System.Collections.Generic;

/// <summary>
/// The concrete gameplay result of selecting an upgrade.
/// UpgradeManager creates this value; BallManager owns the ball-specific work.
/// </summary>
public sealed class UpgradeEffect {
    public static readonly UpgradeEffect Empty = new UpgradeEffect(
        Array.Empty<StatChange>(),
        Array.Empty<ProcessType>(),
        Array.Empty<BallGrant>()
    );

    public IReadOnlyList<StatChange> StatChanges { get; }
    public IReadOnlyList<ProcessType> ProcessTypes { get; }
    public IReadOnlyList<BallGrant> BallGrants { get; }

    public UpgradeEffect(
        IReadOnlyList<StatChange> statChanges,
        IReadOnlyList<ProcessType> processTypes,
        IReadOnlyList<BallGrant> ballGrants
    ) {
        StatChanges = statChanges ?? Array.Empty<StatChange>();
        ProcessTypes = processTypes ?? Array.Empty<ProcessType>();
        BallGrants = ballGrants ?? Array.Empty<BallGrant>();
    }
}

public readonly struct StatChange {
    public UpgradeType Type { get; }
    public float Value { get; }

    public StatChange( UpgradeType type, float value ) {
        Type = type;
        Value = value;
    }
}

public readonly struct BallGrant {
    public BallType BallType { get; }
    public int Count { get; }

    public BallGrant( BallType ballType, int count ) {
        BallType = ballType;
        Count = count;
    }
}
