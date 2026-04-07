using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope {
    protected override void Configure( IContainerBuilder builder ) {
        builder.RegisterInstance(Camera.main);
        builder.Register<PlayScreen>(Lifetime.Singleton)
            .AsSelf()
            .WithParameter("camera", Camera.main)
            .WithParameter("column", 8)
            .WithParameter("row", 10)
            .WithParameter("padding", 0.9f);

        builder.RegisterComponentInHierarchy<WaveScript>();
        builder.RegisterComponentInHierarchy<GameOverScript>();

        builder.RegisterComponentInHierarchy<GameStateManager>();


        builder.RegisterComponentInHierarchy<SpawnController>();
        builder.RegisterComponentInHierarchy<PlayerController>();

        builder.Register<StatManager>(Lifetime.Singleton);
        builder.Register<UpgradeManager>(Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<BrickManager>();
        builder.RegisterComponentInHierarchy<BallManager>();
        builder.RegisterComponentInHierarchy<LevelManager>();
        builder.RegisterComponentInHierarchy<VFXManager>();

        builder.RegisterComponentInHierarchy<LevelUi>();
        builder.RegisterComponentInHierarchy<UpgradeUI>();

        builder.RegisterComponentInHierarchy<QuitScript>();


        builder.Register<BallScript>(Lifetime.Transient);
        builder.Register<BrickScript>(Lifetime.Transient);

    }
}
