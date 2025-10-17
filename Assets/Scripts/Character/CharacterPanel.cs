using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using VContainer;

public class CharacterPanel : MonoBehaviour {

    //[Inject] CharacterEntry characterEntry;
    [Inject] CharacterManager characterManager;
    [Inject] SelectCharacter selectCharacter;

    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject indexPanel;

    [SerializeField] GameObject dotPrefab;
    [SerializeField] Color activeColor = Color.white;
    [SerializeField] Color inactiveColor = Color.gray;

    [SerializeField] List<CharacterSO> characterSOs;

    List<Image> dots;
    List<GameObject> chracters;

    async void Start() {

        //if ( selectCharacter == null ) {
        //    Debug.LogError("SelectCharacter component not found");
        //    return;
        //}

        characterSOs = await characterManager.GetCharacters();


        dots = new List<Image>(characterSOs.Count);
        chracters = new List<GameObject>(characterSOs.Count);

        InitializePanel();

        UpdatePanel(selectCharacter.GetCurrentPlayerIndex());

        selectCharacter.OnCharacterChange += UpdatePanel;

    }

    // Initialize dots based on the number of characters
    void InitializePanel() {
        for ( int i = 0; i < characterSOs.Count; i++ ) {
            CreateDot();
            CreateCharacter(characterSOs[i]);
        }
    }

    void CreateCharacter( CharacterSO character ) {
        GameObject temp = new GameObject(character.GetCharacterName());
        temp.AddComponent<SpriteRenderer>();
        temp.GetComponent<SpriteRenderer>().sprite = character.GetIcon();

        chracters.Add(temp);
    }

    void CreateDot() {
        GameObject dot = Instantiate(dotPrefab, indexPanel.transform);

        dot.transform.localPosition = Vector3.zero;
        dot.transform.localRotation = Quaternion.identity;
        dot.transform.localScale = Vector3.one;

        Image dotImage = dot.GetComponent<Image>();
        dotImage.color = inactiveColor;

        dot.transform.SetParent(indexPanel.transform, false);

        dots.Add(dotImage);
    }


    void UpdatePanel( int index ) {
        SetCharacter(index);
        SetDescription(index);
        SetActiveDot(index);
    }

    //TODO: add character display logic
    void SetCharacter( int index ) {

    }

    void SetDescription( int index ) {
        descriptionText.text = characterSOs[index].GetUpgrade().GetDescription();
    }

    void SetActiveDot( int index ) {
        for ( int i = 0; i < dots.Count; i++ ) {
            dots[i].color = (i == index) ? activeColor : inactiveColor;
        }
    }

}
