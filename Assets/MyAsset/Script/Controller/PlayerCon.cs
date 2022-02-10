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
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("Death"));
                break;
        }
    }

    protected override void UpdateJumpState(EntityJumpState _state)
    {
        base.UpdateJumpState(_state);

        switch (jumpState)
        {
            case EntityJumpState.PrepareToJump:
                playerkey_jump = true;
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("jump"));
                break;
            case EntityJumpState.Grounded:
                playerkey_jump = false;
                playerkey_down = false;
                break;
            case EntityJumpState.Landed:
                playerkey_jump = false;
                playerkey_down = false;

                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("LandOnGround"));
                break;
        }
    }

    protected override void Update()
    {
        if (Input.GetButtonDown("Down"))
        {
            playerkey_down = true;
        }

        if (jumpState == EntityJumpState.Grounded && Input.GetButtonDown("Jump"))
        {
            UpdateJumpState(EntityJumpState.PrepareToJump);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (jumpState == EntityJumpState.Jumping || jumpState == EntityJumpState.InFlight)
            {
                stopJump = true;
            }
        }

        if (foot_col_src.type == COLTYPE.COLLISION)
        {
            if (foot_col_src.state == ColHitState.Enter)
            {
                //몬스터 충돌 판정
                if (jumpState == EntityJumpState.InFlight)
                    if (foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        foot_col_src.other_col.gameObject.SetActive(false);
                        UpdateJumpState(EntityJumpState.PrepareToJump);
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

    public override void Move(float _horizontal)
    {
        if (_horizontal != 0)
            if (jumpState == EntityJumpState.Grounded)
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("Walk02"));
        base.Move(_horizontal);
    }
}
