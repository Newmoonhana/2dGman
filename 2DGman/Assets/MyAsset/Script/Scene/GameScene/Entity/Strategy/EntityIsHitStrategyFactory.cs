using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityIsHitStrategyFactory
{
    public abstract EntityIsHitStrategyFactory CreateIsHitStrategy(string _type);   //���丮 �޼���
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
            Debug.LogWarning("���� ���� ������ �Ҵ���� �ʾҽ��ϴ�.");

        return strategy;
    }
}