using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Waypoint))]
public class WaypointDrawer : PropertyDrawer
{
    Rect _Rect, waypointRect, staytimeRect;
    SerializedProperty waypoint, staytime;
    GUIStyle staytime_GS;
    float propertyHeight;
    int count;  //총 라인의 개수

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        waypoint = property.FindPropertyRelative("waypoint");
        staytime = property.FindPropertyRelative("staytime");

        //표시수치를 조정(표시 위치)
        count = 2;
        _Rect = new Rect(position)
        {
            height = position.height
        };
        waypointRect = new Rect(_Rect)
        {
            height = _Rect.height / count
        };
        staytimeRect = new Rect(_Rect)
        {
            y = waypointRect.y + waypointRect.height,
            width = _Rect.width * 0.5f,
            height = _Rect.height / count
        };
        propertyHeight = count + count * 0.5f;

        EditorGUI.BeginChangeCheck();

        waypoint.vector2Value = EditorGUI.Vector2Field(waypointRect, "이동 좌표", waypoint.vector2Value);

        staytime_GS = new GUIStyle(EditorStyles.numberField);
        staytime_GS.fixedHeight = _Rect.height * 0.33f;
        staytime.floatValue = EditorGUI.FloatField(staytimeRect, "정지 시간", staytime.floatValue, staytime_GS);

        if (EditorGUI.EndChangeCheck())
        {
            
        }
    }

    //GUI 요소의 높이(전체 크기)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * propertyHeight;
    }
}
