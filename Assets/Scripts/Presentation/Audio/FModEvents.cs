using UnityEngine;
using FMODUnity;

public class FModEvents : MonoBehaviour
{
    public static FModEvents Instance { get; private set; }

    [field: Header("Brick Destroyed SFX")]
    [field: SerializeField] public EventReference[] BrickDestroyedSFX { get; private set; }

    [field: Header("Brick Hit SFX")]
    [field: SerializeField] public EventReference[] BrickHitSFX { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple instances of FModEvents found");
        }
        Instance = this;
    }
}
