using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Pooled, URP/WebGL-safe renderer for LightningVFXCommand.
/// Generates a short-lived two-layer bolt without per-frame allocations.
/// </summary>
public sealed class LightningVFXPlayer : VFXPlayerBase {
    [Header("Appearance")]
    [SerializeField] Color coreColor = new(0.93f, 0.98f, 1f, 1f);
    [SerializeField] Color glowColor = new(0.12f, 0.62f, 1f, 0.35f);
    [SerializeField, Min(0.01f)] float coreWidth = 0.055f;
    [SerializeField, Min(0.01f)] float glowWidth = 0.16f;
    [SerializeField, Range(6, 24)] int maxSegments = 16;
    [SerializeField, Range(0f, 0.5f)] float jitter = 0.12f;
    [SerializeField, Min(0.03f)] float duration = 0.16f;
    [SerializeField, Min(0)] int endpointSparkCount = 10;
    [SerializeField] Material lineMaterial;

    LineRenderer coreLine;
    LineRenderer glowLine;
    ParticleSystem sparks;
    Vector3[] points;
    Coroutine playback;
    Action onComplete;

    static Material sharedLineMaterial;

    void Awake() {
        coreLine = CreateLine("Core", coreWidth, coreColor, 60);
        glowLine = CreateLine("Glow", glowWidth, glowColor, 59);
        sparks = CreateSparks();
        points = new Vector3[maxSegments + 1];
    }

    void OnDisable() {
        if (playback != null) {
            StopCoroutine(playback);
            playback = null;
        }
    }

    public override void Play(IVFXCommand command, Action complete) {
        if (command is not LightningVFXCommand lightning) {
            Debug.LogWarning($"LightningVFXPlayer cannot render {command?.GetType().Name}.");
            complete?.Invoke();
            return;
        }

        onComplete = complete;
        if (playback != null) StopCoroutine(playback);
        playback = StartCoroutine(PlayRoutine(lightning.GetStartPos(), lightning.GetEndPos()));
    }

    IEnumerator PlayRoutine(Vector2 start, Vector2 end) {
        float elapsed = 0f;
        int seed = Hash(start, end);

        while (elapsed < duration) {
            BuildBolt(start, end, seed++);
            float alpha = 1f - Mathf.Clamp01(elapsed / duration);
            SetAlpha(coreLine, alpha);
            SetAlpha(glowLine, alpha * 0.8f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        coreLine.positionCount = 0;
        glowLine.positionCount = 0;
        playback = null;
        Action callback = onComplete;
        onComplete = null;
        callback?.Invoke();
    }

    void BuildBolt(Vector2 start, Vector2 end, int seed) {
        Vector2 direction = end - start;
        float length = direction.magnitude;
        if (length < 0.001f) {
            coreLine.positionCount = 0;
            glowLine.positionCount = 0;
            return;
        }

        Vector2 forward = direction / length;
        Vector2 sideways = new(-forward.y, forward.x);
        int segments = Mathf.Clamp(Mathf.CeilToInt(length * 9f), 6, maxSegments);
        int pointCount = segments + 1;
        if (points.Length < pointCount) points = new Vector3[pointCount];

        System.Random random = new(seed);
        points[0] = new Vector3(start.x, start.y, -0.15f);
        for (int i = 1; i < segments; i++) {
            float t = i / (float)segments;
            float envelope = Mathf.Sin(t * Mathf.PI);
            float sidewaysOffset = ((float)random.NextDouble() * 2f - 1f) * jitter * envelope;
            float forwardOffset = ((float)random.NextDouble() * 2f - 1f) * jitter * 0.22f * envelope;
            Vector2 position = start + direction * t + sideways * sidewaysOffset + forward * forwardOffset;
            points[i] = new Vector3(position.x, position.y, -0.15f);
        }
        points[segments] = new Vector3(end.x, end.y, -0.15f);

        coreLine.positionCount = pointCount;
        glowLine.positionCount = pointCount;
        coreLine.SetPositions(points);
        glowLine.SetPositions(points);

        if (endpointSparkCount > 0) {
            sparks.transform.position = start;
            sparks.Emit(endpointSparkCount);
            sparks.transform.position = end;
            sparks.Emit(endpointSparkCount);
        }
    }

    LineRenderer CreateLine(string lineName, float width, Color color, int sortingOrder) {
        GameObject child = new(lineName);
        child.transform.SetParent(transform, false);

        LineRenderer line = child.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.alignment = LineAlignment.View;
        line.textureMode = LineTextureMode.Stretch;
        line.numCornerVertices = 1;
        line.numCapVertices = 2;
        line.widthMultiplier = width;
        line.startWidth = 1f;
        line.endWidth = 0.45f;
        line.startColor = color;
        line.endColor = new Color(color.r, color.g, color.b, 0f);
        line.sortingOrder = sortingOrder;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.sharedMaterial = lineMaterial != null ? lineMaterial : GetSharedLineMaterial();
        return line;
    }

    ParticleSystem CreateSparks() {
        GameObject child = new("EndpointSparks");
        child.transform.SetParent(transform, false);

        ParticleSystem system = child.AddComponent<ParticleSystem>();
        var main = system.main;
        main.playOnAwake = false;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.35f, 0.9f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.025f, 0.07f);
        main.startColor = coreColor;
        main.maxParticles = 40;

        var emission = system.emission;
        emission.enabled = false;

        var shape = system.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.02f;

        var particleRenderer = system.GetComponent<ParticleSystemRenderer>();
        particleRenderer.sortingOrder = 61;
        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleRenderer.sharedMaterial = GetSharedLineMaterial();
        return system;
    }

    void SetAlpha(LineRenderer line, float alpha) {
        Color start = line == coreLine ? coreColor : glowColor;
        line.startColor = new Color(start.r, start.g, start.b, start.a * alpha);
        line.endColor = new Color(start.r, start.g, start.b, 0f);
    }

    static int Hash(Vector2 start, Vector2 end) {
        unchecked {
            int hash = 17;
            hash = hash * 31 + Mathf.RoundToInt(start.x * 100f);
            hash = hash * 31 + Mathf.RoundToInt(start.y * 100f);
            hash = hash * 31 + Mathf.RoundToInt(end.x * 100f);
            return hash * 31 + Mathf.RoundToInt(end.y * 100f);
        }
    }

    static Material GetSharedLineMaterial() {
        if (sharedLineMaterial != null) return sharedLineMaterial;

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Unlit/Color");

        sharedLineMaterial = new Material(shader) {
            color = Color.white,
            hideFlags = HideFlags.DontSave
        };
        return sharedLineMaterial;
    }
}
