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
        UNBEAT, // 무적시간
        DIE,
        VICTORY
    }

    public EntityState state;
    [HideInInspector] public MOVEDIRTYPE movedir = MOVEDIRTYPE.CENTER;
    [HideInInspector] public EntityJumpState jumpState;

    //entity 관련
    [HideInInspector] public GameObject entity_obj;
    [HideInInspector] public Transform entity_tns;
    [HideInInspector] public SpriteRenderer entity_spr;
    [HideInInspector] public Collider2D entity_col;
    [HideInInspector] public IsColliderHit entity_col_src; // 몸 충돌 콜라이더 충돌 정보 스크립트
    [HideInInspector] public Collider2D footcol;
    [HideInInspector] public IsColliderHit foot_col_src; // 땅에 닿는 콜라이더 충돌 정보 스크립트
    [HideInInspector] public Animator entity_ani;
    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Transform parent_tns;    //부모 트랜스폼 백업(움직이는 발판에서 부모 교체 사용)
    [HideInInspector] public IsDistance cameraDis_src;  //카메라와의 거리 측정 후 거리가 멀다면 움직임 정지

    //부가적인 스탯 변수
    public float hp_max = 3;
    public float hp = 3;

    public SPEEDTYPE speed;
    public float jumpPower, jumpTimeLimit;

    [HideInInspector] public bool stopJump;
    [HideInInspector] public bool IsGrounded = true;

    [SerializeField] public EntityMovableStrategyList movableStrategy;

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
        entity_spr = entity_tns.GetChild(0).GetComponent<SpriteRenderer>();
        entity_ani = entity_obj.GetComponent<Animator>();
        entity_col = entity_obj.GetComponent<Collider2D>();
        entity_col_src = entity_col.GetComponent<IsColliderHit>();
        footcol = entity_tns.GetChild(1).GetComponent<Collider2D>();
        foot_col_src = footcol.GetComponent<IsColliderHit>();
        rigid = entity_obj.GetComponent<Rigidbody2D>();
        parent_tns = entity_tns.parent;

        cameraDis_src = entity_obj.GetComponent<IsDistance>();
        if (cameraDis_src == null)
            cameraDis_src = entity_obj.AddComponent<IsDistance>();
            
        cameraDis_src.entity_tns = Camera.main.transform;
        cameraDis_src.distance = 30;
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
    public EntityModel model;
    [SerializeField] GameObject entity_obj;
    public GameObject Entity_obj { get { return model.entity_obj; } set { entity_obj = value; model.SetEntityObject(value); } }

    protected float jumpTime = 0;

    public virtual bool SetHp(float _hp, float _hpMax, DAMAGETYPE _type) //hp가 0 이하일 경우 false
    {
        model.hp_max = _hpMax;
        if (_type == DAMAGETYPE.DAMAGE)
        {
            model.hp -= _hp;
            UpdateState(EntityModel.EntityState.HURT);
            if (model.hp <= 0)
                UpdateState(EntityModel.EntityState.DIE);
            return true;
        }

        else if (_type == DAMAGETYPE.HEAL)
        {
            if (model.hp > model.hp_max)
                model.hp += _hp;
            return true;
        }

        return false;
    }

    protected virtual void Start()
    {
        model.SetEntityObject(entity_obj);

        UpdateState(EntityModel.EntityState.DEFAULT);
        UpdateJumpState(EntityModel.EntityJumpState.Grounded, model, ref jumpTime);
    }

    protected virtual void FixedUpdate()
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
                
                if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Dead Zone"))  //데드 존(추락사) 판정
                {
                    if (model.entity_obj.layer == LayerMask.NameToLayer("Player"))
                    {
                        GameSceneData.player_controller.SetHp(PlayerController.player_model.hp, PlayerController.player_model.hp, DAMAGETYPE.DAMAGE);
                    }
                    else
                    {
                        SetHp(model.hp_max, model.hp_max, DAMAGETYPE.DAMAGE);
                    }
                }
            }
            else if (model.foot_col_src.state == ColHitState.Stay)
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
            }
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

    protected virtual void UpdateState(EntityModel.EntityState _state)
    {
        model.state = _state;

        switch (model.state)
        {
            case EntityModel.EntityState.DEFAULT:
                model.entity_col.enabled = true;
                break;
            case EntityModel.EntityState.HURT:
                model.entity_ani.SetTrigger("_hurt");
                model.entity_col.enabled = false;
                break;
            case EntityModel.EntityState.DIE:
                model.entity_ani.SetBool("_dead", true);
                break;
        }
    }

    public void UpdateJumpState(EntityModel.EntityJumpState s, EntityModel m, ref float j)
    {
        if (m.movableStrategy != null)
            foreach (IEntityMovableStrategy item in m.movableStrategy.strategy)
                item.UpdateJumpState(s, m, ref j);
    }

    public virtual void Move(EntityModel m, int _layoutMask)
    {
        if (m.state != EntityModel.EntityState.HURT)
            if (m.state != EntityModel.EntityState.DIE)
                m.movableStrategy.Move(m, _layoutMask);

    }

    public void Jump(EntityModel m, ref float j)
    {
        if (m.movableStrategy != null)
        {
            if (m.state == EntityModel.EntityState.HURT || m.state == EntityModel.EntityState.DIE)
                if (m.jumpState == EntityModel.EntityJumpState.PrepareToJump)
                    return;
            m.movableStrategy.Jump(m, ref j);
        }
    }

    public void Down(EntityModel m)
    {
        if (m.movableStrategy != null)
        {
            if (m.state == EntityModel.EntityState.HURT || m.state == EntityModel.EntityState.DIE)
                    return;
            if (model.foot_col_src.type == COLTYPE.COLLISION)   //중복 하강 방지 코드
                if (model.foot_col_src.state == ColHitState.Stay)
                    if (model.jumpState == EntityModel.EntityJumpState.Grounded)
                        m.movableStrategy.Down(m);
        }
    }
}
