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

        //표시수치를 조정(표시 위치)
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

        waypoint.vector2Value = EditorGUI.Vector2Field(defaultRect, "이동 좌표", waypoint.vector2Value);

        delaytime_GS = new GUIStyle(EditorStyles.numberField);
        delaytime_GS.fixedHeight = 18f;

        delaytime.floatValue = EditorGUI.FloatField(delaytimeRect, "정지 시간", delaytime.floatValue, delaytime_GS);

        EditorGUI.PropertyField(speedRect, speed, new GUIContent("이동 속도"));
        if (speed.intValue == (int)SPEEDTYPE.NONE)
            EditorGUILayout.LabelField("[Warning] 이동 속도가 설정되지 않았습니다");

        EditorGUI.EndChangeCheck();
    }

    //GUI 요소의 높이(전체 크기)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        propertyHeight = base.GetPropertyHeight(property, label) + 5;
        return propertyHeight * count;
    }
}
