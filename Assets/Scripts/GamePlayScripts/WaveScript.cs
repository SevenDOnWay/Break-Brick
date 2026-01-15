using TMPro;
using UnityEngine;
using VContainer;

public class WaveScript : MonoBehaviour {
    RunDataManager runDataManager;

    [SerializeField] TextMeshProUGUI t_waveIndex;

    int waveIndex;

    [Inject]
    void Constructor( RunDataManager runDataManager ) {
        this.runDataManager = runDataManager;
    }

    private void Awake() {
        waveIndex = 0;
        t_waveIndex.text = waveIndex.ToString();
    }

    public int GetWaveIndex() => waveIndex;

    public void IncreaseWave() {
        waveIndex++;
        t_waveIndex.text = waveIndex.ToString();
    }

    public void SetWave( int waveIndex ) {
        t_waveIndex.text = waveIndex.ToString();
    }

    public void OnDestroy() {
        waveIndex = 0;
    }

    public void SaveWaveIndex() {
        runDataManager.runData.OverwriteWaveIndex(waveIndex);
    }

}
