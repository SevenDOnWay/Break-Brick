using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour {

    SelectPlayer selctPlayer;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject indexPanel;

    [SerializeField] GameObject dotPrefab;
    [SerializeField] Color activeColor = Color.white;
    [SerializeField] Color inactiveColor = Color.gray;

    [SerializeField]
    string[] descriptions = new string[] {
        "Start with 3 ball ",
        "idk yet"
    };

    List<Image> dots = new List<Image>();

    void Start() {
        selctPlayer = GetComponentInParent<SelectPlayer>();

        InitializeDot();

        UpdatePanel(selctPlayer.GetCurrentPlayerIndex());

        selctPlayer.OnCharacterChange += UpdatePanel;

    }

    void InitializeDot() {
        for ( int i = 0; i < selctPlayer.Characters.Length; i++ ) {
            GameObject dot = Instantiate(dotPrefab, indexPanel.transform);

            dot.transform.localPosition = Vector3.zero;
            dot.transform.localRotation = Quaternion.identity;
            dot.transform.localScale = Vector3.one;

            Image dotImage = dot.GetComponent<Image>();
            dotImage.color = inactiveColor;

            dot.transform.SetParent(indexPanel.transform, false);

            dots.Add(dotImage);
        }
    }


    void UpdatePanel( int index ) {
        SetDescription(index);
        SetActiveDot(index);

    }

    void SetDescription( int index ) {
        descriptionText.text = descriptions[index];
    }

    void SetActiveDot( int index ) {
        for ( int i = 0; i < dots.Count; i++ ) {
            dots[i].color = (i == index) ? activeColor : inactiveColor;
        }
    }



}
