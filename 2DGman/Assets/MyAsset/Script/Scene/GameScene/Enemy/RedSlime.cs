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
        EnemyConFactory ef = new LandFactory();
        controller = ef.CreateEnemyCon("land", gameObject);
        controller.kine_obj = gameObject;

        controller.speed = speed;
        controller.jumpPower = jumpPower;
        controller.jumpTimeLimit = jumpTimeLimit;
    }
}
