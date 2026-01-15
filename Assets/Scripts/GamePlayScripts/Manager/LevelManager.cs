using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {


    [SerializeField] AnimationCurve levelProgession;
    [SerializeField] GameObject expPanel;

    TextMeshProUGUI lLevelText;
    TextMeshProUGUI rLevelText;
    Slider levelSlider;
    CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] float visibleTime = 2f;

    //serialized fields for testing in runtime 

    int currentLevel;
    int currentExp;
    bool isShowingExp;
    float hideAtTime;


    Coroutine fadeCoroutine;

    public event Action OnLevelUp;

    private void Start() {
        lLevelText = expPanel.transform.Find("t_LeftLevel").GetComponent<TextMeshProUGUI>();
        rLevelText = expPanel.transform.Find("t_RightLevel").GetComponent<TextMeshProUGUI>();
        levelSlider = expPanel.transform.Find("LevelSlider").GetComponent<Slider>();
        canvasGroup = expPanel.GetComponent<CanvasGroup>();

        if ( canvasGroup == null || lLevelText == null || rLevelText == null) {
            Debug.Log("something null");
        }

        currentLevel = 1;
        lLevelText.text = currentLevel.ToString();
        rLevelText.text = (currentLevel + 1).ToString();

        canvasGroup.alpha = 0f; // Start invisible

    }

    public void AddExp( int exp ) {

        hideAtTime = Time.time + visibleTime;

        if ( fadeCoroutine == null ) fadeCoroutine = StartCoroutine(FadeInOut());


        if ( currentExp + exp < levelProgession.Evaluate(currentLevel) ) {
            currentExp += exp;
            levelSlider.value = (float)currentExp / levelProgession.Evaluate(currentLevel);
        }
        else {
            
            exp -= (int)(levelProgession.Evaluate(currentLevel) - currentExp);

            currentExp = 0;
            currentLevel++;

            LevelUp();

            AddExp(exp); // Recursively add the remaining exp after leveling up
        }


    }

    public void LevelUp() {
        //TODO: Add level up logic here ball +1, and +1 upgrade.

        OnLevelUp?.Invoke();
        UpdaateLevel();

        Debug.Log($"Level Up! New Level: {currentLevel}");
    }

    void UpdaateLevel() {
        currentExp++;

        lLevelText.text = currentLevel.ToString();
        rLevelText.text = (currentLevel + 1).ToString();
        levelSlider.value = 0f;
    }

    #region Fade_logic
    private IEnumerator FadeInOut() {
        // Fade In
        isShowingExp = true;
        yield return StartCoroutine(Fade(0f, 1f));

        // Stay visible
        while ( Time.time < hideAtTime ) {
            yield return null;
        }

        isShowingExp = false;
        // Fade Out
        yield return StartCoroutine(Fade(1f, 0f));

        fadeCoroutine = null;
    }



    private IEnumerator Fade( float from, float to ) {
        float elapsed = 0f;
        while ( elapsed < fadeDuration ) {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    #endregion

}
