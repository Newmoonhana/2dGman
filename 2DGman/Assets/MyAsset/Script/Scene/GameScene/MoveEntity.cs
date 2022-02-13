using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEntity : MonoBehaviour
{
    public Vector2[] waypoints; // 이동 좌표 배열
    public Vector2 currpoint;  //현재 좌표
    public float speed; //이동 속도
    int waypointsIndex = 0;
    float f;

    void Update()
    {
        currpoint = transform.position;

        if (waypointsIndex < waypoints.Length)  // 이동 좌표 배열 크기 -1까지
        {
            f = speed * Time.fixedDeltaTime;    // 1프레임 당 움직임 크기
            transform.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex], f);

            if (Vector2.Distance(waypoints[waypointsIndex], currpoint) == 0)    //현재 좌표 = 이동 좌표(waypoints[waypointsIndex])라면 인덱스 증가
                waypointsIndex++;
        }
    }
}
