using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour {

    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject blockerPanel;

    public void OnEnable() {
        Button temp = gameObject.GetComponent<Button>();

        temp.onClick.AddListener(EnablePanel);

    }

    public void EnablePanel() {
        optionPanel.SetActive(true);
        blockerPanel.SetActive(true);
    }

}
