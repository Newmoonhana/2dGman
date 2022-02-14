using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Waypoint))]
public class WaypointDrawer : PropertyDrawer
{
    Rect defaultRect, delaytimeRect, speedRect;
    SerializedProperty waypoint, delaytime, speed;
    GUIStyle delaytime_GS;
    float propertyHeight = 18, count = 3;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        waypoint = property.FindPropertyRelative("waypoint");
        delaytime = property.FindPropertyRelative("delaytime");
        speed = property.FindPropertyRelative("speed");

        //ǥ�ü�ġ�� ����(ǥ�� ��ġ)
        defaultRect = new Rect(position)
        {
            height = propertyHeight
        };
        delaytimeRect = new Rect(defaultRect)
        {
            y = defaultRect.y + defaultRect.height,
            width = defaultRect.width * 0.6f
        };
        speedRect = new Rect(delaytimeRect)
        {
            y = delaytimeRect.y + delaytimeRect.height,
        };

        EditorGUI.BeginChangeCheck();

        waypoint.vector2Value = EditorGUI.Vector2Field(defaultRect, "�̵� ��ǥ", waypoint.vector2Value);

        delaytime_GS = new GUIStyle(EditorStyles.numberField);
        delaytime_GS.fixedHeight = 18f;

        delaytime.floatValue = EditorGUI.FloatField(delaytimeRect, "���� �ð�", delaytime.floatValue, delaytime_GS);

        EditorGUI.PropertyField(speedRect, speed, new GUIContent("�̵� �ӵ�"));
        if (speed.intValue == (int)SPEEDTYPE.NONE)
            EditorGUILayout.LabelField("[Warning] �̵� �ӵ��� �������� �ʾҽ��ϴ�");

        EditorGUI.EndChangeCheck();
    }

    //GUI ����� ����(��ü ũ��)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        propertyHeight = base.GetPropertyHeight(property, label) + 5;
        return propertyHeight * count;
    }
}
