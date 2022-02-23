using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    //tmp
    
    bool playerCheck = false;
    PlatformEffector2D platform_Pe2d;
    bool isjumpCheck = false;

    PlayerController pc;

    private void Start()
    {
        platform_Pe2d = GetComponent<PlatformEffector2D>();
        pc = GameSceneData.player_controller;
    }

    protected virtual void Update()
    {
        if (UserInputManager.Instance.model.IsButtonInput("Down", UserInputModel.inputItem.TYPE.BUTTONDOWN))
        {
            if (playerCheck)
            {
                if (PlayerController.player_model.playerCon_src.model.jumpState == EntityModel.EntityJumpState.Grounded)
                {
                    platform_Pe2d.colliderMask = 1 << LayerMask.NameToLayer("Foot_Enemy");
                    isjumpCheck = false;
                }
            }
        }
            
        if (!isjumpCheck)
            if (PlayerController.player_model.playerCon_src.model.jumpState == EntityModel.EntityJumpState.PrepareToJump)
            {
                platform_Pe2d.colliderMask = (1 << LayerMask.NameToLayer("Foot_Player")) + (1 << LayerMask.NameToLayer("Foot_Enemy"));
                isjumpCheck = true;
            }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "foot")
        {
            EntityController con;
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Foot_Player"))
            {
                con = pc;
                playerCheck = true;
            }
            else
                con = collision.collider.transform.parent.GetComponent<EntityController>();

            if (con.model.entity_obj.activeSelf)
                con.model.entity_tns.parent = transform;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "foot")
        {
            EntityController con;
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Foot_Player"))
            {
                con = pc;
                playerCheck = false;
            }
            else
                con = collision.collider.transform.parent.GetComponent<EntityController>();

            if (con.model.entity_obj.activeSelf)
                con.model.entity_tns.parent = con.model.parent_tns;
        }
    }
}
