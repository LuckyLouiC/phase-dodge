using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SatellitePathVisualizer))]
public class SatellitePathVisualizerEditor : Editor
{
    SerializedProperty satellitePaths;
    SerializedProperty colors;

    private void OnEnable()
    {
        satellitePaths = serializedObject.FindProperty("satellitePaths");
        colors = serializedObject.FindProperty("colors");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(satellitePaths, new GUIContent("Satellite Paths"), true);

        if (satellitePaths.arraySize > colors.arraySize)
        {
            colors.arraySize = satellitePaths.arraySize;
        }

        for (int i = 0; i < satellitePaths.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(satellitePaths.GetArrayElementAtIndex(i), GUIContent.none);
            colors.GetArrayElementAtIndex(i).enumValueIndex = (int)(SatellitePathVisualizer.PathColor)EditorGUILayout.EnumPopup((SatellitePathVisualizer.PathColor)colors.GetArrayElementAtIndex(i).enumValueIndex);
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

