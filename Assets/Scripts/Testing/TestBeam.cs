using UnityEngine;

public class TestBeam : MonoBehaviour {

    [SerializeField] GameObject beam;
    Beam beamScript;

    [SerializeField] float testPosX;
    [SerializeField] float testPosY;

    public void OnClick() {
       
        Vector2 vector2 = new Vector2(testPosX, testPosY);

        BeamVFXCommand beamVFXCommand = new BeamVFXCommand(vector2);

        VFXEvent.RaiseVFXCommand(beamVFXCommand);

    }

}
