using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(MoveToPathCon))]
[CanEditMultipleObjects]
public class MoveToPathConEditor : Editor
{
    //tmp
    Vector2 vec2;
    Waypoint wp;

    MoveToPathCon script;
    SerializedProperty currpoint, waypoints, speed;
    //int waypoints_size;

    private void OnEnable()
    {
        script = (MoveToPathCon)target;
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
        {
            script.currpoint = vec2;
        }

        //배열의 첫 값 = 오브젝트의 첫 위치
        if (script.waypoints.Count <= 0) //waypoints 배열이 존재하지 않음
        {
            Waypoint wp = new Waypoint();
            wp.waypoint = vec2;
            script.waypoints.Add(wp);
        }
        else
            script.waypoints[0].waypoint = vec2;    //배열의 첫 값에 대입

        serializedObject.Update();

        EditorGUILayout.PropertyField(currpoint);

        if (GUILayout.Button("Left Loop"))
        {
            
        }
        if (GUILayout.Button("Right Loop"))
        {

        }

        EditorGUILayout.PropertyField(speed);
        if (speed.intValue == (int)SPEEDTYPE.NONE)
            EditorGUILayout.LabelField("[Warning] 이동 속도가 설정되지 않았습니다");

        EditorGUILayout.PropertyField(waypoints);

        if (waypoints.arraySize == 0)
            EditorGUILayout.LabelField("[Warning] 이동 좌표가 설정되지 않았습니다");
        serializedObject.ApplyModifiedProperties();
    }
}
