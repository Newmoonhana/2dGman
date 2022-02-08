using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPEEDTYPE
{
    NONE,
    SLOW,
    NORMAL,
    FAST,

    _MAX
}

public class KineObject : MonoBehaviour
{
    //tmp
    Vector2 vec2;
    float f;

    public GameObject kine_obj;
    Transform kine_tns;
    public IsColliderHit foot_col_src; // 땅에 닿는 콜라이더 충돌 정보 스크립트
    public Animator kine_ani;
    public Rigidbody2D rigid;
    public SPEEDTYPE speed;
    float GetSpeed { get { return (int)speed * 3f; } }
    public float jumpPower;
    protected float jumpTime = 0;
    public float jumpTimeLimit;

    Vector3 movement;
    protected bool IsJumping = false;
    protected bool IsGrounded = true;

    private void Awake()
    {
        kine_tns = kine_obj.transform;
    }

    protected virtual void Update()
    {
        //땅 충돌 판정
        if (foot_col_src.type == COLTYPE.COLLISION)
            if (foot_col_src.state == ColHitState.Enter)
            {
                if (foot_col_src.other_col.collider.gameObject.layer == LayerMask.NameToLayer("Land"))
                {
                    IsGrounded = true;
                    kine_ani.SetBool("grounded", true);
                }
            }
    }

    protected virtual void FixedUpdate()
    {
        
    }

    public void Move(float _horizontal)
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
        kine_ani.SetFloat("velocityX", _horizontal);
    }

    public virtual void Jump()
    {
        if (!IsJumping)
            return;

        if (jumpTime == 0)  //점프 시작 시
        {
            IsGrounded = false;
            kine_ani.SetBool("grounded", false);
        }

        if (jumpTime >= jumpTimeLimit)  //하락 체크
        {
            IsJumping = false;
            jumpTime = 0;

            return;
        }
        vec2 = Vector2.up;
        vec2.y *= jumpPower * (jumpTime * 0.1f + 1f);
        rigid.AddForce(vec2, ForceMode2D.Impulse);
        jumpTime += Time.deltaTime;
    }
}
