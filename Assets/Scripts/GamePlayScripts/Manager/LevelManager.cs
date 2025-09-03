using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    [SerializeField] Slider levelSlider;
    [SerializeField] AnimationCurve levelProgession;

    int currentLevel;
    int currentExp;

    public event Action OnLevelUp;

    public void AddExp(int exp) {
        if( currentExp + exp < levelProgession.Evaluate(currentLevel) ) {
            currentExp += exp;
            levelSlider.value = (float)currentExp / levelProgession.Evaluate(currentLevel); 
        }
        else {
            
            exp -= (int)(levelProgession.Evaluate(currentLevel) - currentExp);

            currentExp = 0;
            currentLevel++;

            //TODO: Level up 2 time lead to bug, need to fix

            LevelUp();

            AddExp(exp); // Recursively add the remaining exp after leveling up
        }


    }

    public void LevelUp() {
        //TODO: Add level up logic here ball +1, and +1 upgrade.

        OnLevelUp?.Invoke();

        Debug.Log($"Level Up! New Level: {currentLevel}");
    }


}
