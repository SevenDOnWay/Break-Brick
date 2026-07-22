using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LevelUi : MonoBehaviour {
    ILevelProgressSource levelProgressSource;


    [Header("References")]
    [SerializeField] TextMeshProUGUI lLevelText;
    [SerializeField] TextMeshProUGUI rLevelText;
    [SerializeField] Slider expSlider;
    CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] float visibleTime = 2f;

    Coroutine fadeRoutine;


    [Inject]
    void Constructor( ILevelProgressSource levelProgressSource ) {
        this.levelProgressSource = levelProgressSource;
    }

    void Start() {
        SetUpObserver();

        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f; // Start invisible
        expSlider.minValue = 0;
        expSlider.maxValue = 1;

        UpdateLevelTexts(levelProgressSource.CurrentLevel);
    }

    private void SetUpObserver() {
        levelProgressSource.ExperienceChanged += OnExpChanged;
        levelProgressSource.LevelChanged += OnLevelUp;
        levelProgressSource.ExperiencePanelRequested += ShowExpPanel;
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
        if (fadeRoutine != null) {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeOutAfterDelay());
    }

    IEnumerator FadeOutAfterDelay() {
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(visibleTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        fadeRoutine = null;
    }

}
