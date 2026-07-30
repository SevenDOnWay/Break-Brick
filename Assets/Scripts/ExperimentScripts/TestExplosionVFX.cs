using UnityEngine;
using UnityEngine.UI;

public class TestExplosionVFX : MonoBehaviour {
    [Header("Test Settings")]
    [SerializeField] private VFXType testType = VFXType.Explosion;
    [SerializeField] private float testRadius = 1f;
    public void TriggerTestVFX() {
        Vector3 randomPos = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1f, 1f));

        IVFXCommand testCmd = new ExplosionVFXCommand( randomPos, testRadius);
        VFXEvent.RaiseVFXCommand(testCmd);
    }


}
