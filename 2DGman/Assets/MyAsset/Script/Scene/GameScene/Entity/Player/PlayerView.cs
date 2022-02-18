using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[View]
[System.Serializable]
public class PlayerView : MonoBehaviour
{
    public PlayerController player_con;
    public Text lifeText;   //������ ǥ��
    public Text coinText;   //���� ���� ���� ǥ��

    public void IsHurtEnd()
    {
        player_con.IsHurtEnd();
    }
    public void IsDeadEnd()
    {
        player_con.IsDeadEnd();
    }
}