using UnityEngine;

public class OnClickOutSide : MonoBehaviour {

    [SerializeField] GameObject panelToClose;

    public void OnClick() {
        gameObject.SetActive(false);
    }


}
