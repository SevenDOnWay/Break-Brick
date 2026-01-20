using UnityEngine;

public class TestBeam : MonoBehaviour {

    [SerializeField] GameObject beam;
    Beam beamScript;

    [SerializeField] float testPosX;
    [SerializeField] float testPosY;

    public void OnClick() {
        beamScript = beam.GetComponent<Beam>();

        beamScript.Play(new Vector2(testPosX, testPosY));



    }

}
