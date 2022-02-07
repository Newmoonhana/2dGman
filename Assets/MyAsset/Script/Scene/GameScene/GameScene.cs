using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    //tmp



    private void Awake()
    {
        SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("Music"));
    }
}
