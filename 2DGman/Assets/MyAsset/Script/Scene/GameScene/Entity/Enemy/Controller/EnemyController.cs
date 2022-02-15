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
    DIRTYPE dir = DIRTYPE.CENTER;   //�ڽ��� �������� �÷��̾��� ����
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

    public override void Move(float _horizontal)
    {
        if (dir == DIRTYPE.CENTER)
            return;

        base.Move(_horizontal);
    }

    protected IEnumerator Jumping()   //���� �ð� ���� ����.
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