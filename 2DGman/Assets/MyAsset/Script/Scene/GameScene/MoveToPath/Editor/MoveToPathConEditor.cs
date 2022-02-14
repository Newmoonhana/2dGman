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
        // [���� ���濡 ���� ���� ����]
        //������Ʈ�� ��ġ ����
        vec2 = script.transform.position;
        if (script.currpoint != vec2)
        {
            script.currpoint = vec2;
        }

        //�迭�� ù �� = ������Ʈ�� ù ��ġ
        if (script.waypoints.Count <= 0) //waypoints �迭�� �������� ����
        {
            Waypoint wp = new Waypoint();
            wp.waypoint = vec2;
            script.waypoints.Add(wp);
        }
        else
            script.waypoints[0].waypoint = vec2;    //�迭�� ù ���� ����

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
            EditorGUILayout.LabelField("[Warning] �̵� �ӵ��� �������� �ʾҽ��ϴ�");

        EditorGUILayout.PropertyField(waypoints);

        if (waypoints.arraySize == 0)
            EditorGUILayout.LabelField("[Warning] �̵� ��ǥ�� �������� �ʾҽ��ϴ�");
        serializedObject.ApplyModifiedProperties();
    }
}
