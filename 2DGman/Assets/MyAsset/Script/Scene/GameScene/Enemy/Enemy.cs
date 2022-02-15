using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMYTYPE
{
    SLIME,
    HUMAN
}

public abstract class Enemy : MonoBehaviour
{
    protected ENEMYTYPE type;
    [SerializeField] protected string this_name;
    [SerializeField] protected int hp;
    protected EnemyCon controller;
    [SerializeField] protected SPEEDTYPE speed;
    [SerializeField] protected float jumpPower, jumpTimeLimit;
}

public abstract class Slime : Enemy
{
    public Slime()
    {
        type = ENEMYTYPE.SLIME;
    }
}