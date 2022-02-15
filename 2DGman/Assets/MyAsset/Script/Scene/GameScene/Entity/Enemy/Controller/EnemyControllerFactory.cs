using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyControllerFactory
{
    public abstract EnemyController CreateEnemyCon(string _type, GameObject _obj);   //팩토리 메서드
}

public class LandFactory : EnemyControllerFactory
{
    public override EnemyController CreateEnemyCon(string _type, GameObject _obj)
    {
        EnemyController enemy = null;

        if (_type.Equals("land"))
        {
            enemy = _obj.AddComponent<Land>();
        }
        else if (_type.Equals("jump"))
        {
            enemy = _obj.AddComponent<Jump>();
        }

        return enemy;
    }
}