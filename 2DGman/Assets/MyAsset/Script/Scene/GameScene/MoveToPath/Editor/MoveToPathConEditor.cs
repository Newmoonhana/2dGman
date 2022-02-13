using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(MoveToPathCon))]
[CanEditMultipleObjects]
public class MoveToPathConEditor : Editor
{
    //tmp
    //int i;
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

        //배열 크기 변경
        //if (waypoints_size != waypoints.arraySize)
        //{
        //    if (waypoints_size < waypoints.arraySize)   //설정한 값이 원래 크기보다 작을 때
        //    {
        //        i = waypoints.arraySize - waypoints_size;
        //        if (waypoints_size < 0) //설정한 값이 0보다 작을 때(array error 방지)
        //            i = waypoints.arraySize;    //waypoints_size를 0으로 취급
        //        script.waypoints.RemoveRange(waypoints.arraySize - i, i);
        //    }
        //    else    //설정한 값이 원래 크기보다 클 때
        //    {
        //        if (waypoints.arraySize > 0)
        //            vec2 = script.waypoints[waypoints.arraySize - 1];
        //        else    //waypoints.arraySize == 0
        //            vec2 = script.currpoint;


        //        script.waypoints.AddRange(Enumerable.Repeat<Vector2>(vec2, waypoints_size - waypoints.arraySize).ToArray());
        //    }
        //}

        serializedObject.Update();

        EditorGUILayout.PropertyField(currpoint);
        EditorGUILayout.PropertyField(speed);
        if (speed.intValue == (int)SPEEDTYPE.NONE)
            EditorGUILayout.LabelField("[Warning] 이동 속도가 설정되지 않았습니다");

        //waypoints_size = EditorGUILayout.IntField("이동 좌표 설정", waypoints_size);
        //for (i = 0; i < script.waypoints.Count; i++)
        //{
        //    EditorGUILayout.PropertyField(waypoints.GetArrayElementAtIndex(i));
        //}

        EditorGUILayout.PropertyField(waypoints);

        //if (waypoints_size == 0)
        if (waypoints.arraySize == 0)
            EditorGUILayout.LabelField("[Warning] 이동 좌표가 설정되지 않았습니다");
        serializedObject.ApplyModifiedProperties();
    }
}
