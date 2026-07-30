using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class Beam : MonoBehaviour {

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float beamSpeed;


    private float _posX;

    //private LayerMask layerMask = LayerMask.GetMask("Default");

    int completedCount = 0;



    private Material _mat;
    private Tween _leftTween, _rightTween;

    float squareSize;
    float totalWidth;
    float leftWall;
    float rightWall;

    public void Awake() {
        _mat = lineRenderer.material;
        CalculateBrickSize(Camera.main, 8, 10, (float)0.9);

        leftWall = squareSize * -4;
        rightWall = squareSize * 4;
    }
    void CalculateBrickSize( Camera camera, int column, int row, float padding ) {
        float worldHeight = camera.orthographicSize * 2f;
        float worldWidth = worldHeight * camera.aspect;

        //TODO: add method to Handle PC, and Ipad aspect ratio

        squareSize = (worldWidth * padding) / column;

        Debug.Log("square size: " + squareSize);

    }

    public void Play( Vector2 worldPos ) {
        //transform.position = worldPos;

        _posX = worldPos.x;
        SetupLine(worldPos);
        gameObject.SetActive(true);
    }

    void SetupLine( Vector2 worldPos ) {
        if ( !lineRenderer.useWorldSpace )
            lineRenderer.useWorldSpace = true;

        Vector3 leftP  = new Vector3(leftWall,  worldPos.y, 0f);
        Vector3 midP   = new Vector3(_posX,     worldPos.y, 0f);
        Vector3 rightP = new Vector3(rightWall, worldPos.y, 0f);

        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, leftP);
        lineRenderer.SetPosition(1, midP);
        lineRenderer.SetPosition(2, rightP);

        totalWidth = rightWall - leftWall;
    }

    private void OnEnable() {
        _leftTween?.Kill();
        _rightTween?.Kill();

        _mat.SetFloat("_LeftReveal", 0f);
        _mat.SetFloat("_RightReveal", 0f);

        float leftDist = Mathf.Abs(_posX - leftWall);
        float rightDist = Mathf.Abs(rightWall - _posX);
        //float maxDist = rightWall - leftWall;
        totalWidth = rightWall - leftWall;


        float leftNorm  = Mathf.Clamp01(leftDist / totalWidth);
        float rightNorm = Mathf.Clamp01(rightDist / totalWidth);

        float leftDuration = leftDist / beamSpeed;
        float rightDuration = rightDist / beamSpeed;

        _leftTween = DOTween
            .To(v => _mat.SetFloat("_LeftReveal", v), 0f, leftNorm, leftDuration)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .OnComplete(OnSideComplete);

        _rightTween = DOTween
            .To(v => _mat.SetFloat("_RightReveal", v), 0f, rightNorm, rightDuration)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .OnComplete(OnSideComplete);
    }


    void OnSideComplete() {
        completedCount++;
        if ( completedCount >= 2 ) {
            completedCount = 0;
            Finish();
        }
    }

    void Finish() {
        gameObject.SetActive(false);
    }

    void OnDisable() {
        _leftTween?.Kill();
        _rightTween?.Kill();
    }
}
