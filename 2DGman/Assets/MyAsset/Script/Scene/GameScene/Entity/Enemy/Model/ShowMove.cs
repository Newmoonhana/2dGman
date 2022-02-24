using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMove : EnemyModel_Slime
{
    public ShowMove()
    {
        this_name = "Show Move";
        hp = 1;
    }

    protected override void Awake()
    {
        base.Awake();

        controller.Entity_obj = gameObject;
        controller.enemy_model = this;
        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);

        EnemyControllerFactory ef = new LandFactory();
        controller.model.movableStrategy = ef.CreateEnemyCon("move");
    }
}
