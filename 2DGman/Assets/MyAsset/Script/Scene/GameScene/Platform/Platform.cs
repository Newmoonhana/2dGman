using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    //tmp
    
    bool playerCheck;
    PlatformEffector2D LandPlatform_Pe2d;

    private void Start()
    {
        playerCheck = true;
        LandPlatform_Pe2d = GetComponent<PlatformEffector2D>();
    }

    protected virtual void Update()
    {
        if (GameSceneData.playerCon_src.playerkey_down)
        {
            if (playerCheck)
                LandPlatform_Pe2d.rotationalOffset = 180f;
        }
        if (GameSceneData.playerCon_src.playerkey_jump)
        {
            LandPlatform_Pe2d.rotationalOffset = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].point.y > transform.position.y)
            Debug.Log("Land");
        
        playerCheck = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        playerCheck = false;
    }
}
