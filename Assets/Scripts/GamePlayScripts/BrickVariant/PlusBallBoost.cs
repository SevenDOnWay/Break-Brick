using UnityEngine;
using VContainer;

public class PlusBallBoost : MonoBehaviour, IBrickVariant
{
    [Inject] BallManager ballManager;
    public void OnHit(BrickScript brickScript)
    {

    }

    public void OnSpawn(BrickScript brickScript)
    {

    }
    public void OnEndTurn(BrickScript brickScript)
    {

    }
    public void OnDie(BrickScript brickScript)
    {
        ballManager.RequestExtraBall();
    }
}