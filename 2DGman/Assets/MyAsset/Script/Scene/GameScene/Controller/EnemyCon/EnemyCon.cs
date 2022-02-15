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
    DIRTYPE dir = DIRTYPE.CENTER;   //�ڽ��� �������� �÷��̾��� ����
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

    //�÷��̾��� ���� ���� üŷ.
    void UpdateDirType()
    {
        dir = DIRTYPE.CENTER;
        if (player_tns.position.x < transform.position.x)   //�÷��̾ �ڽź��� ���ʿ� ���� ��
        {
            dir = DIRTYPE.LEFT;
        }
        else if (player_tns.position.x > transform.position.x)   //�÷��̾ �ڽź��� �����ʿ� ���� ��
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

    protected IEnumerator Jumping()   //���� �ð� ���� ����.
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