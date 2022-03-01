using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSlime : EnemyModel_Slime
{
    public BlueSlime()
    {
        this_name = "Blue Slime";
        hp = 1;
    }

    protected override void Awake()
    {
        base.Awake();

        controller.Entity_obj = gameObject;
        controller.enemy_model = this;
        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);

        EntityMovableStrategyFactory ef = new LandFactory();
        controller.model.movableStrategy = ef.CreateMovableStrategy("move&jump");
    }
}
