public interface IBehavior {
    UpgradeBehaviourType Type { get; }

    void Apply( IUpgradeContext context );

}
