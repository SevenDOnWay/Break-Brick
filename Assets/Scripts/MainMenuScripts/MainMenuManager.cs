using JetBrains.Annotations;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class MainMenuManager : MonoBehaviour{

    RunDataManager runDataManager;

    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject extraButton;
    [SerializeField] GameObject warningPanel;

    WarningScript warningScript;


    [Inject]
    public void Constructor( RunDataManager runDataManager ) {
        this.runDataManager = runDataManager;
    }

    void Start() {
        optionPanel.SetActive(false);
        warningPanel.SetActive(false);
        warningScript = warningPanel.GetComponent<WarningScript>();



        if ( runDataManager != null && runDataManager.runData != null ) {
            continueButton.SetActive(true);
        }
        else {
            continueButton.SetActive(false);
        }
    }

    public async void OnClickNewGame() {
        if ( runDataManager == null ) Debug.Log("runDataManager is null");
        if ( runDataManager.runData != null ) {

            Task<bool> warningTask = warningScript.WaitForUser();
            bool confirmed = await warningTask;

            if ( confirmed ) runDataManager.DeleteRun();
            else return;
        }

        await SceneManager.LoadSceneAsync(1); // Load Character Selection Scene
    }

    public async void OnClickContinue() {
        if ( runDataManager.runData != null ) {
            runDataManager.runData.SetIsContinuing(true);
            await SceneManager.LoadSceneAsync(2); // Load Gameplay Scene
        }
    }

    public void OnClickOption() {
        optionPanel.SetActive(true);
    }

}
