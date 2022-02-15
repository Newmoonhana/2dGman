using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyConFactory
{
    public abstract EnemyCon CreateEnemyCon(string _type, GameObject _obj);   //팩토리 메서드
}

public class LandFactory : EnemyConFactory
{
    public override EnemyCon CreateEnemyCon(string _type, GameObject _obj)
    {
        EnemyCon enemy = null;

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