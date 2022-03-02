using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityIsHitStrategyFactory
{
    public abstract EntityIsHitStrategyList CreateIsHitStrategy(string _type);   //���丮 �޼���
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
            Debug.LogWarning("���� ���� ������ �Ҵ���� �ʾҽ��ϴ�.");

        return strategy;
    }
}