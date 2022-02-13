using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEntity : MonoBehaviour
{
    public Vector2[] waypoints; // �̵� ��ǥ �迭
    public Vector2 currpoint;  //���� ��ǥ
    public float speed; //�̵� �ӵ�
    int waypointsIndex = 0;
    float f;

    void Update()
    {
        currpoint = transform.position;

        if (waypointsIndex < waypoints.Length)  // �̵� ��ǥ �迭 ũ�� -1����
        {
            f = speed * Time.fixedDeltaTime;    // 1������ �� ������ ũ��
            transform.position = Vector2.MoveTowards(currpoint, waypoints[waypointsIndex], f);

            if (Vector2.Distance(waypoints[waypointsIndex], currpoint) == 0)    //���� ��ǥ = �̵� ��ǥ(waypoints[waypointsIndex])��� �ε��� ����
                waypointsIndex++;
        }
    }
}
