using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LevelUi : MonoBehaviour {
    LevelManager levelManager;


    [Header("References")]
    [SerializeField] TextMeshProUGUI lLevelText;
    [SerializeField] TextMeshProUGUI rLevelText;
    [SerializeField] Slider expSlider;
    CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] float visibleTime = 2f;

    Tween fadeTween;
    float hideAtTime;


    [Inject]
    void Constructor( LevelManager levelManager ) {
        this.levelManager = levelManager;
    }

    void Start() {
        SetUpObserver();

        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f; // Start invisible
        expSlider.minValue = 0;
        expSlider.maxValue = 1;

        UpdateLevelTexts(levelManager.CurrentLevel);
    }

    private void SetUpObserver() {
        levelManager.NotifiExpChanged += OnExpChanged;
        levelManager.NotifiLevelUp += OnLevelUp;    
        levelManager.NotifiShowExpUI += ShowExpPanel;
    }


    private void OnExpChanged( int level, float percent ) {
        expSlider.value = percent;
    }

    private void OnLevelUp( int newLevel ) {
        UpdateLevelTexts(newLevel);
        expSlider.value = 0f;
    }

    private void UpdateLevelTexts( int level ) {
        lLevelText.text = level.ToString();
        rLevelText.text = (level + 1).ToString();
    }

    private void ShowExpPanel() {
        fadeTween?.Kill(); // Stop existing fade out

        canvasGroup.alpha = 1f; // Make sure it's visible instantly

        // Create a sequence that waits, then fades out
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(visibleTime);
        seq.Append(canvasGroup.DOFade(0f, fadeDuration));

        fadeTween = seq;
    }


    //private void FadeInOut() {
    //    fadeTween?.Kill();

    //    Sequence seq = DOTween.Sequence();

    //    seq.Append(canvasGroup.DOFade(1f, fadeDuration))       // Fade In
    //       .AppendInterval(visibleTime)                       // Stay visible
    //       .Append(canvasGroup.DOFade(0f, fadeDuration))       // Fade out
    //       .OnComplete(() => {
    //           fadeTween = null;
    //       });

    //    fadeTween = seq;
    //}

}
