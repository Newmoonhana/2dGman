using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Vector2 waypoint; // 이동 좌표
    public float delaytime;  //해당 좌표로 이동 후 정지 시간
    public SPEEDTYPE speed; //이동 속도
}

public class MoveToPathCon : MonoBehaviour
{
    public Transform tns;
    public List<Waypoint> waypoints; // 이동 좌표 배열
    public Vector2 currpoint;  //현재 좌표
    int waypointsIndex = 0;
    float f;
    public bool isReverse; //순방향 -> 역방향 -> 순방향 루프 여부(false: 전체 경로 이동 후 0번 배열 좌표로 다시 순환)
    bool isDelay;
    public int loopcount;   //루프 횟수(-1 == 무한 루프, 순방향 / 역방향 별개)
    bool loopdir = false;   //2회 이상 루프 시 방향 조절용 변수

    [ContextMenu("현재 좌표 재설정")]
    void RefreshCurrpoint()
    {
        currpoint = tns.position;
    }

    void Update()
    {
        if (loopdir == false)
            Move();
        else
            ReverseMove();
    }

    void Delay()
    {
        isDelay = true;
    }

    private void Move()
    {
        if (loopcount == 0)
            return;

        if (waypointsIndex < waypoints.Count)  // 이동 좌표 배열 크기 -1까지
        {
            //딜레이 시간동안 정지
            if (!isDelay)
            {
                if (!IsInvoking("Delay"))
                    Invoke("Delay", waypoints[waypointsIndex].delaytime);
                return;
            }
            
            currpoint = tns.position;
            f = GameSceneData.GetSpeed(waypoints[waypointsIndex].speed) * Time.fixedDeltaTime;    // 1프레임 당 움직임 크기
            tns.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex].waypoint, f);
            
            if (Vector2.Distance(waypoints[waypointsIndex].waypoint, currpoint) == 0)    //현재 좌표 = 이동 좌표(waypoints[waypointsIndex])라면 인덱스 증가
            {
                isDelay = false;
                if (waypointsIndex == waypoints.Count - 1)
                {
                    if (isReverse)
                    {
                        loopdir = true;
                        waypointsIndex--;
                    }
                    else
                    {
                            waypointsIndex = 0;
                    }
                    if (loopcount > 0)
                        loopcount--;
                }
                else
                    waypointsIndex++;
            }
        }
    }

    void ReverseMove()
    {
        if (loopcount == 0)
            return;

        if (waypointsIndex >= 0)  // 이동 좌표 0번 배열까지
        {
            //딜레이 시간동안 정지
            if (!isDelay)
            {
                if (!IsInvoking("Delay"))
                    Invoke("Delay", waypoints[waypointsIndex].delaytime);
                return;
            }
            currpoint = tns.position;
            f = GameSceneData.GetSpeed(waypoints[waypointsIndex].speed) * Time.fixedDeltaTime;    // 1프레임 당 움직임 크기
            tns.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex].waypoint, f);

            if (Vector2.Distance(waypoints[waypointsIndex].waypoint, currpoint) == 0)    //현재 좌표 = 이동 좌표(waypoints[waypointsIndex])라면 인덱스 증가
            {
                isDelay = false;
                if (waypointsIndex == 0)
                {
                    if (isReverse) //Reverse 특성 상 else 없음
                    {
                        loopdir = false;
                        waypointsIndex++;
                    }
                    if (loopcount > 0)
                        loopcount--;
                }
                else
                    waypointsIndex--;
            }
        }
    }
}
