using TMPro;
using UnityEngine;

public class WaveScript : MonoBehaviour {

    [SerializeField] TextMeshProUGUI t_waveIndex;

    int waveIndex;

    private void Awake() {
        waveIndex = 0;
        t_waveIndex.text = waveIndex.ToString();
    }

    public int GetWaveIndex() => waveIndex;

    public void IncreaseWave() {
        waveIndex++;
        t_waveIndex.text = waveIndex.ToString();
    }

    public void OnDestroy() {
        waveIndex = 0;
    }

}
