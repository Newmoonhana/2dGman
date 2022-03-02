using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityIsHitStrategyFactory
{
    public abstract EntityIsHitStrategyList CreateIsHitStrategy(string _type);   //팩토리 메서드
}

public class IsHitFactory : EntityIsHitStrategyFactory
{
    public override EntityIsHitStrategyList CreateIsHitStrategy(string _type)
    {
        EntityIsHitStrategyList strategy = null;

        if (_type.Equals("player"))
        {
            strategy = new PlayerIsHit();
        }
        else if (_type.Equals("enemy"))
        {
            strategy = new EnemyIsHit();
        }
        else
            Debug.LogWarning("동작 전략 패턴이 할당되지 않았습니다.");

        return strategy;
    }
}