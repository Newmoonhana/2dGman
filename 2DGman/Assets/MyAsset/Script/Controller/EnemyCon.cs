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
    public Transform player_tns;

    protected override void Awake()
    {
        StartCoroutine(Jumping());

        base.Awake();
    }

    protected override void Update()
    {
        UpdateDirType();

        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (state == EntityState.DIE)
            return;
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

    IEnumerator Jumping()   //일정 시간 마다 점프.
    {
        while (!(state == EntityState.DIE))
        {
            UpdateJumpState(EntityJumpState.PrepareToJump);
            yield return GameManager.waitforseconds_3f;
        }
    }
}
