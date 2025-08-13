using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScripts : MonoBehaviour {

    [SerializeField] GameObject optionPanel;

    public void OnClickNewGame() {
        SceneManager.LoadScene(1);
    }

    public void OnClickContinue() {
        //TOTO: implement playerdata first
    }

    public void OnClickOption() {
        optionPanel.SetActive(true);
    }

    public void OnClickExtra() {
        
    }

}
