using UnityEngine;

/// <summary>
/// Presentation-ready projection of an upgrade definition.
/// </summary>
public readonly struct UpgradeOffer {
    public readonly string Id;
    public readonly Sprite Icon;
    public readonly string Name;
    public readonly string Description;

    public UpgradeOffer( string id, Sprite icon, string name, string description ) {
        Id = id;
        Icon = icon;
        Name = name;
        Description = description;
    }
}
