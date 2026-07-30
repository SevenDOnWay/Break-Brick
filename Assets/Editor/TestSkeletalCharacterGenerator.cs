using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class TestSkeletalCharacterGenerator
{
    private const string RootFolder = "Assets/Art/GeneratedCharacters";
    private const string SpriteFolder = RootFolder + "/Sprites";
    private const string AnimationFolder = RootFolder + "/Animations";
    private const string ControllerFolder = RootFolder + "/Controllers";
    private const string PrefabFolder = RootFolder + "/Prefabs";
    private const float PixelsPerUnit = 100f;

    private enum PartShape
    {
        Capsule,
        Circle,
        Triangle,
        Rectangle,
        Blade,
        Staff,
        Dagger
    }

    private readonly struct CharacterSpec
    {
        public CharacterSpec(
            string name,
            Color primary,
            Color secondary,
            Color accent,
            PartShape weaponShape,
            Vector2 weaponSize)
        {
            Name = name;
            Primary = primary;
            Secondary = secondary;
            Accent = accent;
            WeaponShape = weaponShape;
            WeaponSize = weaponSize;
        }

        public string Name { get; }
        public Color Primary { get; }
        public Color Secondary { get; }
        public Color Accent { get; }
        public PartShape WeaponShape { get; }
        public Vector2 WeaponSize { get; }
    }

    [MenuItem("Tools/Generate Test Skeletal Characters")]
    public static void Generate()
    {
        EnsureFolders();

        var specs = new[]
        {
            new CharacterSpec(
                "Wizard",
                new Color(0.34f, 0.23f, 0.82f, 1f),
                new Color(0.16f, 0.78f, 0.98f, 1f),
                new Color(1f, 0.84f, 0.23f, 1f),
                PartShape.Staff,
                new Vector2(18, 116)),
            new CharacterSpec(
                "Warrior",
                new Color(0.72f, 0.16f, 0.13f, 1f),
                new Color(0.72f, 0.72f, 0.72f, 1f),
                new Color(0.97f, 0.65f, 0.22f, 1f),
                PartShape.Blade,
                new Vector2(24, 96)),
            new CharacterSpec(
                "Thief",
                new Color(0.12f, 0.43f, 0.28f, 1f),
                new Color(0.1f, 0.1f, 0.12f, 1f),
                new Color(0.78f, 0.85f, 0.38f, 1f),
                PartShape.Dagger,
                new Vector2(24, 48)),
        };

        foreach (CharacterSpec spec in specs)
        {
            GenerateCharacter(spec);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generated test skeletal characters in {RootFolder}.");
    }

    private static void EnsureFolders()
    {
        foreach (string folder in new[] { RootFolder, SpriteFolder, AnimationFolder, ControllerFolder, PrefabFolder })
        {
            EnsureFolder(folder);
        }
    }

    private static void EnsureFolder(string assetFolder)
    {
        if (AssetDatabase.IsValidFolder(assetFolder))
        {
            return;
        }

        string parent = Path.GetDirectoryName(assetFolder)?.Replace("\\", "/") ?? "Assets";
        string name = Path.GetFileName(assetFolder);
        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, name);
    }

    private static void GenerateCharacter(CharacterSpec spec)
    {
        Dictionary<string, Sprite> sprites = GenerateSprites(spec);
        GameObject prefabRoot = BuildRig(spec, sprites);
        string characterAnimationFolder = $"{AnimationFolder}/{spec.Name}";
        EnsureFolder(characterAnimationFolder);

        AnimationClip walk = CreateLocomotionClip($"{characterAnimationFolder}/{spec.Name}_Walk.anim", false);
        AnimationClip run = CreateLocomotionClip($"{characterAnimationFolder}/{spec.Name}_Run.anim", true);
        AnimationClip attack = CreateAttackClip($"{characterAnimationFolder}/{spec.Name}_Attack.anim");
        AnimationClip death = CreateDeathClip($"{characterAnimationFolder}/{spec.Name}_Death.anim");
        AnimatorController controller = CreateController(spec, walk, run, attack, death);

        Animator animator = prefabRoot.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        string prefabPath = $"{PrefabFolder}/{spec.Name}_TestSkeletal.prefab";
        AssetDatabase.DeleteAsset(prefabPath);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        Object.DestroyImmediate(prefabRoot);
    }

    private static Dictionary<string, Sprite> GenerateSprites(CharacterSpec spec)
    {
        var sprites = new Dictionary<string, Sprite>();
        string characterSpriteFolder = $"{SpriteFolder}/{spec.Name}";
        EnsureFolder(characterSpriteFolder);

        sprites["Head"] = CreateSprite($"{characterSpriteFolder}/{spec.Name}_Head.png", PartShape.Circle, spec.Secondary, 48, 48);
        sprites["Hat"] = CreateSprite($"{characterSpriteFolder}/{spec.Name}_Hat.png", PartShape.Triangle, spec.Primary, 60, 58);
        sprites["Torso"] = CreateSprite($"{characterSpriteFolder}/{spec.Name}_Torso.png", PartShape.Capsule, spec.Primary, 54, 78);
        sprites["Hip"] = CreateSprite($"{characterSpriteFolder}/{spec.Name}_Hip.png", PartShape.Rectangle, spec.Secondary, 48, 30);
        sprites["Arm"] = CreateSprite($"{characterSpriteFolder}/{spec.Name}_Arm.png", PartShape.Capsule, spec.Secondary, 20, 58);
        sprites["Leg"] = CreateSprite($"{characterSpriteFolder}/{spec.Name}_Leg.png", PartShape.Capsule, spec.Secondary, 22, 64);
        sprites["Foot"] = CreateSprite($"{characterSpriteFolder}/{spec.Name}_Foot.png", PartShape.Capsule, spec.Accent, 34, 18);
        sprites["Weapon"] = CreateSprite(
            $"{characterSpriteFolder}/{spec.Name}_Weapon.png",
            spec.WeaponShape,
            spec.Accent,
            Mathf.RoundToInt(spec.WeaponSize.x),
            Mathf.RoundToInt(spec.WeaponSize.y));

        return sprites;
    }

    private static Sprite CreateSprite(string assetPath, PartShape shape, Color color, int width, int height)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? Directory.GetCurrentDirectory());

        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var clear = new Color(0f, 0f, 0f, 0f);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, IsInsideShape(shape, x, y, width, height) ? color : clear);
            }
        }

        texture.Apply();
        File.WriteAllBytes(fullPath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }

    private static bool IsInsideShape(PartShape shape, int x, int y, int width, int height)
    {
        float nx = (x + 0.5f) / width;
        float ny = (y + 0.5f) / height;
        float cx = nx - 0.5f;
        float cy = ny - 0.5f;

        switch (shape)
        {
            case PartShape.Circle:
                return cx * cx + cy * cy <= 0.23f;
            case PartShape.Triangle:
                return ny <= 0.96f && ny >= 0.06f && Mathf.Abs(cx) <= ny * 0.45f;
            case PartShape.Rectangle:
                return nx > 0.08f && nx < 0.92f && ny > 0.12f && ny < 0.88f;
            case PartShape.Blade:
                return Mathf.Abs(cx) < Mathf.Lerp(0.08f, 0.35f, ny) && ny > 0.05f && ny < 0.95f;
            case PartShape.Staff:
                return Mathf.Abs(cx) < 0.08f || (cy > 0.28f && cx * cx + (cy - 0.32f) * (cy - 0.32f) < 0.06f);
            case PartShape.Dagger:
                return (Mathf.Abs(cx) < Mathf.Lerp(0.05f, 0.3f, ny) && ny > 0.28f) ||
                    (ny <= 0.36f && Mathf.Abs(cx) < 0.26f);
            default:
                return Mathf.Abs(cx) < 0.38f && Mathf.Abs(cy) < 0.45f;
        }
    }

    private static GameObject BuildRig(CharacterSpec spec, IReadOnlyDictionary<string, Sprite> sprites)
    {
        var root = new GameObject(spec.Name + "_TestSkeletal");
        var rig = CreateBone(root.transform, "Rig", Vector3.zero, 0);
        CreatePart(rig, "Hip", sprites["Hip"], new Vector3(0f, 0.7f, 0f), 0);
        CreatePart(rig, "Torso", sprites["Torso"], new Vector3(0f, 1.2f, 0f), 1);
        CreatePart(rig, "Head", sprites["Head"], new Vector3(0f, 1.82f, 0f), 3);
        CreatePart(rig, "Hat", sprites["Hat"], new Vector3(0f, 2.15f, 0f), 4);

        Transform leftArm = CreatePart(rig, "LeftArm", sprites["Arm"], new Vector3(-0.36f, 1.35f, 0f), 2);
        Transform rightArm = CreatePart(rig, "RightArm", sprites["Arm"], new Vector3(0.36f, 1.35f, 0f), 2);
        Transform weapon = CreatePart(rightArm, "Weapon", sprites["Weapon"], new Vector3(0.18f, -0.38f, 0f), 5);
        weapon.localRotation = Quaternion.Euler(0f, 0f, -18f);

        CreatePart(rig, "LeftLeg", sprites["Leg"], new Vector3(-0.18f, 0.2f, 0f), -1);
        CreatePart(rig, "RightLeg", sprites["Leg"], new Vector3(0.18f, 0.2f, 0f), -1);
        CreatePart(rig, "LeftFoot", sprites["Foot"], new Vector3(-0.22f, -0.18f, 0f), 0);
        CreatePart(rig, "RightFoot", sprites["Foot"], new Vector3(0.22f, -0.18f, 0f), 0);

        return root;
    }

    private static Transform CreateBone(Transform parent, string name, Vector3 localPosition, int sortingOrder)
    {
        var bone = new GameObject(name);
        bone.transform.SetParent(parent, false);
        bone.transform.localPosition = localPosition;
        bone.transform.localRotation = Quaternion.identity;
        bone.transform.localScale = Vector3.one;
        return bone.transform;
    }

    private static Transform CreatePart(Transform parent, string name, Sprite sprite, Vector3 localPosition, int sortingOrder)
    {
        Transform part = CreateBone(parent, name, localPosition, sortingOrder);
        SpriteRenderer renderer = part.gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = sortingOrder;
        return part;
    }

    private static AnimatorController CreateController(
        CharacterSpec spec,
        AnimationClip walk,
        AnimationClip run,
        AnimationClip attack,
        AnimationClip death)
    {
        string controllerPath = $"{ControllerFolder}/{spec.Name}_TestSkeletal.controller";
        AssetDatabase.DeleteAsset(controllerPath);

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        controller.AddParameter("Walk", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Run", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Death", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        stateMachine.states = new ChildAnimatorState[0];
        stateMachine.anyStateTransitions = new AnimatorStateTransition[0];

        AnimatorState walkState = AddState(stateMachine, "Walk", walk, new Vector3(260, 80, 0));
        AnimatorState runState = AddState(stateMachine, "Run", run, new Vector3(520, 80, 0));
        AnimatorState attackState = AddState(stateMachine, "Attack", attack, new Vector3(260, 240, 0));
        AnimatorState deathState = AddState(stateMachine, "Death", death, new Vector3(520, 240, 0));
        stateMachine.defaultState = walkState;

        AddAnyStateTrigger(stateMachine, walkState, "Walk");
        AddAnyStateTrigger(stateMachine, runState, "Run");
        AddAnyStateTrigger(stateMachine, attackState, "Attack");
        AddAnyStateTrigger(stateMachine, deathState, "Death");

        return controller;
    }

    private static AnimatorState AddState(AnimatorStateMachine stateMachine, string name, AnimationClip clip, Vector3 position)
    {
        AnimatorState state = stateMachine.AddState(name, position);
        state.motion = clip;
        return state;
    }

    private static void AddAnyStateTrigger(AnimatorStateMachine stateMachine, AnimatorState targetState, string triggerName)
    {
        AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(targetState);
        transition.hasExitTime = false;
        transition.duration = 0.04f;
        transition.AddCondition(AnimatorConditionMode.If, 0f, triggerName);
    }

    private static AnimationClip CreateLocomotionClip(string path, bool isRun)
    {
        AssetDatabase.DeleteAsset(path);
        var clip = new AnimationClip
        {
            frameRate = 12f,
            wrapMode = WrapMode.Loop
        };

        float duration = isRun ? 0.5f : 0.8f;
        float arm = isRun ? 34f : 22f;
        float leg = isRun ? 32f : 20f;
        float bounce = isRun ? 0.08f : 0.04f;

        AddLoopCurve(clip, "Rig", typeof(Transform), "m_LocalPosition.y", 0f, bounce, 0f, duration);
        AddLoopCurve(clip, "Rig/LeftArm", typeof(Transform), "localEulerAngles.z", arm, -arm, arm, duration);
        AddLoopCurve(clip, "Rig/RightArm", typeof(Transform), "localEulerAngles.z", -arm, arm, -arm, duration);
        AddLoopCurve(clip, "Rig/LeftLeg", typeof(Transform), "localEulerAngles.z", -leg, leg, -leg, duration);
        AddLoopCurve(clip, "Rig/RightLeg", typeof(Transform), "localEulerAngles.z", leg, -leg, leg, duration);
        AddLoopCurve(clip, "Rig/Head", typeof(Transform), "localEulerAngles.z", -4f, 4f, -4f, duration);

        SetLooping(clip, true);
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static AnimationClip CreateAttackClip(string path)
    {
        AssetDatabase.DeleteAsset(path);
        var clip = new AnimationClip
        {
            frameRate = 12f,
            wrapMode = WrapMode.Once
        };

        AddCurve(clip, "Rig/RightArm", typeof(Transform), "localEulerAngles.z", 12f, -82f, 26f, 0.42f);
        AddCurve(clip, "Rig/RightArm/Weapon", typeof(Transform), "localEulerAngles.z", -8f, -64f, -12f, 0.42f);
        AddCurve(clip, "Rig/Torso", typeof(Transform), "localEulerAngles.z", 0f, -10f, 0f, 0.42f);
        AddCurve(clip, "Rig/Head", typeof(Transform), "localEulerAngles.z", 0f, -8f, 0f, 0.42f);
        AddCurve(clip, "Rig", typeof(Transform), "m_LocalPosition.x", 0f, 0.1f, 0f, 0.42f);

        SetLooping(clip, false);
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static AnimationClip CreateDeathClip(string path)
    {
        AssetDatabase.DeleteAsset(path);
        var clip = new AnimationClip
        {
            frameRate = 12f,
            wrapMode = WrapMode.Once
        };

        AddCurve(clip, "Rig", typeof(Transform), "localEulerAngles.z", 0f, 0f, -86f, 0.7f);
        AddCurve(clip, "Rig", typeof(Transform), "m_LocalPosition.y", 0f, -0.25f, -0.75f, 0.7f);
        AddCurve(clip, "Rig/LeftArm", typeof(Transform), "localEulerAngles.z", 0f, 48f, 84f, 0.7f);
        AddCurve(clip, "Rig/RightArm", typeof(Transform), "localEulerAngles.z", 0f, -48f, -84f, 0.7f);
        AddCurve(clip, "Rig/Head", typeof(Transform), "localEulerAngles.z", 0f, 14f, 26f, 0.7f);

        SetLooping(clip, false);
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static void AddLoopCurve(AnimationClip clip, string path, System.Type type, string property, float a, float b, float c, float duration)
    {
        var curve = new AnimationCurve(
            new Keyframe(0f, a),
            new Keyframe(duration * 0.5f, b),
            new Keyframe(duration, c));
        AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, type, property), curve);
    }

    private static void AddCurve(AnimationClip clip, string path, System.Type type, string property, float a, float b, float c, float duration)
    {
        var curve = new AnimationCurve(
            new Keyframe(0f, a),
            new Keyframe(duration * 0.5f, b),
            new Keyframe(duration, c));
        AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, type, property), curve);
    }

    private static void SetLooping(AnimationClip clip, bool loop)
    {
        SerializedObject serializedClip = new SerializedObject(clip);
        SerializedProperty settings = serializedClip.FindProperty("m_AnimationClipSettings");
        if (settings != null)
        {
            settings.FindPropertyRelative("m_LoopTime").boolValue = loop;
            settings.FindPropertyRelative("m_LoopBlend").boolValue = loop;
        }

        serializedClip.ApplyModifiedProperties();
    }
}
