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

    enum DIRTYPE
    {
        LEFT,
        CENTER,
        RIGHT
    }
    DIRTYPE dirOfPlayer = DIRTYPE.CENTER;   //자신을 기준으로 플레이어의 방향

    public bool SetHp(float _hp, DAMAGETYPE _type) //hp가 0 이하일 경우 false
    {
        if (_type == DAMAGETYPE.DAMAGE)
            enemy_model.hp -= _hp;
        else if (_type == DAMAGETYPE.HEAL)
            enemy_model.hp += _hp;
        if (enemy_model.hp > 0)
        {
            UpdateState(EntityModel.EntityState.HURT);
            return true;
        }

        UpdateState(EntityModel.EntityState.DIE);
        return false;
    }

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
        Move((int)dirOfPlayer - 1, model);

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
    public void IsHurt()
    {
        UpdateState(EntityModel.EntityState.DEFAULT);
    }
    public void IsDead()
    {
        Entity_obj.SetActive(false);
    }

    //플레이어의 현재 방향 체킹.
    void UpdateDirType()
    {
        dirOfPlayer = DIRTYPE.CENTER;
        if (PlayerController.player_model.player_tns.position.x < transform.position.x)   //플레이어가 자신보다 왼쪽에 있을 때
        {
            dirOfPlayer = DIRTYPE.LEFT;
        }
        else if (PlayerController.player_model.player_tns.position.x > transform.position.x)   //플레이어가 자신보다 오른쪽에 있을 때
        {
            dirOfPlayer = DIRTYPE.RIGHT;
        }
    }

    public void Move()
    {
        if (dirOfPlayer == DIRTYPE.CENTER)
            return;
    }

    protected IEnumerator Jumping()   //일정 시간 마다 점프.
    {
        while (!(model.state == EntityModel.EntityState.DIE))
        {
            yield return GameManager.waitforseconds_3f;
            UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump, model, ref jumpTime);
        }
    }
}

public class Move : EnemyController
{
    public Move()
    {
        model.movableStrategy = new IsMove();
    }
}
public class Jump : EnemyController
{
    public Jump()
    {
        model.movableStrategy = new IsJump();
    }
}
public class MoveAndJump : EnemyController
{
    public MoveAndJump()
    {
        model.movableStrategy = new IsMoveAndJump();
    }
}