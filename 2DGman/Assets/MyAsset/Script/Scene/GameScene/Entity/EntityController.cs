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

    public EntityState state;
    public MOVEDIRTYPE movedir = MOVEDIRTYPE.CENTER;
    public EntityJumpState jumpState;

    //entity ����
    public GameObject entity_obj;
    public Transform entity_tns;
    public Collider2D footcol;
    public IsColliderHit foot_col_src; // ���� ��� �ݶ��̴� �浹 ���� ��ũ��Ʈ
    public Animator entity_ani;
    public Rigidbody2D rigid;

    //�ΰ����� ���� ����
    public SPEEDTYPE speed;
    public float jumpPower, jumpTimeLimit;

    public bool stopJump;
    public bool IsGrounded = true;

    [SerializeField] public IEntityMovableStrategy movableStrategy;

    /// <summary>
    /// entity_obj�� �������� �ٸ� �����鵵 �������ִ� �Լ�
    /// </summary>
    /// <param name="_obj">����� �Ǵ� ���� ������Ʈ(=entity_obj)</param>
    public void SetEntityObject(GameObject _obj)
    {
        if (_obj == null)
        {
            Debug.LogWarning("Entity Model�� _obj�� ���Ե��� ���߽��ϴ�.");
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
    /// �ΰ����� ���� ���� ����
    /// </summary>
    /// <param name="_type">���ǵ� ��(SPEEDTYPE)</param>
    /// <param name="_jumpPower">���� ����</param>
    /// <param name="_jumpTimeLimit">���� �ð� �ִ�ġ</param>
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

            // ���� �� �߶� üũ
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
                    //�� �浹 ����
                    if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Land") || model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Platform"))
                    {  
                        if (model.jumpState == EntityModel.EntityJumpState.InFlight)
                        {
                            model.rigid.velocity = Vector3.zero;
                            UpdateJumpState(EntityModel.EntityJumpState.Landed, model, ref jumpTime);
                        }
                    }
                    else if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Dead Zone"))  //���� ��(�߶���) ����
                    {
                        UpdateState(EntityModel.EntityState.DIE);
                    }
                }
                //else if (model.foot_col_src.state == ColHitState.Stay)
                //{

                //}
                else if (model.foot_col_src.state == ColHitState.Exit)
                {
                    //�� �浹 ����(�߶�)
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
