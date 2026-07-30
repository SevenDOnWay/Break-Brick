using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TestLifeTimeScope : LifetimeScope {
    protected override void Configure( IContainerBuilder builder ) {
        builder.RegisterComponentInHierarchy<VFXManager>();
    }
}
