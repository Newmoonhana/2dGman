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

        //표시수치를 조정(표시 위치)
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

        waypoint.vector2Value = EditorGUI.Vector2Field(defaultRect, "이동 좌표", waypoint.vector2Value);

        staytime_GS = new GUIStyle(EditorStyles.numberField);
        staytime_GS.fixedHeight = 18f;
        staytime.floatValue = EditorGUI.FloatField(staytimeRect, "정지 시간", staytime.floatValue, staytime_GS);

        //if (EditorGUI.EndChangeCheck())
        //{
            
        //}
    }

    //GUI 요소의 높이(전체 크기)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        propertyHeight = base.GetPropertyHeight(property, label) + 5;
        return propertyHeight * count;
    }
}
