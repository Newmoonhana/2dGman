using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyModel : MonoBehaviour
{
    public enum ENEMYTYPE
    {
        NONE,
        SLIME,
        HUMAN
    }

    protected ENEMYTYPE type;
    [SerializeField] protected string this_name;
    [SerializeField] public float hp;
    protected EnemyController controller;
    [SerializeField] protected SPEEDTYPE speed;
    [SerializeField] protected float jumpPower, jumpTimeLimit;
    [SerializeField] public float jumpOnTime, jumpLoopTime;
    [SerializeField] public float downOnTime, downLoopTime;
}

public abstract class EnemyModel_None : EnemyModel
{
    public EnemyModel_None()
    {
        type = ENEMYTYPE.NONE;
    }

    protected virtual void Awake()
    {
        controller = GetComponent<EnemyController>();
        EntityIsHitStrategyFactory ef_ih = new IsHitFactory();
        controller.model.ishitStrategy = ef_ih.CreateIsHitStrategy("enemy");
    }
}

public abstract class EnemyModel_Slime : EnemyModel
{
    public EnemyModel_Slime()
    {
        type = ENEMYTYPE.SLIME;
    }

    protected virtual void Awake()
    {
        controller = GetComponent<EnemyController>();
        EntityIsHitStrategyFactory ef_ih = new IsHitFactory();
        controller.model.ishitStrategy = ef_ih.CreateIsHitStrategy("enemy");
    }
}

public abstract class EnemyModel_Human : EnemyModel
{
    public EnemyModel_Human()
    {
        type = ENEMYTYPE.HUMAN;
    }

    protected virtual void Awake()
    {
        controller = GetComponent<EnemyController>();
        EntityIsHitStrategyFactory ef_ih = new IsHitFactory();
        controller.model.ishitStrategy = ef_ih.CreateIsHitStrategy("enemy");
    }
}