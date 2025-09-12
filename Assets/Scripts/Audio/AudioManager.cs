using UnityEngine;
using FMODUnity;
using System;

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

    private void Start()
    {
        BrickScript.OnBrickDestroyed += PlayBrickDestroyedSound;
        BrickScript.OnBrickHit += PlayBrickHitSound;
    }


    private void PlayBrickDestroyedSound(object sender, EventArgs e)
    {
        PlayOneShot(FModEvents.Instance.BrickDestroyedSFX, Camera.main.transform.position);
    }
    private void PlayBrickHitSound(object sender, EventArgs e)
    {
        PlayOneShot(FModEvents.Instance.BrickHitSFX, Camera.main.transform.position);
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
