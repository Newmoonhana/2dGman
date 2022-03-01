using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityMovableStrategyFactory
{
    public abstract EntityMovableStrategyList CreateMovableStrategy(string _type);   //팩토리 메서드
}

public class LandFactory : EntityMovableStrategyFactory
{
    public override EntityMovableStrategyList CreateMovableStrategy(string _type)
    {
        EntityMovableStrategyList strategy = null;

        if (_type.Equals("move"))
        {
            strategy = new Move();
        }
        else if (_type.Equals("follow_player"))
        {
            strategy = new FollowPlayer();
        }
        else if (_type.Equals("jump"))
        {
            strategy = new Jump();
        }
        else if (_type.Equals("move&jump"))
        {
            strategy = new MoveAndJump();
        }
        else if (_type.Equals("down&jump"))
        {
            strategy = new DownAndJump();
        }
        else
            Debug.LogWarning("동작 전략 패턴이 할당되지 않았습니다.");

        return strategy;
    }
}