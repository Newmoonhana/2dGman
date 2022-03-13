using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big : EnemyModel_Human
{
    public Big()
    {
        this_name = "Big";
        hp = 3;
    }

    protected override void Awake()
    {
        base.Awake();

        controller.Entity_obj = gameObject;
        controller.enemy_model = this;
        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);

        EntityMovableStrategyFactory ef = new LandFactory();
        controller.model.movableStrategy = ef.CreateMovableStrategy("move");
    }
}
