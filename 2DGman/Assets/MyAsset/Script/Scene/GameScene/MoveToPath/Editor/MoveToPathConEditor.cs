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

        //�迭 ũ�� ����
        //if (waypoints_size != waypoints.arraySize)
        //{
        //    if (waypoints_size < waypoints.arraySize)   //������ ���� ���� ũ�⺸�� ���� ��
        //    {
        //        i = waypoints.arraySize - waypoints_size;
        //        if (waypoints_size < 0) //������ ���� 0���� ���� ��(array error ����)
        //            i = waypoints.arraySize;    //waypoints_size�� 0���� ���
        //        script.waypoints.RemoveRange(waypoints.arraySize - i, i);
        //    }
        //    else    //������ ���� ���� ũ�⺸�� Ŭ ��
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
            EditorGUILayout.LabelField("[Warning] �̵� �ӵ��� �������� �ʾҽ��ϴ�");

        //waypoints_size = EditorGUILayout.IntField("�̵� ��ǥ ����", waypoints_size);
        //for (i = 0; i < script.waypoints.Count; i++)
        //{
        //    EditorGUILayout.PropertyField(waypoints.GetArrayElementAtIndex(i));
        //}

        EditorGUILayout.PropertyField(waypoints);

        //if (waypoints_size == 0)
        if (waypoints.arraySize == 0)
            EditorGUILayout.LabelField("[Warning] �̵� ��ǥ�� �������� �ʾҽ��ϴ�");
        serializedObject.ApplyModifiedProperties();
    }
}
