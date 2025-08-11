using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope {
    protected override void Configure( IContainerBuilder builder ) {
        builder.RegisterInstance(Camera.main);
        builder.Register<PlayScreen>(Lifetime.Singleton)
            .AsSelf()
            .WithParameter("column", 8)
            .WithParameter("row", 10)
            .WithParameter("padding", 0.9f);

        builder.RegisterComponentInHierarchy<PlayerController>();
        builder.RegisterComponentInHierarchy<SpawnController>();
        builder.RegisterComponentInHierarchy<BallController>();
    }
}
