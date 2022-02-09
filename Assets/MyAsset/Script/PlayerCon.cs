using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : KineObject
{
    //tmp
    Vector2 _vec2;

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
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("jump"));
                break;
        }
    }

    protected override void Update()
    {
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

        if (foot_col_src.type == COLTYPE.COLLISION)
        {
            if (foot_col_src.state == ColHitState.Enter)
            {
                //몬스터 충돌 판정
                if (jumpState == JumpState.InFlight)
                    if (foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        foot_col_src.other_col.gameObject.SetActive(false);
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
            Jump();
            base.FixedUpdate();
        }
    }

    //public override void Jump()
    //{
    //    //Debug.Log(jumpState);
    //    switch (jumpState)
    //    {
    //        case JumpState.PrepareToJump:
    //            rigid.velocity = Vector3.zero;
    //            break;
    //        case JumpState.Jumping:
    //            rigid.velocity = Vector3.zero;
    //            _vec2 = Vector2.up;
    //            _vec2.y *= jumpPower * (jumpTime * 0.1f + 1f);
    //            rigid.AddForce(_vec2, ForceMode2D.Impulse);
    //            jumpTime += Time.fixedDeltaTime;

    //            if (stopJump || jumpTime >= jumpTimeLimit)  //하락 체크
    //            {
    //                rigid.velocity = Vector3.zero;
    //                _vec2.y *= jumpPower / 10 * (jumpTime * 0.1f + 1f);
    //                rigid.AddForce(_vec2, ForceMode2D.Impulse);
    //                inFlight = true;
    //                jumpTime = 0;
    //                return;
    //            }
    //            break;
    //    }
    //}
}
