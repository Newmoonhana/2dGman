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



    public static float GetSpeed(SPEEDTYPE _type)
    {   
        return (float)_type * 2f;
    }

    private void Awake()
    {
        AudioManager.Instance.Play("Music");
    }
}
