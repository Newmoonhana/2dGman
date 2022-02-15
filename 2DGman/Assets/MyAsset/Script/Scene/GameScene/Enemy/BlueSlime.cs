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
        EnemyConFactory ef = new LandFactory();
        controller = ef.CreateEnemyCon("jump", gameObject);
        controller.kine_obj = gameObject;

        controller.speed = speed;
        controller.jumpPower = jumpPower;
        controller.jumpTimeLimit = jumpTimeLimit;
    }
}
