using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Women : EnemyModel_Human
{
    public Women()
    {
        this_name = "Women";
        hp = 2;
    }

    protected override void Awake()
    {
        base.Awake();

        controller.Entity_obj = gameObject;
        controller.enemy_model = this;
        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);

        EntityMovableStrategyFactory ef = new LandFactory();
        controller.model.movableStrategy = ef.CreateMovableStrategy("follow_player");
    }
}
