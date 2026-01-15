using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CharacterLifetimeScope : LifetimeScope {
    protected override void Configure( IContainerBuilder builder ) {
        builder.RegisterComponentInHierarchy<CharacterDataBase>();

        builder.RegisterComponentInHierarchy<SelectCharacter>();
        builder.RegisterComponentInHierarchy<CharacterPanel>();
        builder.RegisterComponentInHierarchy<DifficultPanel>();


    }
}
