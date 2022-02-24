using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityMovableStrategy
{
    void Move(EntityModel model, int _layerMask);
    void UpdateJumpState(EntityModel.EntityJumpState _state, EntityModel model, ref float jumpTime);
    void Jump(EntityModel model, ref float jumpTime);
}

public class DefaultMoveable : IEntityMovableStrategy
{
    //tmp
    Vector2 vec2;
    RaycastHit2D[] hit;
    float f;
    int i;

    protected bool IsForwardHit(EntityModel model, int _layerMask)
    {
        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
            return false;

        f = model.entity_tns.localScale.x * 0.75f;  //����ĳ��Ʈ ��Ʈ �ִ� �Ÿ�
        vec2 = model.entity_tns.position;
        vec2.y += model.entity_tns.localScale.y / 2;
        if (model.movedir == EntityModel.MOVEDIRTYPE.LEFT)
            i = -1;
        else
            i = 1;
        Debug.DrawRay(vec2, Vector2.right * i * f, Color.red);
        hit = Physics2D.RaycastAll(vec2, Vector2.right * i, f, _layerMask);

        if (hit != null)
        {
            foreach (RaycastHit2D hitItem in hit)
                if (hitItem.collider != model.entity_col)
                {
                    if (model.movedir == EntityModel.MOVEDIRTYPE.LEFT)
                    {
                        if (hitItem.point.x < model.entity_tns.position.x)
                            return true;
                    }
                    else
                    {
                        if (hitItem.point.x > model.entity_tns.position.x)
                            return true;
                    }
                }
        }
        return false;
    }   //�ٶ󺸴� ���⿡ �浹�ϴ� ��ü Ȯ��

    public virtual void Move(EntityModel model, int _layerMask)
    {
        //if (jumpState == EntityJumpState.Grounded)    //���� �� ���� ��ȯ �ȵǴ� ���� ���ϸ� �� �ڵ� ����ϸ� ��(�� ������Ʈ���� �ʿ� ��� �ּ� ó��)
        //{
        //model.movedir = EntityModel.MOVEDIRTYPE.CENTER;
        //if (_horizontal < 0)
        //{
        //    model.movedir = EntityModel.MOVEDIRTYPE.LEFT;
        //}
        //else if (_horizontal > 0)
        //{
        //    model.movedir = EntityModel.MOVEDIRTYPE.RIGHT;
        //}
        //}
        if (IsForwardHit(model, _layerMask))
            return;

        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
        {
            model.entity_ani.SetFloat("_velocityX", 0);
            return;
        }

        Vector3 movement = Vector3.zero; //�̵� ��ǥ ���
        float speed = 1;
        //���� ���¿� ���� �ӵ��� ����(������ó��)�ε� �׳� ������ ���� ���ؼ� �ּ� ó��.
        //switch (model.jumpState)
        //{
        //    case EntityModel.EntityJumpState.Jumping:
        //        speed = 0.6f;
        //        break;
        //    case EntityModel.EntityJumpState.InFlight:
        //        speed = 0.4f;
        //        break;
        //}
        if (model.movedir == EntityModel.MOVEDIRTYPE.LEFT)
        {
            movement = Vector3.left;
            speed = -speed;
            model.entity_spr.flipX = true;
        }
        else if (model.movedir == EntityModel.MOVEDIRTYPE.RIGHT)
        {
            movement = Vector3.right;
            model.entity_spr.flipX = false;
        }
        movement.x = speed;

        model.entity_obj.transform.position += movement * GameSceneData.GetSpeed(model.speed) * Time.deltaTime;
        model.entity_ani.SetFloat("_velocityX", Mathf.Abs((int)model.movedir - 1));
    }

    public virtual void Jump(EntityModel model, ref float jumpTime)
    {
        switch (model.jumpState)
        {
            case EntityModel.EntityJumpState.PrepareToJump:
                if (!model.IsGrounded)
                    break;
                model.rigid.velocity = Vector3.zero;  //���� Tag�� ������ �ش� �ڵ� �ʿ�
                UpdateJumpState(EntityModel.EntityJumpState.Jumping, model, ref jumpTime);
                return;
            case EntityModel.EntityJumpState.Jumping:
                if (model.stopJump || jumpTime >= model.jumpTimeLimit)  //�϶� üũ
                    return; //������ Update���� ���߿� ������ �������� üũ�Ǿ� �־ InFlight�� �ٲ� �ʿ� ����(������ ���� �߿� �÷��� ������ �ٷ� �������� ���� ����)

                model.rigid.velocity = Vector3.zero;
                Vector2 addForce = Vector2.up;
                addForce.y *= model.jumpPower * (jumpTime * 0.1f + 1f);
                model.rigid.AddForce(addForce, ForceMode2D.Impulse);
                jumpTime += Time.fixedDeltaTime;
                model.entity_ani.SetFloat("_velocityY", Mathf.Abs(jumpTime));
                return;
        }
    }

    public virtual void UpdateJumpState(EntityModel.EntityJumpState _state, EntityModel model, ref float jumpTime)
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
                model.entity_ani.SetFloat("_velocityY", -1);
                model.footcol.enabled = true;
                break;
            case EntityModel.EntityJumpState.Landed:
                UpdateJumpState(EntityModel.EntityJumpState.Grounded, model, ref jumpTime);
                break;
        }
    }
}

public class IsMove : DefaultMoveable
{
    public override void Move(EntityModel model, int _layerMask)
    {
        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
            return;

        if (base.IsForwardHit(model, _layerMask))   //��ü�� �浹 �� ���� ��ȯ
            model.movedir = model.movedir == EntityModel.MOVEDIRTYPE.LEFT ? EntityModel.MOVEDIRTYPE.RIGHT : EntityModel.MOVEDIRTYPE.LEFT;

        base.Move(model, _layerMask);
    }
    public override void UpdateJumpState(EntityModel.EntityJumpState s, EntityModel m, ref float j) { }
    public override void Jump(EntityModel m, ref float j) { }
}

public class IsFollowPlayer : DefaultMoveable
{
    //�÷��̾��� ���� ���� üŷ.
    void UpdateDirType(EntityModel model)
    {
        model.movedir = EntityModel.MOVEDIRTYPE.CENTER;
        if (PlayerController.player_model.player_tns.position.x < model.entity_tns.transform.position.x)   //�÷��̾ �ڽź��� ���ʿ� ���� ��
        {
            model.movedir = EntityModel.MOVEDIRTYPE.LEFT;
        }
        else if (PlayerController.player_model.player_tns.position.x > model.entity_tns.transform.position.x)   //�÷��̾ �ڽź��� �����ʿ� ���� ��
        {
            model.movedir = EntityModel.MOVEDIRTYPE.RIGHT;
        }
    }

    public override void Move(EntityModel model, int _layerMask)
    {
        UpdateDirType(model);
        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
            return;

        base.Move(model, _layerMask);
    }
    public override void UpdateJumpState(EntityModel.EntityJumpState s, EntityModel m, ref float j) { }
    public override void Jump(EntityModel m, ref float j) { }
}

public class IsJump : DefaultMoveable
{
    public override void Move(EntityModel m, int l) { }
}

public class IsInputPlayer : DefaultMoveable
{
    public override void Move(EntityModel model, int _layerMask)
    {
        if (model.movedir != EntityModel.MOVEDIRTYPE.CENTER)
            if (model.jumpState == EntityModel.EntityJumpState.Grounded)
                AudioManager.Instance.Play("Walk01");
        base.Move(model, _layerMask);
    }

    public override void UpdateJumpState(EntityModel.EntityJumpState _state, EntityModel model, ref float jumpTime)
    {
        switch (_state)
        {
            case EntityModel.EntityJumpState.PrepareToJump:
                AudioManager.Instance.Play("Jump");
                break;
            case EntityModel.EntityJumpState.Landed:
                AudioManager.Instance.Play("LandOnGround");
                break;
        }
        base.UpdateJumpState(_state, model, ref jumpTime);
    }
}

public class EntityMovableStrategyList  //������ �������� ������ ����
{
    public List<IEntityMovableStrategy> strategy = new List<IEntityMovableStrategy>();

    public virtual void Move(EntityModel m, int _layoutMask)
    {
        if (m.movableStrategy != null)
            foreach (IEntityMovableStrategy item in m.movableStrategy.strategy)
                item.Move(m, _layoutMask);
    }
    public void Jump(EntityModel m, ref float j)
    {
        if (m.movableStrategy != null)
            foreach (IEntityMovableStrategy item in m.movableStrategy.strategy)
                item.Jump(m, ref j);
    }
}
public class Move : EntityMovableStrategyList
{
    public Move()
    {
        strategy.Add(new IsMove());
    }
}
public class FollowPlayer : EntityMovableStrategyList
{
    public FollowPlayer()
    {
        strategy.Add(new IsFollowPlayer());
    }
}
public class Jump : EntityMovableStrategyList
{
    public Jump()
    {
        strategy.Add(new IsJump());
    }
}
public class MoveAndJump : EntityMovableStrategyList
{
    public MoveAndJump()
    {
        strategy.Add(new IsMove());
        strategy.Add(new IsJump());
    }
}
public class InputPlayer : EntityMovableStrategyList
{
    public InputPlayer()
    {
        strategy.Add(new IsInputPlayer());
    }
}