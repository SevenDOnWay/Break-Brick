using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour {

    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider SFXSlider;

    public void OnClickClose() {
        gameObject.SetActive(false);
    }

    //TODO : Implement saving and loading of settings, audio
}
