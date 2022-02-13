using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPath : MonoBehaviour
{
    public List<Vector2> waypoints; // �̵� ��ǥ �迭
    public Vector2 currpoint;  //���� ��ǥ
    public SPEEDTYPE speed; //�̵� �ӵ�
    int waypointsIndex = 0;
    float f;

    [ContextMenu("���� ��ǥ �缳��")]
    void RefreshCurrpoint()
    {
        currpoint = transform.position;
    }

    void Update()
    {
        currpoint = transform.position;

        if (waypointsIndex < waypoints.Count)  // �̵� ��ǥ �迭 ũ�� -1����
        {
            f = GameSceneData.GetSpeed(speed) * Time.fixedDeltaTime;    // 1������ �� ������ ũ��
            transform.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex], f);

            if (Vector2.Distance(waypoints[waypointsIndex], currpoint) == 0)    //���� ��ǥ = �̵� ��ǥ(waypoints[waypointsIndex])��� �ε��� ����
                waypointsIndex++;
        }
    }
}
