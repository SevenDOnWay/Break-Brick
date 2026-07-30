using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Test-scene-only bridge between the Spawn Boss UI button and SpawnController.
/// </summary>
[RequireComponent(typeof(Button))]
public sealed class BossSpawnTestButton : MonoBehaviour
{
    private Button button;
    private SpawnController spawnController;

private void Awake()
    {
        button = GetComponent<Button>();

        foreach (SpawnController controller in FindObjectsByType<SpawnController>(FindObjectsSortMode.None))
        {
            if (controller.gameObject.scene == gameObject.scene)
            {
                spawnController = controller;
                break;
            }
        }

        button.onClick.AddListener(SpawnBoss);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(SpawnBoss);
        }
    }

    private void SpawnBoss()
    {
        if (spawnController == null)
        {
            Debug.LogWarning("Boss spawn test button could not find a SpawnController.");
            return;
        }

        spawnController.SpawnBoss();
    }
}
