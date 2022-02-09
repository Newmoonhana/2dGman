using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : KineObject
{
    public enum JumpState
    {
        Grounded,   //지면
        PrepareToJump,  //점프 입력
        Jumping,    //점프(상승) 중
        InFlight,   //추락
        Landed  //땅에 충돌
    }
    //tmp
    Vector2 _vec2;

    JumpState jumpState = JumpState.Grounded;
    bool inFlight, stopJump;

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

    void UpdateJumpState()
    {
        switch (jumpState)
        {
            case JumpState.Grounded:
                IsJumping = false;
                stopJump = false;
                IsGrounded = true;
                inFlight = false;
                break;
            case JumpState.PrepareToJump:
                jumpState = JumpState.Jumping;
                kine_ani.SetBool("grounded", false);
                IsJumping = true;
                IsGrounded = false;
                stopJump = false;
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("jump"));
                break;
            case JumpState.Jumping:
                if (inFlight)
                {
                    IsJumping = false;
                    jumpState = JumpState.InFlight;
                }
                break;
            case JumpState.InFlight:
                if (IsGrounded)
                {
                    jumpState = JumpState.Landed;
                }
                break;
            case JumpState.Landed:
                kine_ani.SetBool("grounded", true);
                jumpState = JumpState.Grounded;
                break;
        }
    }

    protected override void Update()
    {
        if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
        {
            jumpState = JumpState.PrepareToJump;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (jumpState == JumpState.Jumping || jumpState == JumpState.InFlight)
            {
                stopJump = true;
            }
        }

        UpdateJumpState();

        if (foot_col_src.type == COLTYPE.COLLISION)
        {
            if (foot_col_src.state == ColHitState.Enter)
            {
                //몬스터 충돌 판정
                if (jumpState == JumpState.InFlight)
                    if (foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        foot_col_src.other_col.gameObject.SetActive(false);
                        jumpState = JumpState.PrepareToJump;
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

    public override void Jump()
    {
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                rigid.velocity = Vector3.zero;
                break;
            case JumpState.Jumping:
                rigid.velocity = Vector3.zero;
                _vec2 = Vector2.up;
                _vec2.y *= jumpPower * (jumpTime * 0.1f + 1f);
                rigid.AddForce(_vec2, ForceMode2D.Impulse);
                jumpTime += Time.fixedDeltaTime;

                if (stopJump || jumpTime >= jumpTimeLimit)  //하락 체크
                {
                    rigid.velocity = Vector3.zero;
                    _vec2.y *= jumpPower / 10 * (jumpTime * 0.1f + 1f);
                    rigid.AddForce(_vec2, ForceMode2D.Impulse);
                    inFlight = true;
                    jumpTime = 0;
                    return;
                }
                break;
        }
    }
}
