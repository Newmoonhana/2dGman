using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� �� �� ���� ������ ���صǼ� ���
public abstract class EnemyControllerFactory
{
    public abstract EntityMovableStrategyList CreateEnemyCon(string _type);   //���丮 �޼���
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
        else if (_type.Equals("down&jump"))
        {
            enemy = new DownAndJump();
        }
        else
            Debug.LogWarning("���� ���� ������ �Ҵ���� �ʾҽ��ϴ�.");

        return enemy;
    }
}