using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Waypoint))]
public class WaypointDrawer : PropertyDrawer
{
    Rect defaultRect, staytimeRect;
    SerializedProperty waypoint, staytime;
    GUIStyle staytime_GS;
    float propertyHeight = 18, count = 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        waypoint = property.FindPropertyRelative("waypoint");
        staytime = property.FindPropertyRelative("staytime");

        //ǥ�ü�ġ�� ����(ǥ�� ��ġ)
        defaultRect = new Rect(position)
        {
            height = propertyHeight
        };
        staytimeRect = new Rect(defaultRect)
        {
            y = defaultRect.y + defaultRect.height,
            width = defaultRect.width * 0.5f,
        };

        EditorGUI.BeginChangeCheck();

        waypoint.vector2Value = EditorGUI.Vector2Field(defaultRect, "�̵� ��ǥ", waypoint.vector2Value);

        staytime_GS = new GUIStyle(EditorStyles.numberField);
        staytime_GS.fixedHeight = 18f;
        staytime.floatValue = EditorGUI.FloatField(staytimeRect, "���� �ð�", staytime.floatValue, staytime_GS);

        //if (EditorGUI.EndChangeCheck())
        //{
            
        //}
    }

    //GUI ����� ����(��ü ũ��)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        propertyHeight = base.GetPropertyHeight(property, label) + 5;
        return propertyHeight * count;
    }
}
