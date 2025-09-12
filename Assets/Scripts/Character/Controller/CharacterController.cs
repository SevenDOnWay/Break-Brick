using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

public class CharacterController : MonoBehaviour {

    [Inject] CharacterEntry characterEntry;

    List<GameObject> characters = new List<GameObject>();

    private void Awake() {
        InitializeCharacter();

    }


    void InitializeCharacter() {
        foreach(var character in characterEntry.characters ) {
            GameObject temp = new GameObject(character.characterName);
            temp.AddComponent<SpriteRenderer>();
            temp.GetComponent<SpriteRenderer>().sprite = character.icon;

            characters.Add(temp);
        }
    }
}
