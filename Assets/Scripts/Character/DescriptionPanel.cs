using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class DescriptionPanel : MonoBehaviour {

    [Inject] CharacterEntry characterEntry;
    [Inject] SelectCharacter selectCharacter;

    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject indexPanel;

    [SerializeField] GameObject dotPrefab;
    [SerializeField] Color activeColor = Color.white;
    [SerializeField] Color inactiveColor = Color.gray;

    List<Image> dots = new List<Image>();

    void Start() {

        if ( selectCharacter == null ) {
            Debug.LogError("SelectCharacter component not found");
            return;
        }

        InitializeDot();

        UpdatePanel(selectCharacter.GetCurrentPlayerIndex());

        selectCharacter.OnCharacterChange += UpdatePanel;

    }

    void InitializeDot() {
        for ( int i = 0; i < characterEntry.characters.Length; i++ ) {
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
        descriptionText.text = characterEntry.characters[index].description;
    }

    void SetActiveDot( int index ) {
        for ( int i = 0; i < dots.Count; i++ ) {
            dots[i].color = (i == index) ? activeColor : inactiveColor;
        }
    }



}
