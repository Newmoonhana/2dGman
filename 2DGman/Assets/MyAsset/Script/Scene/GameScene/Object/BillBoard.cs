using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillBoard : MonoBehaviour
{
    public RenderTexture rt;
    IsDistance isDist_src;
    GameObject rawimage_canvas_obj;
    GameObject rawimage_obj_obj;
    RawImage rawimage;
    Camera this_camera;

    private void Start()
    {
        isDist_src = GetComponent<IsDistance>();
        rawimage_canvas_obj = transform.GetChild(1).gameObject;
        rawimage_obj_obj = transform.GetChild(2).gameObject;
        rawimage = rawimage_canvas_obj.transform.GetChild(0).GetComponent<RawImage>();
        RenderTexture rt_tmp = new RenderTexture(rt);
        rawimage.texture = rt_tmp;
        this_camera = rawimage_obj_obj.transform.GetChild(0).GetComponent<Camera>();
        this_camera.targetTexture = rt_tmp;
    }

    void Update()
    {
        if (isDist_src.isDist)
        {
            if (!rawimage_canvas_obj.activeSelf)
                rawimage_canvas_obj.SetActive(true);
        }
        else
        {
            if (rawimage_canvas_obj.activeSelf)
                rawimage_canvas_obj.SetActive(false);
        }
    }
}
