using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    enum DIRTYPE
    {
        LEFT,
        CENTER,
        RIGHT
    }
    DIRTYPE dir = DIRTYPE.CENTER;   //자신을 기준으로 플레이어의 방향
    protected bool IsMove, IsJump;

    protected override void Start()
    {
        if (IsJump)
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
        if (IsMove)
            Move((int)dir - 1);

        base.FixedUpdate();
    }

    protected override void UpdateState(EntityModel.EntityState _state)
    {
        base.UpdateState(_state);

        switch (model.state)
        {
            case EntityModel.EntityState.HURT:
                AudioManager.Instance.Play(AudioManager.Instance.GetAudioFile("LandOnEnemy"));
                break;
            case EntityModel.EntityState.DIE:

                break;
        }
    }

    //플레이어의 현재 방향 체킹.
    void UpdateDirType()
    {
        dir = DIRTYPE.CENTER;
        if (PlayerController.player_model.player_tns.position.x < transform.position.x)   //플레이어가 자신보다 왼쪽에 있을 때
        {
            dir = DIRTYPE.LEFT;
        }
        else if (PlayerController.player_model.player_tns.position.x > transform.position.x)   //플레이어가 자신보다 오른쪽에 있을 때
        {
            dir = DIRTYPE.RIGHT;
        }
    }

    public override void Move(float _horizontal)
    {
        if (dir == DIRTYPE.CENTER)
            return;

        base.Move(_horizontal);
    }

    protected IEnumerator Jumping()   //일정 시간 마다 점프.
    {
        while (!(model.state == EntityModel.EntityState.DIE))
        {
            yield return GameManager.waitforseconds_3f;
            UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump);
        }
    }
}

public class Land : EnemyController
{
    public Land()
    {
        IsMove = true;
        IsJump = false;
    }
}

public class Jump : EnemyController
{
    public Jump()
    {
        IsMove = true;
        IsJump = true;
    }
}