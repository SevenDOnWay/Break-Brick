using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

public class MainMenuLifeTimeScope : LifetimeScope {

    protected override void Configure( IContainerBuilder builder) {
    }

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }


}
