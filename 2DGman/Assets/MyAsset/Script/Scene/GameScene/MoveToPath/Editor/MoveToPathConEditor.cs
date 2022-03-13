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
        // [���� ���濡 ���� ���� ����]
        //����(OnInspectorGUI()���� ���� ������ ����)
        if (isLoopbtStyle == null)
            isLoopbtStyle = new GUIStyle(EditorStyles.miniButton);

        //������Ʈ�� ��ġ ����
        vec2 = script.transform.position;
        if (script.currpoint != vec2)
        {
            script.currpoint = vec2;
        }

        //(�����Ϳ��� �÷��� ���� �ƴ� ��) �迭�� ù �� = ������Ʈ�� ù ��ġ ����
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (script.waypoints.Count <= 0) //waypoints �迭�� �������� ����
            {
                Waypoint wp = new Waypoint();
                wp.waypoint = vec2;
                script.waypoints.Add(wp);
            }
            else
                script.waypoints[0].waypoint = vec2;    //�迭�� ù ���� ����
        }
#endif

        // [��ġ]
        serializedObject.Update();

        EditorGUILayout.PropertyField(tns, new GUIContent("������ Ʈ������")); EditorGUILayout.Space();
        EditorGUILayout.PropertyField(currpoint, new GUIContent("���� ��ǥ")); EditorGUILayout.Space();

        //Loop �׷�(bool == true ? ��Ʈ Bold : �⺻ ��Ʈ)
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Is Reverse", isLoopbtStyle))
            {
                isReverse.boolValue = !isReverse.boolValue;
                script.isReverse = isReverse.boolValue;
            }
            isLoopbtStyle.fontStyle = isReverse.boolValue ? FontStyle.Bold : FontStyle.Normal;
            EditorGUILayout.PropertyField(loopcount, new GUIContent("���� Ƚ��"));
        }
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(waypoints, new GUIContent("�̵��� ��ǥ ����Ʈ"));

        if (waypoints.arraySize == 0)
            EditorGUILayout.LabelField("[Warning] �̵� ��ǥ�� �������� �ʾҽ��ϴ�");
        serializedObject.ApplyModifiedProperties();
    }
}
