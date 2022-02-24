using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    IsDistance isDist_src;
    GameObject rowimage_canvas_obj;
    GameObject rowimage_obj_obj;

    private void Start()
    {
        isDist_src = GetComponent<IsDistance>();
        rowimage_canvas_obj = transform.GetChild(1).gameObject;
        rowimage_obj_obj = transform.GetChild(2).gameObject;
    }

    void Update()
    {
        if (isDist_src.isDist)
        {
            if (!rowimage_canvas_obj.activeSelf)
                rowimage_canvas_obj.SetActive(true);
           // if (!rowimage_obj_obj.activeSelf)
           //     rowimage_obj_obj.SetActive(true);
        }
        else
        {
            if (rowimage_canvas_obj.activeSelf)
                rowimage_canvas_obj.SetActive(false);
            //if (rowimage_obj_obj.activeSelf)
            //    rowimage_obj_obj.SetActive(false);
        }
    }
}
