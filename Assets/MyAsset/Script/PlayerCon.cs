using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : KineObject
{
    public enum JumpState
    {
        Grounded,
        PrepareToJump,
        Jumping,
        InFlight,
        Landed
    }
    //tmp

    JumpState jumpState = JumpState.Grounded;
    bool stopJump;

    void UpdateJumpState()
    {
        switch (jumpState)
        {
            case JumpState.Grounded:
                IsJumping = false;
                stopJump = false;
                IsGrounded = true;
                break;
            case JumpState.PrepareToJump:
                jumpState = JumpState.Jumping;
                IsJumping = true;
                stopJump = false;
                break;
            case JumpState.Jumping:
                if (!IsGrounded)
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
}
