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
        Move(model, (1 << LayerMask.NameToLayer("Land")) + (1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("EnemyWallZone")));

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

    protected IEnumerator Jumping()   //일정 시간 마다 점프.
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