using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCon : KineObject
{
    enum DIRTYPE
    {
        LEFT,
        CENTER,
        RIGHT
    }
    DIRTYPE dir = DIRTYPE.CENTER;   //자신을 기준으로 플레이어의 방향
    protected bool IsMove, IsJump;
    Transform player_tns;

    protected override void Start()
    {
        player_tns = GameSceneData.player_tns;
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
        if (state == EntityState.DIE)
            return;
        if (IsMove)
            Move((int)dir - 1);

        base.FixedUpdate();
    }

    protected override void UpdateState(EntityState _state)
    {
        base.UpdateState(_state);

        switch (state)
        {
            case EntityState.HURT:
                SFXManager.Instance.Play(SFXManager.Instance.GetAudioFile("LandOnEnemy"));
                break;
            case EntityState.DIE:

                break;
        }
    }

    //플레이어의 현재 방향 체킹.
    void UpdateDirType()
    {
        dir = DIRTYPE.CENTER;
        if (player_tns.position.x < transform.position.x)   //플레이어가 자신보다 왼쪽에 있을 때
        {
            dir = DIRTYPE.LEFT;
        }
        else if (player_tns.position.x > transform.position.x)   //플레이어가 자신보다 오른쪽에 있을 때
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
        while (!(state == EntityState.DIE))
        {
            yield return GameManager.waitforseconds_3f;
            UpdateJumpState(EntityJumpState.PrepareToJump);
        }
    }
}

public class Land : EnemyCon
{
    public Land()
    {
        IsMove = true;
        IsJump = false;
    }
}

public class Jump : EnemyCon
{
    public Jump()
    {
        IsMove = true;
        IsJump = true;
    }
}