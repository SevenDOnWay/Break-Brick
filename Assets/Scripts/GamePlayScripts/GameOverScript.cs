using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameOverScript : MonoBehaviour {

    WaveScript waveScript;
    LevelManager levelManager;

    [SerializeField] Button GameOverButton;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject blockerGameOverPanel;

    private TaskCompletionSource<bool> tcs;

    [Inject]
    public void Constructor(
        WaveScript waveScript,
        LevelManager levelManager 
        ) {
        this.waveScript = waveScript;
        this.levelManager = levelManager;
    }

    private void Start() {
        GameOverButton.onClick.AddListener(OnGameOverButtonClicked);
    }

    public Task HandleGameOver() {
        blockerGameOverPanel.SetActive(true);

        //TODO: missmatch between getter of wave index and level index, need to fix this

        waveText.text = waveScript.GetWaveIndex().ToString();
        levelText.text = levelManager.CurrentLevel.ToString();

        tcs = new TaskCompletionSource<bool>();

        return tcs.Task;   // Caller will await this
    }

    private void OnGameOverButtonClicked() {
        tcs?.TrySetResult(true);
    }
}
