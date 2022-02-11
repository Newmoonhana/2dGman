using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    //tmp

    public static PlayerCon playerCon_src;
    public List<GameObject> enemy_pre;
    public Transform enemyPulling_tns;
    List<EnemyCon> enemy_lst;


    private void Awake()
    {
        playerCon_src = GameObject.Find("Player Controller").GetComponent<PlayerCon>();
        SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("Music"));
    }
}