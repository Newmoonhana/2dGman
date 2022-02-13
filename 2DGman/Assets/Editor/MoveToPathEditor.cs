using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(MoveToPath))]
[CanEditMultipleObjects]
public class MoveToPathEditor : Editor
{
    //tmp
    int i, i2;
    Vector2 vec2;

    MoveToPath script;
    SerializedProperty currpoint, waypoints, speed;
    int waypoints_size;

    private void OnEnable()
    {
        script = (MoveToPath)target;
        currpoint = serializedObject.FindProperty("currpoint");
        speed = serializedObject.FindProperty("speed");
        waypoints = serializedObject.FindProperty("waypoints");
    }

    public override void OnInspectorGUI()
    {
        // [변수 변경에 따른 설정 변경]
        //오브젝트의 위치 변경
        vec2 = script.transform.position;
        if (script.currpoint != vec2)
            script.currpoint = vec2;

        //배열 크기 변경
        if (waypoints_size != waypoints.arraySize)
        {
            if (waypoints_size < waypoints.arraySize)
            {
                i = waypoints.arraySize - waypoints_size;
                if (waypoints_size < 0)
                    i = waypoints.arraySize;
                script.waypoints.RemoveRange(waypoints.arraySize - i, i);
            }
            else
            {
                if (waypoints.arraySize > 0)
                    vec2 = script.waypoints[waypoints.arraySize - 1];
                else
                    vec2 = Vector2.zero;

                script.waypoints.AddRange(Enumerable.Repeat<Vector2>(vec2, waypoints_size - waypoints.arraySize).ToArray());
            }
        }

        serializedObject.Update();

        EditorGUILayout.PropertyField(currpoint);
        EditorGUILayout.PropertyField(speed);
        if (speed.intValue == (int)SPEEDTYPE.NONE)
            EditorGUILayout.LabelField("[Warning] 이동 속도가 설정되지 않았습니다");

        waypoints_size = EditorGUILayout.IntField("이동 좌표 설정", waypoints_size);
        for (i = 0; i < script.waypoints.Count; i++)
        {
            EditorGUILayout.PropertyField(waypoints.GetArrayElementAtIndex(i));
        }

        //EditorGUILayout.PropertyField(waypoints);

        if (waypoints_size == 0)
        //if (waypoints.arraySize == 0)
            EditorGUILayout.LabelField("[Warning] 이동 좌표가 설정되지 않았습니다");
        serializedObject.ApplyModifiedProperties();
    }
}
