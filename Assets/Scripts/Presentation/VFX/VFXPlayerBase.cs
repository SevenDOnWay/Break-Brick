using System;
using UnityEngine;

public abstract class VFXPlayerBase : MonoBehaviour {
    public abstract void Play( IVFXCommand command, Action onComplete );
}
