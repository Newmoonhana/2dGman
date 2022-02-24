using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 만들고보니 쓸 일 없고 오히려 방해되서 폐기
public abstract class EnemyControllerFactory
{
    public abstract EntityMovableStrategyList CreateEnemyCon(string _type);   //팩토리 메서드
}

public class LandFactory : EnemyControllerFactory
{
    public override EntityMovableStrategyList CreateEnemyCon(string _type)
    {
        EntityMovableStrategyList enemy = null;

        if (_type.Equals("move"))
        {
            enemy = new Move();
        }
        else if (_type.Equals("follow_player"))
        {
            enemy = new FollowPlayer();
        }
        else if (_type.Equals("jump"))
        {
            enemy = new Jump();
        }
        else if (_type.Equals("move&jump"))
        {
            enemy = new MoveAndJump();
        }
        else
            Debug.LogWarning("동작 전략 패턴이 할당되지 않았습니다.");

        return enemy;
    }
}