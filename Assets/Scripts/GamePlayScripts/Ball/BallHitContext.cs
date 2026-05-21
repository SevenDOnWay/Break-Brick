using UnityEngine;

public readonly struct BallHitContext {
    public readonly BallScript ball;
    public readonly BrickScript brick;
    public readonly Collision2D collision;
    public readonly StatManager statManager;
    public readonly int directDamage;
    public readonly Vector2 hitNormal;
    public readonly float squareSize;

    public BallHitContext(
        BallScript ball,
        BrickScript brick,
        Collision2D collision,
        StatManager statManager,
        int directDamage,
        Vector2 hitNormal,
        float squareSize
    ) {
        this.ball = ball;
        this.brick = brick;
        this.collision = collision;
        this.statManager = statManager;
        this.directDamage = directDamage;
        this.hitNormal = hitNormal;
        this.squareSize = squareSize;
    }

    public bool TryMarkEffectTriggered( BallHitEffect effect ) {
        return ball != null && ball.TryMarkSpecialEffect(effect, brick);
    }
}
