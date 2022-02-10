using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : KineObject
{
    //tmp

    public bool playerkey_jump = false, playerkey_down = false;

    protected override void UpdateState(EntityState _state)
    {
        base.UpdateState(_state);

        switch (state)
        {
            case EntityState.HURT:
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("Hurt"));
                break;
            case EntityState.DIE:
                break;
        }
    }

    protected override void UpdateJumpState(JumpState _state)
    {
        base.UpdateJumpState(_state);

        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                playerkey_jump = true;
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("jump"));
                break;
            case JumpState.Grounded:
            case JumpState.Landed:
                playerkey_jump = false;
                playerkey_down = false;
                break;
        }
    }

    protected override void Update()
    {
        if (Input.GetButtonDown("Down"))
        {
            playerkey_down = true;
        }

        if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
        {
            UpdateJumpState(JumpState.PrepareToJump);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (jumpState == JumpState.Jumping || jumpState == JumpState.InFlight)
            {
                stopJump = true;
            }
        }

        if (kine_col_src.type == COLTYPE.COLLISION)
        {
            if (kine_col_src.state == ColHitState.Enter)
            {
                //몬스터 충돌 판정
                if (jumpState == JumpState.InFlight)
                    if (kine_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        kine_col_src.other_col.gameObject.SetActive(false);
                        UpdateJumpState(JumpState.PrepareToJump);
                    }
            }
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (state == EntityState.DEFAULT)
        {
            Move(Input.GetAxisRaw("Horizontal"));
        }
        base.FixedUpdate();
    }
}
