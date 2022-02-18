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

    [SerializeField] public IEntityMovableStrategy movableStrategy;

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
        entity_ani = entity_obj.GetComponent<Animator>();
        footcol = entity_tns.GetChild(1).GetComponent<Collider2D>();
        foot_col_src = footcol.GetComponent<IsColliderHit>();
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

public class EntityController : MonoBehaviour, IEntityMovableStrategy
{
    public EntityModel model = new EntityModel();
    [SerializeField] GameObject entity_obj;
    public GameObject Entity_obj { get { return model.entity_obj; } set { entity_obj = value; model.SetEntityObject(value); } }
    //tmp
    

    protected float jumpTime = 0;

    protected virtual void Start()
    {
        model.SetEntityObject(entity_obj);

        UpdateState(EntityModel.EntityState.DEFAULT);
        UpdateJumpState(EntityModel.EntityJumpState.Grounded, model, ref jumpTime);
    }

    protected virtual void FixedUpdate()
    {
        if (model.state == EntityModel.EntityState.DEFAULT)
        {
            Jump(model, ref jumpTime);

            // 점프 시 추락 체크
            if (model.rigid.velocity.y < 0)
            {
                if (model.jumpState == EntityModel.EntityJumpState.Jumping)
                    UpdateJumpState(EntityModel.EntityJumpState.InFlight, model, ref jumpTime);
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
                            UpdateJumpState(EntityModel.EntityJumpState.Landed, model, ref jumpTime);
                        }
                    }
                    else if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Dead Zone"))  //데드 존(추락사) 판정
                    {
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
                            UpdateJumpState(EntityModel.EntityJumpState.InFlight, model, ref jumpTime);
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

    public void UpdateJumpState(EntityModel.EntityJumpState s, EntityModel m, ref float j)
    {
        if (m.movableStrategy != null)
            m.movableStrategy.UpdateJumpState(s, m, ref j);
    }

    public void Move(float f, EntityModel m)
    {
        if (m.movableStrategy != null)
            m.movableStrategy.Move(f, m);
    }

    public void Jump(EntityModel m, ref float j)
    {
        if (m.movableStrategy != null)
            m.movableStrategy.Jump(m, ref j);
    }
}
