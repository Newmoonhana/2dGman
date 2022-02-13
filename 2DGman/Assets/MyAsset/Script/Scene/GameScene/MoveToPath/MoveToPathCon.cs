using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Vector2 waypoint; // 이동 좌표
    public float staytime;  //해당 좌표로 이동 후 정지 시간
}

public class MoveToPathCon : MonoBehaviour
{
    [SerializeField] public List<Waypoint> waypoints; // 이동 좌표 배열
    public Vector2 currpoint;  //현재 좌표
    public SPEEDTYPE speed; //이동 속도
    int waypointsIndex = 0;
    float f;

    [ContextMenu("현재 좌표 재설정")]
    void RefreshCurrpoint()
    {
        currpoint = transform.position;
    }

    void Update()
    {
        currpoint = transform.position;

        if (waypointsIndex < waypoints.Count)  // 이동 좌표 배열 크기 -1까지
        {
            f = GameSceneData.GetSpeed(speed) * Time.fixedDeltaTime;    // 1프레임 당 움직임 크기
            transform.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex].waypoint, f);

            if (Vector2.Distance(waypoints[waypointsIndex].waypoint, currpoint) == 0)    //현재 좌표 = 이동 좌표(waypoints[waypointsIndex])라면 인덱스 증가
                waypointsIndex++;
        }
    }
}
