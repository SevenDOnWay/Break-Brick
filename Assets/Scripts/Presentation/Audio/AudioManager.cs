using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of AudioManager found. Destroying the new one.");
        }

        Instance = this;
    }

    private void OnEnable() {
        GameplayEvents.OnBrickEvent += HandleBrickEvent;
    }

    private void OnDisable() {
        GameplayEvents.OnBrickEvent -= HandleBrickEvent;
    }

    private void HandleBrickEvent( BrickEventType eventType ) {
        if ( FModEvents.Instance == null || Camera.main == null ) {
            return;
        }

        switch ( eventType ) {
            case BrickEventType.Destroyed:
                PlayOneShot(FModEvents.Instance.BrickDestroyedSFX, Camera.main.transform.position);
                break;
            case BrickEventType.Hit:
                PlayOneShot(FModEvents.Instance.BrickHitSFX, Camera.main.transform.position);
                break;
        }
    }

    private void PlayOneShot(EventReference[] sounds, Vector3 position)
    {
        int randomIndex = UnityEngine.Random.Range(0, sounds.Length);
        PlayOneShot(sounds[randomIndex], position);
    }

    private void PlayOneShot(EventReference sound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }

    

}
