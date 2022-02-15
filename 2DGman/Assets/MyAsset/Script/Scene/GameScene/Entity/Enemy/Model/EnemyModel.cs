using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyModel : MonoBehaviour
{
    public enum ENEMYTYPE
    {
        SLIME,
        HUMAN
    }

    protected ENEMYTYPE type;
    [SerializeField] protected string this_name;
    [SerializeField] protected int hp;
    protected EnemyController controller;
    [SerializeField] protected SPEEDTYPE speed;
    [SerializeField] protected float jumpPower, jumpTimeLimit;
}

public abstract class Slime : EnemyModel
{
    public Slime()
    {
        type = ENEMYTYPE.SLIME;
    }
}