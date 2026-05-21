using UnityEngine;

[CreateAssetMenu(fileName = "SpecialBallConfig", menuName = "Break Brick/Ball/Special Ball Config")]
public class SpecialBallConfig : ScriptableObject {
    [SerializeField] BallType ballType = BallType.Normal;
    [SerializeField] Color ballColor = Color.white;
    [SerializeField, Min(0.01f)] float speedMultiplier = 1f;
    [SerializeField, Min(0f)] float directDamageMultiplier = 1f;
    [SerializeField, Min(0)] int directDamageBonus = 0;
    [SerializeField, Min(0)] int pierceLimit = 0;
    [SerializeField] BallHitEffect[] hitEffects;

    public BallType BallType => ballType;
    public float SpeedMultiplier => speedMultiplier;
    public int PierceLimit => ballType == BallType.Piercing ? pierceLimit : 0;

    public int GetDirectDamage( int baseDamage ) {
        return Mathf.Max(0, Mathf.RoundToInt(baseDamage * directDamageMultiplier) + directDamageBonus);
    }

    public void ApplyVisuals( BallScript ball ) {
        if ( ball == null || ballType == BallType.Normal ) {
            return;
        }

        ball.ApplySpecialBallColor(ballColor);
    }

    public void ApplyHitEffects( BallHitContext context ) {
        if ( hitEffects == null ) {
            return;
        }

        foreach ( var effect in hitEffects ) {
            if ( effect == null ) continue;
            effect.Apply(context);
        }
    }
}
