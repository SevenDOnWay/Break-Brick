using System.Threading.Tasks;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class QuitScript : MonoBehaviour {

    [Inject] BrickManager brickManager;
    [Inject] BallManager ballManager;
    [Inject] WaveScript waveScript;

    public void OnEnable() {
        Button temp = gameObject.GetComponent<Button>();
        
        temp.onClick.AddListener(QuitGame);
    }



    public async void QuitGame() {

        brickManager.SaveBrick();
        ballManager.Save();
        waveScript.SaveWaveIndex();
        await RunDataManager.Instance.Save();

        await SceneManager.LoadSceneAsync(0);
    }
}
