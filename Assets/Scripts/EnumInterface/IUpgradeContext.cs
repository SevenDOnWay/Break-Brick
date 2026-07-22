/// <summary>
/// Runtime operations that may be requested by an upgrade definition or
/// behavior. The contract belongs to Core so data never depends on concrete
/// managers.
/// </summary>
public interface IUpgradeContext {
    void ModifyStat( UpgradeType type, float value );
    void AddBall( BallType type, int amount );
    void AddProcess( ProcessType type );
    void SetBehaviorActive( UpgradeBehaviourType type, bool active = true );
}
