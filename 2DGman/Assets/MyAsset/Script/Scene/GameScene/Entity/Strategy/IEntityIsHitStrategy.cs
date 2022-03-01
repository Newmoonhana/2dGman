using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityIsHitStrategy
{
    void Hit(EntityModel model, Collider2D hit_col, Collider2D other_col, int _layerMask);
}

public class IsEnemyHit : IEntityIsHitStrategy
{
    public virtual void Hit(EntityModel model, Collider2D hit_col, Collider2D other_col, int _layerMask)
    {
        
    }
}

public class EntityIsHitStrategyList  //옵저버 패턴으로 움직임 관리
{
    public List<IEntityIsHitStrategy> strategy = new List<IEntityIsHitStrategy>();

    public virtual void Hit(EntityModel m, Collider2D hit_col, Collider2D other_col, int _layoutMask)
    {
        if (m.ishitStrategy != null)
            foreach (IEntityIsHitStrategy item in m.ishitStrategy.strategy)
                item.Hit(m, hit_col, other_col, _layoutMask);
    }
    
}
public class PlayerIsHit : EntityIsHitStrategyList
{
    public PlayerIsHit()
    {
        strategy.Add(new IsEnemyHit());
    }
}