using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : SingletonPattern_IsA_Mono<GameManager>
{
    //tmp


    public void Awake()
    {
        if (!DontDestroyInst(this))
            return;

        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    /// <summary>
    /// 최적화 - 프레임 제약(높은 프레임이 필요하지 않는 경우 프레임 수 낮추기)
    /// </summary>
    /// <param name="_isdown">프레임 수 낮추기</param>
    public void Setting_Frame(bool _isdown)
    {
        Application.targetFrameRate = 60;   //기본 fps
        int _interval = 1;  // 60 / 1 = 기본 fps
        if (_isdown)
            _interval = 3;  //낮출 fps
        OnDemandRendering.renderFrameInterval = _interval;  // Application.targetFrameRate / _interval fps
    }
}
