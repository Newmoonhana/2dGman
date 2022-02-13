using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Vector2 waypoint; // �̵� ��ǥ
    public float staytime;  //�ش� ��ǥ�� �̵� �� ���� �ð�
}

public class MoveToPathCon : MonoBehaviour
{
    [SerializeField] public List<Waypoint> waypoints; // �̵� ��ǥ �迭
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
            transform.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex].waypoint, f);

            if (Vector2.Distance(waypoints[waypointsIndex].waypoint, currpoint) == 0)    //���� ��ǥ = �̵� ��ǥ(waypoints[waypointsIndex])��� �ε��� ����
                waypointsIndex++;
        }
    }
}
