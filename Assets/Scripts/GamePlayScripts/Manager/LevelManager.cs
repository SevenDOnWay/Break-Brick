using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour, ILevelProgressSource {


    [SerializeField] AnimationCurve levelProgression;

    public int CurrentLevel { get; private set; } = 1;
    public float CurrentExp { get; private set; } = 0;


    [SerializeField] GameObject expPanel;

    public event Action<int, float> NotifiExpChanged; 
    public event Action<int> NotifiLevelUp;           
    public event Action NotifiShowExpUI;

    public event Action<int, float> ExperienceChanged {
        add => NotifiExpChanged += value;
        remove => NotifiExpChanged -= value;
    }

    public event Action<int> LevelChanged {
        add => NotifiLevelUp += value;
        remove => NotifiLevelUp -= value;
    }

    public event Action ExperiencePanelRequested {
        add => NotifiShowExpUI += value;
        remove => NotifiShowExpUI -= value;
    }

    //serialized fields for testing in runtime 

    public void AddExp( int exp ) {

        NotifiShowExpUI?.Invoke();

        float required = levelProgression.Evaluate(CurrentLevel);

        if ( CurrentExp + exp < required ) {
            CurrentExp += exp;

            NotifiExpChanged?.Invoke(CurrentLevel, CurrentExp / required);

            return;
        }
        else {

            exp -= Mathf.RoundToInt(required - CurrentExp);
            CurrentLevel++;
            CurrentExp = 0;

            LevelUp();

            NotifiExpChanged?.Invoke(CurrentLevel, 0);

            AddExp(exp); // Recursively add the remaining exp after leveling up
        }

    }

    public void LevelUp() {

        NotifiLevelUp?.Invoke(CurrentLevel);

        Debug.Log($"Level Up! New Level: {CurrentLevel}");
    }
}
