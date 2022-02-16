using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 만들고보니 쓸 일 없고 오히려 방해되서 폐기
//public abstract class EnemyControllerFactory
//{
//    public abstract EnemyController CreateEnemyCon(string _type, GameObject _obj);   //팩토리 메서드
//}

//public class LandFactory : EnemyControllerFactory
//{
//    public override EnemyController CreateEnemyCon(string _type, GameObject _obj)
//    {
//        EnemyController enemy = null;

//        if (_type.Equals("move"))
//        {
//            enemy = _obj.AddComponent<Move>();
//        }
//        else if (_type.Equals("jump"))
//        {
//            enemy = _obj.AddComponent<Jump>();
//        }
//        else if (_type.Equals("move&jump"))
//        {
//            enemy = _obj.AddComponent<MoveAndJump>();
//        }
//        else
//            Debug.LogWarning("동작 전략 패턴이 할당되지 않았습니다.");

//        return enemy;
//    }
//}