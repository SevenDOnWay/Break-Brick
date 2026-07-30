using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A prefab-local, URP-safe player made only from ParticleSystem and LineRenderer.
/// Configure each prefab in the inspector; the catalog only selects and pools it.
/// </summary>
public sealed class ArcadeVFXPlayer : MonoBehaviour {
    public enum RenderMode { Burst, Line, Chain, Aura, Shockwave }

    [Header("Prefab-local art controls")]
    [SerializeField] RenderMode renderMode = RenderMode.Burst;
    [SerializeField] Color primaryColor = Color.white;
    [SerializeField] Color secondaryColor = Color.cyan;
    [SerializeField, Min(0.05f)] float duration = .45f;
    [SerializeField, Min(1)] int particleCount = 16;
    [SerializeField, Min(.01f)] float particleSize = .09f;
    [SerializeField, Min(.01f)] float lineWidth = .06f;
    [SerializeField, Min(.01f)] float intensity = 1f;
    [SerializeField] bool persistent;

    ParticleSystem particles;
    LineRenderer line;
    Coroutine returnRoutine;
    Action<ArcadeVFXPlayer> onComplete;
    Transform follow;
    ArcadeVFXRequest activeRequest;
    bool persistentActive;
    static Material sharedLineMaterial;

    void Awake() => EnsureRenderers();
    void OnDisable() { if (returnRoutine != null) StopCoroutine(returnRoutine); returnRoutine = null; follow = null; onComplete = null; persistentActive = false; }
    void LateUpdate() { if (follow != null) transform.position = follow.position; }

    public void Play(ArcadeVFXRequest request, Action<ArcadeVFXPlayer> completed) {
        EnsureRenderers();
        if (returnRoutine != null) StopCoroutine(returnRoutine);
        activeRequest = request;
        onComplete = completed;
        follow = request.FollowTarget;
        persistentActive = request.Loop || persistent;
        transform.position = follow != null ? follow.position : request.Position;
        gameObject.SetActive(true);

        float scale = request.Radius * request.Intensity * intensity;
        ConfigureParticles(scale, persistentActive);
        ConfigureLine(request, scale);
        particles.Play(true);
        if (!persistentActive) returnRoutine = StartCoroutine(ReturnAfter(duration));
    }

    public void StopPersistent() { if (persistentActive && returnRoutine == null) returnRoutine = StartCoroutine(ReturnAfter(0f)); }
    public bool Follows(Transform target) => persistentActive && follow == target;

    IEnumerator ReturnAfter(float delay) {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        line.enabled = false;
        persistentActive = false;
        Action<ArcadeVFXPlayer> callback = onComplete;
        onComplete = null;
        callback?.Invoke(this);
    }

    void EnsureRenderers() {
        if (particles == null) {
            particles = GetComponentInChildren<ParticleSystem>(true);
            if (particles == null) {
                GameObject child = new("Particles"); child.transform.SetParent(transform, false);
                particles = child.AddComponent<ParticleSystem>();
                var renderer = child.GetComponent<ParticleSystemRenderer>();
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }
            var main = particles.main; main.playOnAwake = false; main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = particles.emission; emission.enabled = false;
        }
        if (line == null) {
            line = GetComponentInChildren<LineRenderer>(true);
            if (line == null) { GameObject child = new("Line"); child.transform.SetParent(transform, false); line = child.AddComponent<LineRenderer>(); }
            line.useWorldSpace = true; line.numCapVertices = 2; line.numCornerVertices = 2;
            if (sharedLineMaterial == null) {
                Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
                sharedLineMaterial = new Material(shader) { hideFlags = HideFlags.DontSave };
            }
            line.sharedMaterial = sharedLineMaterial;
        }
    }

    void ConfigureParticles(float scale, bool loop) {
        var main = particles.main;
        main.loop = loop;
        main.duration = duration;
        main.startLifetime = loop ? 1.2f : Mathf.Max(.1f, duration * .8f);
        main.startSpeed = renderMode == RenderMode.Aura ? .08f : .8f * scale;
        main.startSize = particleSize * scale;
        main.startColor = new ParticleSystem.MinMaxGradient(primaryColor, secondaryColor);
        main.maxParticles = Mathf.Clamp(Mathf.CeilToInt(particleCount * Mathf.Max(.5f, scale)), 1, 96);
        var emission = particles.emission; emission.enabled = true; emission.rateOverTime = loop ? Mathf.Max(2, particleCount / 3) : 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)Mathf.Clamp(particleCount, 1, 96)) });
        var shape = particles.shape; shape.enabled = true; shape.shapeType = renderMode == RenderMode.Aura ? ParticleSystemShapeType.Circle : ParticleSystemShapeType.Sphere; shape.radius = .06f * scale;
        var color = particles.colorOverLifetime; color.enabled = true;
        Gradient gradient = new(); gradient.SetKeys(new[] { new GradientColorKey(primaryColor, 0), new GradientColorKey(secondaryColor, 1) }, new[] { new GradientAlphaKey(.95f, 0), new GradientAlphaKey(0f, 1) });
        color.color = gradient;
    }

    void ConfigureLine(ArcadeVFXRequest request, float scale) {
        bool lineMode = renderMode != RenderMode.Burst && renderMode != RenderMode.Aura;
        line.enabled = lineMode;
        if (!lineMode) return;
        line.startWidth = lineWidth * scale; line.endWidth = lineWidth * scale * .35f;
        line.startColor = primaryColor; line.endColor = secondaryColor;
        List<Vector3> points = new() { transform.position };
        if (renderMode == RenderMode.Chain && request.TargetPositions != null && request.TargetPositions.Count > 0) {
            for (int i = 0; i < request.TargetPositions.Count; i++) points.Add(request.TargetPositions[i]);
        } else {
            Vector3 end = request.EndPosition;
            if (end == Vector3.zero) end = transform.position + (request.Direction.sqrMagnitude > .001f ? request.Direction.normalized : Vector3.right) * scale * 2f;
            if (renderMode == RenderMode.Shockwave) {
                points.Add(transform.position + Vector3.right * scale); points.Add(transform.position + Vector3.up * scale);
                points.Add(transform.position + Vector3.left * scale); points.Add(transform.position + Vector3.down * scale);
            } else points.Add(end);
        }
        line.positionCount = points.Count; line.SetPositions(points.ToArray());
    }
}
