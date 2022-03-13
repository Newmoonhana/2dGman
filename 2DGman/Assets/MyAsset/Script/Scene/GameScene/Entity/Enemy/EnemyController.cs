using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DAMAGETYPE
{
    EQUAL,
    DAMAGE,
    HEAL
}

public class EnemyController : EntityController
{
    public EnemyModel enemy_model;

    protected override void Start()
    {
        model.hp_max = model.hp = enemy_model.hp;
        InvokeRepeating("Jumping", enemy_model.jumpOnTime, enemy_model.jumpLoopTime);
        InvokeRepeating("Downing", enemy_model.downOnTime, enemy_model.downLoopTime);


        base.Start();
    }

    protected override void FixedUpdate()
    {
        if (model.state == EntityModel.EntityState.DIE)
            return;
        Move(model, (1 << LayerMask.NameToLayer("Land")) + (1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("EnemyWallZone")));
        Hit(model, this, model.entity_col_src);

        base.FixedUpdate();
    }

    public override void UpdateState(EntityModel.EntityState _state)
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

    protected void Jumping()   //일정 시간 마다 점프.
    {
        if (model.state == EntityModel.EntityState.DIE)
            return;
        if (model.state == EntityModel.EntityState.HURT)
            return;
        if (model.jumpState == EntityModel.EntityJumpState.Grounded)
            UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump, model, ref jumpTime);
    }

    protected void Downing()
    {
        base.Down(model);
    }
}