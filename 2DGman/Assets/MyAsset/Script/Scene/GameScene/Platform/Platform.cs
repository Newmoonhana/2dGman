using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    //tmp
    
    bool playerCheck = false;
    PlatformEffector2D platform_Pe2d;
    bool isjumpCheck = false;

    private void Start()
    {
        platform_Pe2d = GetComponent<PlatformEffector2D>();
    }

    protected virtual void Update()
    {
        if (GameSceneData.playerCon_src.IsDown)
        {
            if (playerCheck)
            {
                if (GameSceneData.playerCon_src.jumpState == KineObject.EntityJumpState.Grounded)
                {
                    platform_Pe2d.colliderMask = 0;
                    isjumpCheck = false;
                }
            }
        }
            
        if (!isjumpCheck)
            if (GameSceneData.playerCon_src.IsJump)
            {
                platform_Pe2d.colliderMask = 1 << LayerMask.NameToLayer("Foot_Player");
                isjumpCheck = true;
            }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Foot_Player"))
            playerCheck = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Foot_Player"))
        {
            playerCheck = false;
        }
    }
}
