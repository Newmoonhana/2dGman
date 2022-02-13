using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineObject : MonoBehaviour
{
    protected enum MOVEDIRTYPE
    {
        LEFT,
        CENTER,
        RIGHT
    }

    public enum EntityJumpState
    {
        Grounded,   //지면
        PrepareToJump,  //점프 입력
        Jumping,    //점프(상승) 중
        InFlight,   //추락
        Landed  //땅에 충돌
    }

    public enum EntityState
    {
        DEFAULT,
        HURT,
        DIE,
        VICTORY
    }

    //tmp
    Vector2 vec2;
    float f, f1;

    protected EntityState state;
    protected MOVEDIRTYPE movedir = MOVEDIRTYPE.CENTER;
    public EntityJumpState jumpState;
    public GameObject kine_obj;
    Transform kine_tns;
    public IsColliderHit foot_col_src; // 땅에 닿는 콜라이더 충돌 정보 스크립트
    public Animator kine_ani;
    public Rigidbody2D rigid;
    public SPEEDTYPE speed;
    protected bool stopJump;
    public float jumpPower;
    protected float jumpTime = 0;
    public float jumpTimeLimit;

    Vector3 movement;
    protected bool IsGrounded = true;

    protected virtual void Awake()
    {
        UpdateState(EntityState.DEFAULT);
        UpdateJumpState(EntityJumpState.Grounded);
        kine_tns = kine_obj.transform;
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        if (state == EntityState.DEFAULT)
        {
            Jump();

            if (rigid.velocity.y < 0)
            {
                if (jumpState == EntityJumpState.Jumping)
                    UpdateJumpState(EntityJumpState.InFlight);
            }
        }

        if (foot_col_src.type == COLTYPE.TRIGGER)
        {
            if (foot_col_src.other_col_TRIGGER == null)
                return;
            if (foot_col_src.state == ColHitState.Enter)
            {
                if (foot_col_src.other_col_TRIGGER.gameObject.layer == LayerMask.NameToLayer("Dead Zone"))  //데드 존(추락사) 판정
                {
                    UpdateState(EntityState.HURT);
                    UpdateState(EntityState.DIE);
                }
            }
            else if (foot_col_src.state == ColHitState.Stay)
            {
                //땅 충돌 판정
                if (foot_col_src.other_col_TRIGGER.gameObject.layer == LayerMask.NameToLayer("Land") || foot_col_src.other_col_TRIGGER.gameObject.layer == LayerMask.NameToLayer("Land_Platform"))
                {
                    if (jumpState == EntityJumpState.InFlight)
                    {
                        rigid.velocity = Vector3.zero;
                        UpdateJumpState(EntityJumpState.Landed);
                    }
                }
            }
            else if (foot_col_src.state == ColHitState.Exit)
            {
                //땅 충돌 판정(추락)
                if (foot_col_src.other_col_TRIGGER.gameObject.layer == LayerMask.NameToLayer("Land") || foot_col_src.other_col_TRIGGER.gameObject.layer == LayerMask.NameToLayer("Land_Platform"))
                {
                    if (jumpState == EntityJumpState.Grounded)
                    {
                        UpdateJumpState(EntityJumpState.InFlight);
                    }
                        
                }
            }
        }
    }

    protected virtual void UpdateState(EntityState _state)
    {
        state = _state;

        switch (state)
        {
            case EntityState.HURT:
                kine_ani.SetTrigger("_hurt");
                break;
            case EntityState.DIE:
                kine_ani.SetBool("_dead", true);
                break;
        }
    }

    protected virtual void UpdateJumpState(EntityJumpState _state)
    {
        jumpState = _state;

        switch (jumpState)
        {
            case EntityJumpState.Grounded:
                IsGrounded = true;
                kine_ani.SetBool("_grounded", true);
                stopJump = false;
                break;
            case EntityJumpState.PrepareToJump:
                kine_ani.SetBool("_grounded", false);
                jumpTime = 0;
                stopJump = false;
                break;
            case EntityJumpState.InFlight:
                IsGrounded = false;
                kine_ani.SetBool("_grounded", false);
                kine_ani.SetFloat("_velocityY", 0);
                break;
            case EntityJumpState.Landed:
                UpdateJumpState(EntityJumpState.Grounded);
                break;
        }
    }

    public virtual void Move(float _horizontal)
    {
        //if (jumpState == EntityJumpState.Grounded)    //점프 후 방향 전환 안되는 점프 원하면 이 코드 사용하면 됨(이 프로젝트에선 필요 없어서 주석 처리)
        //{
        movedir = MOVEDIRTYPE.CENTER;
        if (_horizontal < 0)
        {
            movedir = MOVEDIRTYPE.LEFT;
        }
        else if (_horizontal > 0)
        {
            movedir = MOVEDIRTYPE.RIGHT;
        }
        //}

        if (movedir == MOVEDIRTYPE.CENTER)
        {
            kine_ani.SetFloat("_velocityX", 0);
            return;
        }

        movement = Vector3.zero;
        vec2 = kine_tns.localScale;
        f = Mathf.Abs(kine_tns.localScale.x);
        f1 = 1;
        switch (jumpState)
        {
            case EntityJumpState.Jumping:
                f1 = 0.6f;
                break;
            case EntityJumpState.InFlight:
                f1 = 0.4f;
                break;
        }
        if (movedir == MOVEDIRTYPE.LEFT)
        {
            movement = Vector3.left;
            vec2.x = -f;
            kine_tns.localScale = vec2;
            vec2.x = -f1;
        }
        else if (movedir == MOVEDIRTYPE.RIGHT)
        {
            movement = Vector3.right;
            vec2.x = f;
            kine_tns.localScale = vec2;
            vec2.x = f1;
        }
        movement.x = vec2.x;

        kine_obj.transform.position += movement * GameSceneData.GetSpeed(speed) * Time.deltaTime;
        kine_ani.SetFloat("_velocityX", Mathf.Abs((int)movedir - 1));
    }

    public virtual void Jump()
    {
        switch (jumpState)
        {
            case EntityJumpState.PrepareToJump:
                if (!IsGrounded)
                    break;
                rigid.velocity = Vector3.zero;
                UpdateJumpState(EntityJumpState.Jumping);
                return;
            case EntityJumpState.Jumping:
                if (stopJump || jumpTime >= jumpTimeLimit)  //하락 체크
                    return;

                rigid.velocity = Vector3.zero;
                vec2 = Vector2.up;
                vec2.y *= jumpPower * (jumpTime * 0.1f + 1f);
                rigid.AddForce(vec2, ForceMode2D.Impulse);
                jumpTime += Time.fixedDeltaTime;
                kine_ani.SetFloat("_velocityY", Mathf.Abs(jumpTime));
                return;
        }
    }
}
