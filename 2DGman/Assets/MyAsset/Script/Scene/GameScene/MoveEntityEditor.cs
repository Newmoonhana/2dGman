using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveEntity))]
[CanEditMultipleObjects]
public class MoveEntityEditor : Editor
{
    SerializedProperty currpoint, waypoints;

    private void OnEnable()
    {
        currpoint = serializedObject.FindProperty("currpoint");
        waypoints = serializedObject.FindProperty("waypoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(currpoint);
        EditorGUILayout.PropertyField(waypoints);
        serializedObject.ApplyModifiedProperties();
    }
}
