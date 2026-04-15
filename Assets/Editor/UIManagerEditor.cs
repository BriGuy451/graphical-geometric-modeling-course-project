using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw your normal inspector fields
        DrawDefaultInspector();

        // Reference to your component
        UIManager viewer = (UIManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== Live Runtime Debug ===");

        // Display any runtime values you want
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Animation State", viewer.CurrentState.ToString());

        // Forces repaint so the value updates
        if (Application.isPlaying)
            Repaint();
    }
}