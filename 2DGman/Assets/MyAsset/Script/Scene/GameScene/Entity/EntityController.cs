using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityModel
{
    public enum MOVEDIRTYPE
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

    public EntityState state;
    public MOVEDIRTYPE movedir = MOVEDIRTYPE.CENTER;
    public EntityJumpState jumpState;

    //entity 관련
    public GameObject entity_obj;
    public Transform entity_tns;
    public Collider2D footcol;
    public IsColliderHit foot_col_src; // 땅에 닿는 콜라이더 충돌 정보 스크립트
    public Animator entity_ani;
    public Rigidbody2D rigid;

    //부가적인 스탯 변수
    public SPEEDTYPE speed;
    public float jumpPower, jumpTimeLimit;

    public bool stopJump;
    public bool IsGrounded = true;

    /// <summary>
    /// entity_obj를 기준으로 다른 변수들도 대입해주는 함수
    /// </summary>
    /// <param name="_obj">기반이 되는 게임 오브젝트(=entity_obj)</param>
    public void SetEntityObject(GameObject _obj)
    {
        if (_obj == null)
        {
            Debug.LogWarning("Entity Model에 _obj가 대입되지 못했습니다.");
            return;
        }
            
        entity_obj = _obj;
        entity_tns = entity_obj.transform;
        footcol = entity_tns.GetChild(1).GetComponent<Collider2D>();
        foot_col_src = footcol.GetComponent<IsColliderHit>();
        entity_ani = entity_tns.GetChild(0).GetComponent<Animator>();
        rigid = entity_obj.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 부가적인 스탯 변수 대입
    /// </summary>
    /// <param name="_type">스피드 값(SPEEDTYPE)</param>
    /// <param name="_jumpPower">점프 강도</param>
    /// <param name="_jumpTimeLimit">점프 시간 최대치</param>
    public void SetSubValue(SPEEDTYPE _type, float _jumpPower, float _jumpTimeLimit)
    {
        speed = _type;
        jumpPower = _jumpPower;
        jumpTimeLimit = _jumpTimeLimit;
    }
}

public class EntityController : MonoBehaviour
{
    public EntityModel model = new EntityModel();
    [SerializeField] GameObject entity_obj;
    public GameObject Entity_obj { get { return model.entity_obj; } set { entity_obj = value; model.SetEntityObject(value); } }
    //tmp
    Vector2 vec2;
    Vector3 vec3;
    float f, f1;

    protected float jumpTime = 0;

    protected virtual void Start()
    {
        model.SetEntityObject(entity_obj);

        UpdateState(EntityModel.EntityState.DEFAULT);
        UpdateJumpState(EntityModel.EntityJumpState.Grounded);
    }

    protected virtual void FixedUpdate()
    {
        if (model.state == EntityModel.EntityState.DEFAULT)
        {
            Jump();

            if (model.rigid.velocity.y < 0)
            {
                if (model.jumpState == EntityModel.EntityJumpState.Jumping)
                    UpdateJumpState(EntityModel.EntityJumpState.InFlight);
            }

            if (model.foot_col_src.type == COLTYPE.COLLISION)
            {
                if (model.foot_col_src.other_col_COLLISION == null)
                    return;
                if (model.foot_col_src.state == ColHitState.Enter)
                {
                    //땅 충돌 판정
                    if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Land") || model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Platform"))
                    {  
                        if (model.jumpState == EntityModel.EntityJumpState.InFlight)
                        {
                            model.rigid.velocity = Vector3.zero;
                            UpdateJumpState(EntityModel.EntityJumpState.Landed);
                        }
                    }
                    else if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Dead Zone"))  //데드 존(추락사) 판정
                    {
                        UpdateState(EntityModel.EntityState.HURT);
                        UpdateState(EntityModel.EntityState.DIE);
                    }
                }
                //else if (model.foot_col_src.state == ColHitState.Stay)
                //{

                //}
                else if (model.foot_col_src.state == ColHitState.Exit)
                {
                    //땅 충돌 판정(추락)
                    if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Land") || model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Platform"))
                    {
                        if (model.jumpState == EntityModel.EntityJumpState.Grounded)
                        {
                            UpdateJumpState(EntityModel.EntityJumpState.InFlight);
                        }
                    }
                }
            }
        }
    }

    protected virtual void UpdateState(EntityModel.EntityState _state)
    {
        model.state = _state;

        switch (model.state)
        {
            case EntityModel.EntityState.HURT:
                model.entity_ani.SetTrigger("_hurt");
                break;
            case EntityModel.EntityState.DIE:
                model.entity_ani.SetBool("_dead", true);
                break;
        }
    }

    protected virtual void UpdateJumpState(EntityModel.EntityJumpState _state)
    {
        model.jumpState = _state;
        
        switch (model.jumpState)
        {
            case EntityModel.EntityJumpState.Grounded:
                model.IsGrounded = true;
                model.entity_ani.SetBool("_grounded", true);
                model.stopJump = false;
                break;
            case EntityModel.EntityJumpState.PrepareToJump:
                model.entity_ani.SetBool("_grounded", false);
                jumpTime = 0;
                model.stopJump = false;
                model.footcol.enabled = false;
                break;
            case EntityModel.EntityJumpState.InFlight:
                model.IsGrounded = false;
                model.entity_ani.SetBool("_grounded", false);
                model.entity_ani.SetFloat("_velocityY", 0);
                model.footcol.enabled = true;
                break;
            case EntityModel.EntityJumpState.Landed:
                UpdateJumpState(EntityModel.EntityJumpState.Grounded);
                break;
        }
    }

    public virtual void Move(float _horizontal)
    {
        //if (jumpState == EntityJumpState.Grounded)    //점프 후 방향 전환 안되는 점프 원하면 이 코드 사용하면 됨(이 프로젝트에선 필요 없어서 주석 처리)
        //{
        model.movedir = EntityModel.MOVEDIRTYPE.CENTER;
        if (_horizontal < 0)
        {
            model.movedir = EntityModel.MOVEDIRTYPE.LEFT;
        }
        else if (_horizontal > 0)
        {
            model.movedir = EntityModel.MOVEDIRTYPE.RIGHT;
        }
        //}

        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
        {
            model.entity_ani.SetFloat("_velocityX", 0);
            return;
        }

        vec3 = Vector3.zero; //이동 좌표 계산
        vec2 = model.entity_tns.localScale; //크기(좌우 반전에 필요)
        f = Mathf.Abs(model.entity_tns.localScale.x);
        f1 = 1;
        switch (model.jumpState)
        {
            case EntityModel.EntityJumpState.Jumping:
                f1 = 0.6f;
                break;
            case EntityModel.EntityJumpState.InFlight:
                f1 = 0.4f;
                break;
        }
        if (model.movedir == EntityModel.MOVEDIRTYPE.LEFT)
        {
            vec3 = Vector3.left;
            vec2.x = -f;
            model.entity_tns.localScale = vec2;
            vec2.x = -f1;
        }
        else if (model.movedir == EntityModel.MOVEDIRTYPE.RIGHT)
        {
            vec3 = Vector3.right;
            vec2.x = f;
            model.entity_tns.localScale = vec2;
            vec2.x = f1;
        }
        vec3.x = vec2.x;

        model.entity_obj.transform.position += vec3 * GameSceneData.GetSpeed(model.speed) * Time.deltaTime;
        model.entity_ani.SetFloat("_velocityX", Mathf.Abs((int)model.movedir - 1));
    }

    public virtual void Jump()
    {
        switch (model.jumpState)
        {
            case EntityModel.EntityJumpState.PrepareToJump:
                if (!model.IsGrounded)
                    break;
                model.rigid.velocity = Vector3.zero;  //점프 Tag를 밟으면 해당 코드 필요
                UpdateJumpState(EntityModel.EntityJumpState.Jumping);
                return;
            case EntityModel.EntityJumpState.Jumping:
                if (model.stopJump || jumpTime >= model.jumpTimeLimit)  //하락 체크
                    return;

                model.rigid.velocity = Vector3.zero;
                vec2 = Vector2.up;
                vec2.y *= model.jumpPower * (jumpTime * 0.1f + 1f);
                model.rigid.AddForce(vec2, ForceMode2D.Impulse);
                jumpTime += Time.fixedDeltaTime;
                model.entity_ani.SetFloat("_velocityY", Mathf.Abs(jumpTime));
                return;
        }
    }
}
