using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityMovableStrategy
{
    void Move(float _horizontal, EntityModel model);
    void UpdateJumpState(EntityModel.EntityJumpState _state, EntityModel model, ref float jumpTime);
    void Jump(EntityModel model, ref float jumpTime);
}

public class IsMove : IEntityMovableStrategy
{
    public void UpdateJumpState(EntityModel.EntityJumpState s, EntityModel m, ref float j) { }
    public void Jump(EntityModel m, ref float j) { }
    public virtual void Move(float _horizontal, EntityModel model)
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

        Vector3 movement = Vector3.zero; //�̵� ��ǥ ���
        Vector2 xscale = model.entity_tns.localScale; //ũ��(�¿� ������ �ʿ�)
        float xscaleAbs = Mathf.Abs(model.entity_tns.localScale.x);
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
            xscale.x = -xscaleAbs;
            model.entity_tns.localScale = xscale;
            speed = -speed;
        }
        else if (model.movedir == EntityModel.MOVEDIRTYPE.RIGHT)
        {
            movement = Vector3.right;
            xscale.x = xscaleAbs;
            model.entity_tns.localScale = xscale;
        }
        movement.x = speed;

        model.entity_obj.transform.position += movement * GameSceneData.GetSpeed(model.speed) * Time.deltaTime;
        model.entity_ani.SetFloat("_velocityX", Mathf.Abs((int)model.movedir - 1));
    }
}

public class IsInputMove : IsMove
{
    public override void Move(float _horizontal, EntityModel model)
    {
        if (_horizontal != 0)
            if (model.jumpState == EntityModel.EntityJumpState.Grounded)
                AudioManager.Instance.Play("Walk01");
        base.Move(_horizontal, model);
    }
}

public class IsJump : IEntityMovableStrategy
{
    public void Move(float f, EntityModel m) { }

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
    public void Jump(EntityModel model, ref float jumpTime)
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
}

public class IsInputJump : IsJump
{
    public override void UpdateJumpState(EntityModel.EntityJumpState _state, EntityModel model, ref float jumpTime)
    {
        switch (_state)
        {
            case EntityModel.EntityJumpState.PrepareToJump:
                AudioManager.Instance.Play("jump");
                break;
            case EntityModel.EntityJumpState.Landed:
                AudioManager.Instance.Play("LandOnGround");
                break;
        }
        base.UpdateJumpState(_state, model, ref jumpTime);
    }
}