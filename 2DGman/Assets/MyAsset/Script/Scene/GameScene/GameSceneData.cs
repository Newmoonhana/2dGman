using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPEEDTYPE
{
    NONE,
    SLOW,
    NORMAL,
    FAST,

    _MAX
}

public class GameSceneData : MonoBehaviour
{
    //tmp

    public static Transform player_tns;
    public static PlayerCon playerCon_src;
    public List<GameObject> enemy_pre;
    List<EnemyCon> enemy_lst;

    public static float GetSpeed(SPEEDTYPE _type)
    {   
        return (int)_type * 3f;
    }

    private void Awake()
    {
        player_tns = GameObject.Find("Player").transform;
        playerCon_src = GameObject.Find("Player Controller").GetComponent<PlayerCon>();
        SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("Music"));
    }
}
