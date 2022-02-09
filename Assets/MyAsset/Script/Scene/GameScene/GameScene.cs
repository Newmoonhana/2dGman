using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    //tmp

    public List<GameObject> enemy_pre;
    public Transform enemyPulling_tns;
    List<EnemyCon> enemy_lst;


    private void Awake()
    {
        SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("Music"));
    }
}
