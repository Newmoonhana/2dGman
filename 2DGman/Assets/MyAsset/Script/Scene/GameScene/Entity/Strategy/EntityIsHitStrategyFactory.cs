using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityIsHitStrategyFactory
{
    public abstract EntityIsHitStrategyFactory CreateIsHitStrategy(string _type);   //팩토리 메서드
}

public class DefaultFactory : EntityIsHitStrategyFactory
{
    public override EntityIsHitStrategyFactory CreateIsHitStrategy(string _type)
    {
        EntityIsHitStrategyFactory strategy = null;

        if (_type.Equals(""))
        {
            
        }
        else
            Debug.LogWarning("동작 전략 패턴이 할당되지 않았습니다.");

        return strategy;
    }
}