using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSlime : Slime
{
    public BlueSlime()
    {
        this_name = "Blue Slime";
        hp = 1;
    }

    protected override void Awake()
    {
        base.Awake();

        //EnemyControllerFactory ef = new LandFactory();
        //controller = ef.CreateEnemyCon("move&jump", gameObject);
        controller.model.movableStrategy = new IsMoveAndJump();
        controller.Entity_obj = gameObject;

        controller.enemy_model = this;
        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);
    }
}
