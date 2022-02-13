using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : KineObject
{
    //tmp
    Vector2 _vec2;

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
        switch (_state)
        {
            case EntityJumpState.Grounded:
                playerkey_jump = false;
                playerkey_down = false;
                break;
            case EntityJumpState.PrepareToJump:
                playerkey_jump = true;
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("jump"));
                break;
            case EntityJumpState.Landed:
                playerkey_jump = false;
                playerkey_down = false;
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("LandOnGround"));
                break;
        }
        base.UpdateJumpState(_state);
    }

    protected override void Update()
    {
        if (Input.GetButtonDown("Down"))
        {
            playerkey_down = true;
            _vec2 = Vector2.down;
            _vec2.y *= jumpPower / 10 * (jumpTime * 0.1f + 1f);
            rigid.AddForce(_vec2, ForceMode2D.Impulse);
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

        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (state == EntityState.DEFAULT)
        {
            Move(Input.GetAxisRaw("Horizontal"));
        }

        if (foot_col_src.type == COLTYPE.TRIGGER)
        {
            if (foot_col_src.state == ColHitState.Enter)
            {
                //몬스터 충돌 판정
                if (foot_col_src.other_col_TRIGGER.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    foot_col_src.other_col_TRIGGER.gameObject.SetActive(false);
                    UpdateJumpState(EntityJumpState.Grounded);
                    UpdateJumpState(EntityJumpState.PrepareToJump);
                }
            }
        }

        base.FixedUpdate();
    }

    public override void Move(float _horizontal)
    {
        if (_horizontal != 0)
            if (jumpState == EntityJumpState.Grounded)
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("Walk01"));
        base.Move(_horizontal);
    }
}
