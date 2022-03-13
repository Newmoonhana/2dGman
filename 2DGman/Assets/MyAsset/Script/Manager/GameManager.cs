using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    //tmp

    public static GameManager Instance;
    int _interval;
    public static WaitForSeconds waitforseconds_3f = new WaitForSeconds(3f);

    public void Awake()
    {
        Instance = this;
        //if (!DontDestroyInst(this))
        //    return;

        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    /// <summary>
    /// ����ȭ - ������ ����(���� �������� �ʿ����� �ʴ� ��� ������ �� ���߱�)
    /// </summary>
    /// <param name="_isdown">������ �� ���߱�</param>
    public void Setting_Frame(bool _isdown)
    {
        Application.targetFrameRate = 60;   //�⺻ fps
        _interval = 1;  // 60 / 1 = �⺻ fps
        if (_isdown)
            _interval = 3;  //���� fps
        OnDemandRendering.renderFrameInterval = _interval;  // Application.targetFrameRate / _interval fps
    }
}
