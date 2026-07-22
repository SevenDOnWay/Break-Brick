using System;
using System.Collections;
using UnityEngine;

public class ExplosionVFXPlayer : VFXPlayerBase {

    [SerializeField] ParticleSystem particleSystem;

    Action onCompleteCallback;

    public override void Play( IVFXCommand command, Action onComplete ) {

        onCompleteCallback = onComplete;
        if ( command is not ExplosionVFXCommand explosionCmd ) {
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
