using System;
using System.Collections;
using UnityEngine;

public class ExplosionVFXPlayer : VFXPlayerBase{

    public override VFXType GetVFXType() => VFXType.Explosion;
    [SerializeField] ParticleSystem particleSystem;

    Action onCompleteCallback;

    public override void PlayExplosion( IVFXCommand cmd, Action onComplete ) {

        onCompleteCallback = onComplete;
        if ( cmd is not ExplosionVFXCommand explosionCmd ) {
            Debug.LogError("Wrong command type passed to ExplosionVFXPlayer");
            onComplete?.Invoke();
            return;
        }

        transform.position = explosionCmd.position;

        var shape = particleSystem.shape;
        shape.radius = explosionCmd.radius;

        particleSystem.Play();
    }

    private void OnParticleSystemStopped() {
        onCompleteCallback?.Invoke();
    }
}
