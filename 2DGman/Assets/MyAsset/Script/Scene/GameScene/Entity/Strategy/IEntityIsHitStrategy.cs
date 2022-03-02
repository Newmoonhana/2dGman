using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityIsHitStrategy
{
    bool IsHit(EntityModel model, Collider2D hit_col, IsColliderHit _col_src, COLTYPE hit_type, ColHitState hit_state, int _layoutMask);
    void Hit(EntityModel model, Collider2D hit_col, IsColliderHit _col_src);
}

public class IsHitStrategy : IEntityIsHitStrategy
{
    public bool IsHit(EntityModel model, Collider2D hit_col, IsColliderHit _col_src, COLTYPE hit_type, ColHitState hit_state, int _layoutMask)
    {
        if (_col_src.state == hit_state)
        {
            if (_col_src.type == COLTYPE.COLLISION)
            {
                if (_col_src.other_col_COLLISION.gameObject.layer == _layoutMask)
                    return true;
            }
            else if (_col_src.type == COLTYPE.TRIGGER)
            {
                if (_col_src.other_col_TRIGGER.gameObject.layer == _layoutMask)
                    return true;
            }
        }
            
        return false;
    }
    public virtual void Hit(EntityModel model, Collider2D hit_col, IsColliderHit _col_src) { }
}

public class Player_IsDamageHit : IsHitStrategy
{
    public override void Hit(EntityModel model, Collider2D hit_col, IsColliderHit _col_src)
    {
        if (!IsHit(model, hit_col, _col_src, COLTYPE.COLLISION, ColHitState.Stay, LayerMask.NameToLayer("Enemy")))
            return;
        
        //몬스터 충돌 판정
        if (model.state == EntityModel.EntityState.DEFAULT)
            if (model.entity_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                if (model.entity_col_src.other_col_COLLISION.gameObject.GetComponent<EnemyController>().model.state == EntityModel.EntityState.DEFAULT)
                    GameSceneData.player_controller.SetHp(1, PlayerController.player_model.hp, DAMAGETYPE.DAMAGE);
    }
}

public class EntityIsHitStrategyList  //옵저버 패턴으로 움직임 관리
{
    public List<IEntityIsHitStrategy> strategy = new List<IEntityIsHitStrategy>();

    public virtual void Hit(EntityModel m, Collider2D hit_col, IsColliderHit _col_src)
    {
        if (m.ishitStrategy != null)
            foreach (IEntityIsHitStrategy item in m.ishitStrategy.strategy)
                item.Hit(m, hit_col, _col_src);
    }
    
}
public class PlayerIsHit : EntityIsHitStrategyList
{
    public PlayerIsHit()
    {
        strategy.Add(new Player_IsDamageHit());
    }
}