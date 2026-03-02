using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class QuitScript : MonoBehaviour {

    RunDataManager runDataManager;
    BrickManager brickManager;
    BallManager ballManager;
    WaveScript waveScript;


    [Inject]
    void Constructor(
        RunDataManager runDataManager,
        BrickManager brickManager,
        BallManager ballManager,
        WaveScript waveScript
     ) {
        this.runDataManager = runDataManager;
        this.brickManager = brickManager;
        this.ballManager = ballManager;
        this.waveScript = waveScript;
    }

    public void OnEnable() {
        Button temp = gameObject.GetComponent<Button>();
        
        temp.onClick.AddListener(QuitGame);
    }



    public async void QuitGame() {

        brickManager.SaveBrick();
        ballManager.Save();
        waveScript.SaveWaveIndex();
        await runDataManager.Save();

        await SceneManager.LoadSceneAsync(0);
    }
}
