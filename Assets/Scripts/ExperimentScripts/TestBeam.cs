using UnityEngine;

public class TestBeam : MonoBehaviour {

    [SerializeField] GameObject beam;
    Beam beamScript;

    [SerializeField] float testPosX;
    [SerializeField] float testPosY;

    public void OnClick() {
       
        Vector2 vector2 = new Vector2(testPosX, testPosY);

        HorizontalBeamVFXCommand beamVFXCommand = new HorizontalBeamVFXCommand(vector2);

        VFXEvent.RaiseVFXCommand(beamVFXCommand);

    }

}
