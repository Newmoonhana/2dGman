using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineObject : MonoBehaviour
{
    public enum SPEEDTYPE
    {
        NONE,
        SLOW,
        NORMAL,
        FAST,

        _MAX
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
    float f;

    protected EntityState state;
    protected EntityJumpState jumpState;
    public GameObject kine_obj;
    Transform kine_tns;
    public IsColliderHit foot_col_src; // 땅에 닿는 콜라이더 충돌 정보 스크립트
    public Animator kine_ani;
    public Rigidbody2D rigid;
    public SPEEDTYPE speed;
    float GetSpeed { get { return (int)speed * 3f; } }
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
        kine_ani.SetBool("_grounded", true);
        kine_tns = kine_obj.transform;
    }

    protected virtual void Update()
    {
        if (foot_col_src.type == COLTYPE.COLLISION)
        {
            if (foot_col_src.other_col == null)
                return;
            if (foot_col_src.state == ColHitState.Enter)
            {
                
                if (foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Dead Zone"))  //데드 존(추락사) 판정
                {
                    UpdateState(EntityState.HURT);
                    UpdateState(EntityState.DIE);
                }
            }
            else if (foot_col_src.state == ColHitState.Stay)
            {
                //땅 충돌 판정
                if (foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Land") || foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Land_Platform"))
                    if (jumpState == EntityJumpState.InFlight)
                    {
                        UpdateJumpState(EntityJumpState.Landed);
                    }
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (state == EntityState.DEFAULT)
        {
            Jump();
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
                IsGrounded = false;
                stopJump = false;
                break;
            case EntityJumpState.Landed:
                UpdateJumpState(EntityJumpState.Grounded);
                break;
        }
    }

    public virtual void Move(float _horizontal)
    {
        movement = Vector3.zero;
        vec2 = kine_tns.localScale;
        f = Mathf.Abs(kine_tns.localScale.x);
        if (_horizontal < 0)
        {
            movement = Vector3.left;
            vec2.x = -f;
            kine_tns.localScale = vec2;
        }
        else if (_horizontal > 0)
        {
            movement = Vector3.right;
            vec2.x = f;
            kine_tns.localScale = vec2;
        }

        kine_obj.transform.position += movement * GetSpeed * Time.deltaTime;
        kine_ani.SetFloat("_velocityX", Mathf.Abs(_horizontal));
    }

    public virtual void Jump()
    {
        switch (jumpState)
        {
            case EntityJumpState.PrepareToJump:
                rigid.velocity = Vector3.zero;
                UpdateJumpState(EntityJumpState.Jumping);
                break;
            case EntityJumpState.Jumping:
                rigid.velocity = Vector3.zero;
                vec2 = Vector2.up;
                vec2.y *= jumpPower * (jumpTime * 0.1f + 1f);
                rigid.AddForce(vec2, ForceMode2D.Impulse);
                jumpTime += Time.fixedDeltaTime;

                if (stopJump || jumpTime >= jumpTimeLimit)  //하락 체크
                {
                    rigid.velocity = Vector3.zero;
                    vec2.y *= jumpPower / 10 * (jumpTime * 0.1f + 1f);
                    rigid.AddForce(vec2, ForceMode2D.Impulse);
                    UpdateJumpState(EntityJumpState.InFlight);
                    return;
                }
                break;
        }
    }
}
