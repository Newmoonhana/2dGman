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

    public enum JumpState
    {
        Grounded,   //����
        PrepareToJump,  //���� �Է�
        Jumping,    //����(���) ��
        InFlight,   //�߶�
        Landed  //���� �浹
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
    protected JumpState jumpState;
    public GameObject kine_obj;
    Transform kine_tns;
    public IsColliderHit foot_col_src; // ���� ��� �ݶ��̴� �浹 ���� ��ũ��Ʈ
    public Animator kine_ani;
    public Rigidbody2D rigid;
    public SPEEDTYPE speed;
    float GetSpeed { get { return (int)speed * 3f; } }
    protected bool inFlight, stopJump;
    public float jumpPower;
    protected float jumpTime = 0;
    public float jumpTimeLimit;

    Vector3 movement;
    protected bool IsJumping = false;
    protected bool IsGrounded = true;

    protected virtual void Awake()
    {
        UpdateState(EntityState.DEFAULT);
        UpdateJumpState(JumpState.Grounded);
        kine_tns = kine_obj.transform;
    }

    protected virtual void Update()
    {
        if (foot_col_src.type == COLTYPE.COLLISION)
        {
            if (foot_col_src.state == ColHitState.Enter)
            {
                //�� �浹 ����
                if (foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Land"))
                {
                    IsGrounded = true;
                    kine_ani.SetBool("grounded", true);
                }
                else if (foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Dead Zone"))  //���� ��(�߶���) ����
                {
                    kine_ani.SetTrigger("hurt");
                    kine_ani.SetBool("dead", true);
                    UpdateState(EntityState.DIE);
                }
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (jumpState == JumpState.Jumping || jumpState == JumpState.InFlight)
            if (IsGrounded)
            {
                UpdateJumpState(JumpState.Landed);
            }

        Jump();
    }

    protected virtual void UpdateState(EntityState _state)
    {
        state = _state;

        switch (state)
        {
            case EntityState.HURT:
                kine_ani.SetTrigger("hurt");
                break;
            case EntityState.DIE:
                kine_ani.SetBool("dead", true);
                break;
        }
    }

    protected virtual void UpdateJumpState(JumpState _state)
    {
        jumpState = _state;

        switch (jumpState)
        {
            case JumpState.Grounded:
                IsJumping = false;
                stopJump = false;
                IsGrounded = true;
                inFlight = false;
                break;
            case JumpState.PrepareToJump:
                kine_ani.SetBool("grounded", false);
                jumpTime = 0;
                IsJumping = true;
                IsGrounded = false;
                stopJump = false;
                break;
            case JumpState.Landed:
                kine_ani.SetBool("grounded", true);
                UpdateJumpState(JumpState.Grounded);
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
        kine_ani.SetFloat("velocityX", Mathf.Abs(_horizontal));
    }

    public virtual void Jump()
    {
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                rigid.velocity = Vector3.zero;
                UpdateJumpState(JumpState.Jumping);
                break;
            case JumpState.Jumping:
                rigid.velocity = Vector3.zero;
                vec2 = Vector2.up;
                vec2.y *= jumpPower * (jumpTime * 0.1f + 1f);
                rigid.AddForce(vec2, ForceMode2D.Impulse);
                jumpTime += Time.fixedDeltaTime;

                if (stopJump || jumpTime >= jumpTimeLimit)  //�϶� üũ
                {
                    rigid.velocity = Vector3.zero;
                    vec2.y *= jumpPower / 10 * (jumpTime * 0.1f + 1f);
                    rigid.AddForce(vec2, ForceMode2D.Impulse);
                    inFlight = true;
                    IsJumping = false;
                    UpdateJumpState(JumpState.InFlight);
                    return;
                }
                break;
        }
    }
}
