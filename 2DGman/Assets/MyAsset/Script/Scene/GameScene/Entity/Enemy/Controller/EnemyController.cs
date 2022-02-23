using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DAMAGETYPE
{
    DAMAGE,
    HEAL
}

public class EnemyController : EntityController
{
    public EnemyModel enemy_model;

    protected override void Start()
    {
        model.hp_max = model.hp = enemy_model.hp;
        StartCoroutine(Jumping());

        base.Start();
    }

    protected override void FixedUpdate()
    {
        if (model.state == EntityModel.EntityState.DIE)
            return;
        Move(model, (1 << LayerMask.NameToLayer("Land")) + (1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("EnemyWall")));

        base.FixedUpdate();
    }

    protected override void UpdateState(EntityModel.EntityState _state)
    {
        base.UpdateState(_state);
        
        switch (model.state)
        {
            case EntityModel.EntityState.HURT:
            case EntityModel.EntityState.DIE:
                AudioManager.Instance.Play("LandOnEnemy");
                break;
        }
    }
    public void IsHurtEnd()
    {
        UpdateState(EntityModel.EntityState.DEFAULT);
    }
    public void IsDeadEnd()
    {
        Entity_obj.SetActive(false);
    }

    //�÷��̾��� ���� ���� üŷ.
    void UpdateDirType()
    {
        model.movedir = EntityModel.MOVEDIRTYPE.CENTER;
        if (PlayerController.player_model.player_tns.position.x < transform.position.x)   //�÷��̾ �ڽź��� ���ʿ� ���� ��
        {
            model.movedir = EntityModel.MOVEDIRTYPE.LEFT;
        }
        else if (PlayerController.player_model.player_tns.position.x > transform.position.x)   //�÷��̾ �ڽź��� �����ʿ� ���� ��
        {
            model.movedir = EntityModel.MOVEDIRTYPE.RIGHT;
        }
    }

    public override void Move(EntityModel m, int _layoutMask)
    {
        UpdateDirType();
        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
            return;

        base.Move(m, _layoutMask);
    }

    protected IEnumerator Jumping()   //���� �ð� ���� ����.
    {
        while (!(model.state == EntityModel.EntityState.DIE))
        {
            yield return null;
            if (model.state != EntityModel.EntityState.HURT)
            {
                yield return GameManager.waitforseconds_3f;
                UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump, model, ref jumpTime);
            }
        }
    }
}

//public class Move : EnemyController
//{
//    public Move()
//    {
//        model.movableStrategy = new IsMove();
//    }
//}
//public class Jump : EnemyController
//{
//    public Jump()
//    {
//        model.movableStrategy = new IsJump();
//    }
//}
//public class MoveAndJump : EnemyController
//{
//    public MoveAndJump()
//    {
//        model.movableStrategy = new IsMoveAndJump();
//    }
//}