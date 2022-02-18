using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[View]
[System.Serializable]
public class PlayerView : MonoBehaviour
{
    public PlayerController player_con;
    public Text lifeText;   //라이프 표시
    public Text coinText;   //먹은 코인 개수 표시

    public void IsHurtEnd()
    {
        player_con.IsHurtEnd();
    }
    public void IsDeadEnd()
    {
        player_con.IsDeadEnd();
    }
}