using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MainMenuLifeTimeScope : LifetimeScope {

    protected override void Configure( IContainerBuilder builder) {
        //builder.Register<PlayerDataManager>(Lifetime.Singleton);

    }



}
