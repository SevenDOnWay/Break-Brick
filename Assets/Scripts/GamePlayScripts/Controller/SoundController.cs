using System;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSO audioSO;
    private AudioSource audioSource;

    // Ensure that the AudioSO is assigned in the inspector or through code
    private void Awake()
    {
        if (audioSO == null)
        {
            Debug.LogError("AudioSO is not assigned in the SoundController.");
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        BrickScript.OnBrickDestroyed += PlayBrickDestroyedSound;
        BrickScript.OnBrickHit += PlayBrickHitSound;
    }

    private void PlayBrickHitSound(object sender, EventArgs e)
    {
        PlaySound(audioSO.brickHit, Camera.main.transform.position);
    }

    private void PlayBrickDestroyedSound(object sender, EventArgs e)
    {
        PlaySound(audioSO.brickDestroyed, Camera.main.transform.position);
    }

    private void PlaySound(AudioClip[] audioClips, Vector3 position, float volume = 1f)
    {
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("No audio clips available to play.");
            return;
        }
        int randomIndex = UnityEngine.Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[randomIndex], volume);
    }

}
