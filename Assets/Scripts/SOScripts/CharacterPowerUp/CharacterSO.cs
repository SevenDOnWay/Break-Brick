using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "CharacterSO", menuName = "ScriptableObjects/CharacterSO")]
public class CharacterSO : ScriptableObject {

    public string characterName;
    public Sprite icon;
    public string description;

}
