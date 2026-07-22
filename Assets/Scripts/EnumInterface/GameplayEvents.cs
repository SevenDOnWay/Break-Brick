using System;

/// <summary>
/// Cross-boundary game events. Gameplay publishes semantic events; optional
/// presentation systems subscribe without taking gameplay dependencies.
/// </summary>
public static class GameplayEvents {
    public static event Action<BrickEventType> OnBrickEvent;

    public static void RaiseBrickEvent( BrickEventType eventType ) {
        OnBrickEvent?.Invoke(eventType);
    }
}
