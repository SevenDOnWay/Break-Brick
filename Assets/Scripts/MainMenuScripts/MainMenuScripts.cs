using JetBrains.Annotations;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class MainMenuScripts : MonoBehaviour {

    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject continueButton;

    public void Start() {
        optionPanel.SetActive(false);

        // Check if there is saved data to enable/disable the Continue button
        if ( RunDataManager.Instance != null && RunDataManager.Instance.runData != null ) {
            continueButton.SetActive(true);
        }
        else {
            continueButton.SetActive(false);
        }
    }

    public async void OnClickNewGame() {

        if ( RunDataManager.Instance.runData != null ) {
            RunDataManager.Instance.ClearRun();
            await RunDataManager.Instance.Save();
        }
            await SceneManager.LoadSceneAsync(1);
    }



    public async void OnClickContinue() {

        if ( RunDataManager.Instance.runData != null ) {
            RunDataManager.Instance.runData.isContinuing = true;
            await RunDataManager.Instance.Save();
            await SceneManager.LoadSceneAsync(2);
        }
        else {
            Debug.Log("loi cai doe j roi ");
        }
    }

    public void OnClickOption() {
        optionPanel.SetActive(true);
    }

    public void OnClickExtra() {

    }

}
