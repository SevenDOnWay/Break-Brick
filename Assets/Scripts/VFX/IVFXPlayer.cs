using System;
using UnityEngine;

public interface IVFXPlayer {
    public void Execute( IVFXCommand cmd, Action OnComplete);
}
