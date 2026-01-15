using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifeTimeScope : LifetimeScope{

    protected override void Configure( IContainerBuilder builder ) {
        //Data
        builder.RegisterComponentInHierarchy<PlayerDataManager>();
        builder.RegisterComponentInHierarchy<RunDataManager>();

        //Audio
        builder.RegisterComponentInHierarchy<AudioManager>();


    }

}
