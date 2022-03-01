using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSlime : EnemyModel_Slime
{
    public RedSlime()
    {
        this_name = "Red Slime";
        hp = 1;
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
