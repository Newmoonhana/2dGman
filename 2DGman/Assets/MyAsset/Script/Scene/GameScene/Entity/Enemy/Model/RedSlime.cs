using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSlime : Slime
{
    public RedSlime()
    {
        this_name = "Red Slime";
        hp = 1;
    }

    protected override void Awake()
    {
        base.Awake();

        //EnemyControllerFactory ef = new LandFactory();
        //controller = ef.CreateEnemyCon("move", gameObject);
        controller.model.movableStrategy = new IsMove();
        controller.Entity_obj = gameObject;

        controller.enemy_model = this;
        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);
    }
}
