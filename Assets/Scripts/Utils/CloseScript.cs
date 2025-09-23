using UnityEngine;
using UnityEngine.UI;

public class CloseScript : MonoBehaviour {

    [SerializeField] GameObject optionPanel;

    public void OnEnable() {
        Button temp = gameObject.GetComponent<Button>();
        temp.onClick.AddListener(ClosePanel);
    }

    public void ClosePanel() {
        optionPanel.SetActive(false);
    }

}
