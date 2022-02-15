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
        footcol = entity_tns.GetChild(1).GetComponent<Collider2D>();
        foot_col_src = footcol.GetComponent<IsColliderHit>();
        entity_ani = entity_tns.GetChild(0).GetComponent<Animator>();
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
                    //�� �浹 ����
                    if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Land") || model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Platform"))
                    {  
                        if (model.jumpState == EntityModel.EntityJumpState.InFlight)
                        {
                            model.rigid.velocity = Vector3.zero;
                            UpdateJumpState(EntityModel.EntityJumpState.Landed);
                        }
                    }
                    else if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Dead Zone"))  //���� ��(�߶���) ����
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
                    //�� �浹 ����(�߶�)
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
        //if (jumpState == EntityJumpState.Grounded)    //���� �� ���� ��ȯ �ȵǴ� ���� ���ϸ� �� �ڵ� ����ϸ� ��(�� ������Ʈ���� �ʿ� ��� �ּ� ó��)
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

        vec3 = Vector3.zero; //�̵� ��ǥ ���
        vec2 = model.entity_tns.localScale; //ũ��(�¿� ������ �ʿ�)
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
                model.rigid.velocity = Vector3.zero;  //���� Tag�� ������ �ش� �ڵ� �ʿ�
                UpdateJumpState(EntityModel.EntityJumpState.Jumping);
                return;
            case EntityModel.EntityJumpState.Jumping:
                if (model.stopJump || jumpTime >= model.jumpTimeLimit)  //�϶� üũ
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
