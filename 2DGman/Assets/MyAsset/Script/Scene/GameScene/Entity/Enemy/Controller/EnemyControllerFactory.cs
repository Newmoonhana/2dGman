using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� �� �� ���� ������ ���صǼ� ���
//public abstract class EnemyControllerFactory
//{
//    public abstract EnemyController CreateEnemyCon(string _type, GameObject _obj);   //���丮 �޼���
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
//            Debug.LogWarning("���� ���� ������ �Ҵ���� �ʾҽ��ϴ�.");

//        return enemy;
//    }
//}