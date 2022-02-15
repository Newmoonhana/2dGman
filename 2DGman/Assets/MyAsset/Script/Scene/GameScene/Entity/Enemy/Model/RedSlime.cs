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

    void Awake()
    {
        EnemyControllerFactory ef = new LandFactory();
        controller = ef.CreateEnemyCon("land", gameObject);
        controller.Entity_obj = gameObject;

        controller.model.SetSubValue(speed, jumpPower, jumpTimeLimit);
    }
}
