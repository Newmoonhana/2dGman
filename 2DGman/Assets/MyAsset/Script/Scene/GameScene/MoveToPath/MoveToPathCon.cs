using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Vector2 waypoint; // �̵� ��ǥ
    public float delaytime;  //�ش� ��ǥ�� �̵� �� ���� �ð�
    public SPEEDTYPE speed; //�̵� �ӵ�
}

public class MoveToPathCon : MonoBehaviour
{
    public Transform tns;
    public List<Waypoint> waypoints; // �̵� ��ǥ �迭
    public Vector2 currpoint;  //���� ��ǥ
    int waypointsIndex = 0;
    float f;
    public bool isReverse; //������ -> ������ -> ������ ���� ����(false: ��ü ��� �̵� �� 0�� �迭 ��ǥ�� �ٽ� ��ȯ)
    bool isDelay;
    public int loopcount;   //���� Ƚ��(-1 == ���� ����, ������ / ������ ����)
    bool loopdir = false;   //2ȸ �̻� ���� �� ���� ������ ����

    [ContextMenu("���� ��ǥ �缳��")]
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

        if (waypointsIndex < waypoints.Count)  // �̵� ��ǥ �迭 ũ�� -1����
        {
            //������ �ð����� ����
            if (!isDelay)
            {
                if (!IsInvoking("Delay"))
                    Invoke("Delay", waypoints[waypointsIndex].delaytime);
                return;
            }
            
            currpoint = tns.position;
            f = GameSceneData.GetSpeed(waypoints[waypointsIndex].speed) * Time.fixedDeltaTime;    // 1������ �� ������ ũ��
            tns.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex].waypoint, f);
            
            if (Vector2.Distance(waypoints[waypointsIndex].waypoint, currpoint) == 0)    //���� ��ǥ = �̵� ��ǥ(waypoints[waypointsIndex])��� �ε��� ����
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

        if (waypointsIndex >= 0)  // �̵� ��ǥ 0�� �迭����
        {
            //������ �ð����� ����
            if (!isDelay)
            {
                if (!IsInvoking("Delay"))
                    Invoke("Delay", waypoints[waypointsIndex].delaytime);
                return;
            }
            currpoint = tns.position;
            f = GameSceneData.GetSpeed(waypoints[waypointsIndex].speed) * Time.fixedDeltaTime;    // 1������ �� ������ ũ��
            tns.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex].waypoint, f);

            if (Vector2.Distance(waypoints[waypointsIndex].waypoint, currpoint) == 0)    //���� ��ǥ = �̵� ��ǥ(waypoints[waypointsIndex])��� �ε��� ����
            {
                isDelay = false;
                if (waypointsIndex == 0)
                {
                    if (isReverse) //Reverse Ư�� �� else ����
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
