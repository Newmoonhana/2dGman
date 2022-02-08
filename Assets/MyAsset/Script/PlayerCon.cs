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
                break;
            case JumpState.Jumping:
                if (inFlight)
                {
                    jumpState = JumpState.InFlight;
                }
                break;
            case JumpState.InFlight:
                if (IsGrounded)
                {
                    IsJumping = false;
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
            stopJump = true;
        }

        UpdateJumpState();

        base.Update();
    }

    protected override void FixedUpdate()
    {
        Move(Input.GetAxisRaw("Horizontal"));
        Jump();
        base.FixedUpdate();
    }

    public override void Jump()
    {
        if (!IsJumping)
            return;

        if (jumpTime == 0)  //점프 시작 시
        {
            
        }

        if (stopJump || jumpTime >= jumpTimeLimit)  //하락 체크
        {
            inFlight = true;
            jumpTime = 0;

            return;
        }
        _vec2 = Vector2.up;
        _vec2.y *= jumpPower * (jumpTime * 0.1f + 1f);
        rigid.AddForce(_vec2, ForceMode2D.Impulse);
        jumpTime += Time.deltaTime;
    }
}
