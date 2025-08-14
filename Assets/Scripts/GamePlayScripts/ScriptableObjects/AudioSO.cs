using UnityEngine;

[CreateAssetMenu(menuName = "SO/Audio Scriptable Object")]
public class AudioSO : ScriptableObject
{
    public AudioClip[] brickDestroyed;
    public AudioClip[] brickHit;
}
