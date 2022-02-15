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

    void Awake()
    {
        EnemyControllerFactory ef = new LandFactory();
        controller = ef.CreateEnemyCon("jump", gameObject);
        controller.Entity_obj = gameObject;

        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);
    }
}
