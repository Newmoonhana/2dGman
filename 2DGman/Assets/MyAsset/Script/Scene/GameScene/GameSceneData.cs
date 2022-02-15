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
        return (int)_type * 3f;
    }

    private void Awake()
    {
        AudioManager.Instance.Play(AudioManager.Instance.GetAudioFile("Music"));
    }
}
