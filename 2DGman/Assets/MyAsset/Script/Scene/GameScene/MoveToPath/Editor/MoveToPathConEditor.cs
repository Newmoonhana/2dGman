using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(MoveToPathCon))]
[CanEditMultipleObjects]
public class MoveToPathConEditor : Editor
{
    Vector2 vec2;   //tmp

    MoveToPathCon script;
    SerializedProperty tns, currpoint, waypoints, loopcount, isReverse;
    GUIStyle isLoopbtStyle;

    private void OnEnable()
    {
        script = (MoveToPathCon)target;
        tns = serializedObject.FindProperty("tns");
        currpoint = serializedObject.FindProperty("currpoint");
        waypoints = serializedObject.FindProperty("waypoints");
        loopcount = serializedObject.FindProperty("loopcount");
        isReverse = serializedObject.FindProperty("isReverse");
    }

    public override void OnInspectorGUI()
    {
        // [변수 변경에 따른 설정 변경]
        //선언(OnInspectorGUI()에서 하지 않으면 에러)
        if (isLoopbtStyle == null)
            isLoopbtStyle = new GUIStyle(EditorStyles.miniButton);

        //오브젝트의 위치 변경
        vec2 = script.transform.position;
        if (script.currpoint != vec2)
        {
            script.currpoint = vec2;
        }

        //(에디터에서 플레이 중이 아닐 때) 배열의 첫 값 = 오브젝트의 첫 위치 세팅
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (script.waypoints.Count <= 0) //waypoints 배열이 존재하지 않음
            {
                Waypoint wp = new Waypoint();
                wp.waypoint = vec2;
                script.waypoints.Add(wp);
            }
            else
                script.waypoints[0].waypoint = vec2;    //배열의 첫 값에 대입
        }
#endif

        // [배치]
        serializedObject.Update();

        EditorGUILayout.PropertyField(tns, new GUIContent("움직일 트랜스폼")); EditorGUILayout.Space();
        EditorGUILayout.PropertyField(currpoint, new GUIContent("현재 좌표")); EditorGUILayout.Space();

        //Loop 그룹(bool == true ? 폰트 Bold : 기본 폰트)
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Is Reverse", isLoopbtStyle))
            {
                isReverse.boolValue = !isReverse.boolValue;
                script.isReverse = isReverse.boolValue;
            }
            isLoopbtStyle.fontStyle = isReverse.boolValue ? FontStyle.Bold : FontStyle.Normal;
            EditorGUILayout.PropertyField(loopcount, new GUIContent("루프 횟수"));
        }
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(waypoints, new GUIContent("이동할 좌표 리스트"));

        if (waypoints.arraySize == 0)
            EditorGUILayout.LabelField("[Warning] 이동 좌표가 설정되지 않았습니다");
        serializedObject.ApplyModifiedProperties();
    }
}
