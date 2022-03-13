using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public static PlayerController player_controller;
    public static Transform startpoint_tns;
    public static GameObject clearCanvas_obj;

    public static float GetSpeed(SPEEDTYPE _type)
    {   
        return (float)_type * 1.75f;
    }

    private void Awake()
    {
        AudioManager.Instance.Play("10. Track 10");
        player_controller = GameObject.Find("Player Controller").GetComponent<PlayerController>();
        startpoint_tns = GameObject.Find("Start Point").transform;
        clearCanvas_obj = GameObject.Find("GameCanvas").transform.Find("Clear Panel").gameObject;
    }

    public void RetryButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
