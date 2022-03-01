using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMove : EnemyModel_None
{
    public string moveable_name;

    public ShowMove()
    {
        this_name = "Show " + moveable_name;
        hp = 1;
    }

    protected override void Awake()
    {
        base.Awake();

        controller.Entity_obj = gameObject;
        controller.enemy_model = this;
        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);

        EntityMovableStrategyFactory ef = new LandFactory();
        controller.model.movableStrategy = ef.CreateMovableStrategy(moveable_name);
    }
}
