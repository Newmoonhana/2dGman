using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IsDistance : MonoBehaviour
{
    public Transform entity_tns;
    public float distance;
    float dist = 9999;
    public bool isDist;

    private void Update()
    {
        dist = Vector2.Distance(entity_tns.position, transform.position);
        if (dist <= distance)
            isDist = true;
        else
            isDist = false;
    }
}
