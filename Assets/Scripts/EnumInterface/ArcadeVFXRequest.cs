using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Stable identifiers for the portable arcade VFX library.</summary>
public enum ArcadeVFXId {
    Explosion, Lightning, FreezeApply, FreezeLoop, PoisonApply, PoisonLoop,
    PiercingImpact, HeavyImpact, Crit, Rally, Sniper, Shockwave,
    HorizontalBeam, VerticalBeam, HealPulse, GhostPhase, ShieldBlock,
    Split, PlusBall, Reinforce, FogPulse
}

/// <summary>
/// Presentation-only data. It intentionally contains no gameplay services or scene references.
/// Target positions are copied by the caller so a pooled effect can be replayed safely.
/// </summary>
public readonly struct ArcadeVFXRequest {
    public readonly ArcadeVFXId Id;
    public readonly Vector3 Position;
    public readonly Vector3 Direction;
    public readonly Vector3 EndPosition;
    public readonly float Radius;
    public readonly float Intensity;
    public readonly int Seed;
    public readonly IReadOnlyList<Vector3> TargetPositions;
    public readonly Transform FollowTarget;
    public readonly bool Loop;

    public ArcadeVFXRequest(
        ArcadeVFXId id, Vector3 position, Vector3 direction = default, Vector3 endPosition = default,
        float radius = 1f, float intensity = 1f, int seed = 0,
        IReadOnlyList<Vector3> targetPositions = null, Transform followTarget = null, bool loop = false) {
        Id = id;
        Position = position;
        Direction = direction;
        EndPosition = endPosition;
        Radius = Mathf.Max(0.01f, radius);
        Intensity = Mathf.Max(0.01f, intensity);
        Seed = seed;
        TargetPositions = targetPositions;
        FollowTarget = followTarget;
        Loop = loop;
    }

    public static ArcadeVFXRequest At(ArcadeVFXId id, Vector3 position, float radius = 1f, float intensity = 1f) =>
        new(id, position, radius: radius, intensity: intensity, seed: Environment.TickCount);
}

/// <summary>Decoupled bridge from gameplay to the portable presentation library.</summary>
public static class ArcadeVFXEvent {
    public static event Action<ArcadeVFXRequest> OnRequest;
    public static event Action<Transform> OnStopPersistent;
    public static void Raise(ArcadeVFXRequest request) => OnRequest?.Invoke(request);
    public static void StopPersistent(Transform followTarget) => OnStopPersistent?.Invoke(followTarget);
}
