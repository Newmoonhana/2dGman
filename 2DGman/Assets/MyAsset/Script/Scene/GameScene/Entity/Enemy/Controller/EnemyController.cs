using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : EntityController
{
    enum DIRTYPE
    {
        LEFT,
        CENTER,
        RIGHT
    }
    DIRTYPE dir = DIRTYPE.CENTER;   //�ڽ��� �������� �÷��̾��� ����

    protected override void Start()
    {
        StartCoroutine(Jumping());

        base.Start();
    }

    void Update()
    {
        UpdateDirType();
    }

    protected override void FixedUpdate()
    {
        if (model.state == EntityModel.EntityState.DIE)
            return;
        Move((int)dir - 1, model);

        base.FixedUpdate();
    }

    protected override void UpdateState(EntityModel.EntityState _state)
    {
        base.UpdateState(_state);

        switch (model.state)
        {
            case EntityModel.EntityState.HURT:
                AudioManager.Instance.Play("LandOnEnemy");
                break;
            //case EntityModel.EntityState.DIE:

            //    break;
        }
    }

    //�÷��̾��� ���� ���� üŷ.
    void UpdateDirType()
    {
        dir = DIRTYPE.CENTER;
        if (PlayerController.player_model.player_tns.position.x < transform.position.x)   //�÷��̾ �ڽź��� ���ʿ� ���� ��
        {
            dir = DIRTYPE.LEFT;
        }
        else if (PlayerController.player_model.player_tns.position.x > transform.position.x)   //�÷��̾ �ڽź��� �����ʿ� ���� ��
        {
            dir = DIRTYPE.RIGHT;
        }
    }

    public void Move()
    {
        if (dir == DIRTYPE.CENTER)
            return;
    }

    protected IEnumerator Jumping()   //���� �ð� ���� ����.
    {
        while (!(model.state == EntityModel.EntityState.DIE))
        {
            yield return GameManager.waitforseconds_3f;
            UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump, model, ref jumpTime);
        }
    }
}

public class Land : EnemyController
{
    public Land()
    {
        model.movableStrategy.Add(new IsMove());
    }
}

public class Jump : EnemyController
{
    public Jump()
    {
        model.movableStrategy.Add(new IsMove());
        model.movableStrategy.Add(new IsJump());
    }
}