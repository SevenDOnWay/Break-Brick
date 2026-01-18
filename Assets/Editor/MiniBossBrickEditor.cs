using UnityEditor;
using UnityEngine;
using static SpawnController;

[CustomPropertyDrawer(typeof(Brick))]
public class MiniBossBrickEditor : PropertyDrawer {

    public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) {
        if ( !property.isExpanded )
            return EditorGUIUtility.singleLineHeight; // only header when collapsed

        // Always: type, prefab, isMiniBoss
        int lineCount = 3;

        // Only show these if NOT miniboss
        if ( !property.FindPropertyRelative("isMiniBoss").boolValue ) {
            lineCount += 3; // minWave, baseChance, growthPerWave
        }

        // +1 for the foldout line
        return EditorGUIUtility.singleLineHeight * (lineCount + 1)
             + (lineCount) * EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
        // Use type name (or fallback to prefab name if you want)
        SerializedProperty typeProp = property.FindPropertyRelative("type");
        string displayName = typeProp.enumDisplayNames[typeProp.enumValueIndex];

        // Draw foldout header
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, displayName, true);

        if ( !property.isExpanded ) return; // collapsed

        EditorGUI.BeginProperty(position, label, property);

        // Indent children
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;

        // Track rect position
        Rect rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
            position.width, EditorGUIUtility.singleLineHeight);

        // Always shown
        EditorGUI.PropertyField(rect, typeProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUI.PropertyField(rect, property.FindPropertyRelative("prefab"));
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty isMiniBossProp = property.FindPropertyRelative("isMiniBoss");
        EditorGUI.PropertyField(rect, isMiniBossProp);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Only if not miniboss
        if ( !isMiniBossProp.boolValue ) {
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("minWave"));
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("baseChance"));
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("growthPerWave"));
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

}
