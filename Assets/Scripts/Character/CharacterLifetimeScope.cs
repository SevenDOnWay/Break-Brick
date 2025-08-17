using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CharacterLifetimeScope : LifetimeScope {
    protected override void Configure( IContainerBuilder builder ) {
        builder.RegisterComponentInHierarchy<CharacterEntry>();
        builder.RegisterComponentInHierarchy<SelectCharacter>();
        builder.RegisterComponentInHierarchy<DescriptionPanel>();
        builder.RegisterComponentInHierarchy<DifficultPanel>();
        builder.RegisterComponentInHierarchy<CharacterController>();
        builder.RegisterComponentInHierarchy<SelectState>();
    }
}
