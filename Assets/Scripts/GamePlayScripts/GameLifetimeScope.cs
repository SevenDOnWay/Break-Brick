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

        builder.RegisterComponentInHierarchy<SpawnController>();
        builder.RegisterComponentInHierarchy<PlayerController>();

        builder.RegisterComponentInHierarchy<BrickManager>();
        builder.RegisterComponentInHierarchy<BallManager>();

        builder.Register<BallScript>(Lifetime.Transient);
    }
}
