using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityIsHitStrategy
{
    bool IsHit(EntityModel model, IsColliderHit _col_src, COLTYPE hit_type, ColHitState hit_state, int _layerMask); //레이어로 체크
    bool IsHit(EntityModel model, IsColliderHit _col_src, COLTYPE hit_type, ColHitState hit_state, string _tagMask); //태그로 체크
    void Hit(EntityModel model, EntityController con, IsColliderHit _col_src);
}

public class IsHitStrategy : IEntityIsHitStrategy
{
    public bool IsHit(EntityModel model, IsColliderHit _col_src, COLTYPE hit_type, ColHitState hit_state, int _layerMask)
    {
        if (_col_src.state == hit_state)
        {
            if (_col_src.type == COLTYPE.COLLISION)
            {
                if (_col_src.other_col_COLLISION.gameObject.layer == _layerMask)
                    return true;
            }
            else if (_col_src.type == COLTYPE.TRIGGER)
            {
                if (_col_src.other_col_TRIGGER.gameObject.layer == _layerMask)
                    return true;
            }
        }
            
        return false;
    }
    public bool IsHit(EntityModel model, IsColliderHit _col_src, COLTYPE hit_type, ColHitState hit_state, string _tagMask)
    {
        if (_col_src.state == hit_state)
        {
            if (_col_src.type == COLTYPE.COLLISION)
            {
                if (string.Equals(_col_src.other_col_COLLISION.gameObject.tag, _tagMask))
                    return true;
            }
            else if (_col_src.type == COLTYPE.TRIGGER)
            {
                if (string.Equals(_col_src.other_col_TRIGGER.gameObject.tag, _tagMask))
                    return true;
            }
        }

        return false;
    }
    public virtual void Hit(EntityModel model, EntityController con, IsColliderHit _col_src) { }
}

public class IsEnemyHit : IsHitStrategy //플레이어가 Enemy에 충돌
{
    public override void Hit(EntityModel model, EntityController con, IsColliderHit _col_src)
    {
        if (!IsHit(model, _col_src, COLTYPE.COLLISION, ColHitState.Stay, LayerMask.NameToLayer("Enemy")))
            return;
        
        if (model.state == EntityModel.EntityState.DEFAULT)
            if (model.entity_col_src.other_col_COLLISION.gameObject.GetComponent<EnemyController>().model.state == EntityModel.EntityState.DEFAULT)
                con.SetHp(1, model.hp_max, DAMAGETYPE.DAMAGE);
    }
}
public class IsJumpPoleHit : IsHitStrategy //플레이어를 점프시키는 엔티티가 플레이어 발에 충돌
{
    public override void Hit(EntityModel model, EntityController con, IsColliderHit _col_src)
    {
        if (!IsHit(model, _col_src, COLTYPE.COLLISION, ColHitState.Stay, "jump pole"))
            return;
        
        if (model.state == EntityModel.EntityState.DEFAULT || model.state == EntityModel.EntityState.UNBEAT)
            if (model.jumpState == EntityModel.EntityJumpState.Grounded || model.jumpState == EntityModel.EntityJumpState.InFlight)
                if (model.state != EntityModel.EntityState.HURT)
                    if (model.state != EntityModel.EntityState.DIE)
                    {
                        EnemyController enemy = model.foot_col_src.other_col_COLLISION.gameObject.GetComponent<EnemyController>();

                        if (enemy.model.state == EntityModel.EntityState.DEFAULT)
                        {
                            enemy.model.rigid.velocity = Vector2.zero;
                            enemy.SetHp(1, enemy.model.hp_max, DAMAGETYPE.DAMAGE);

                            if (enemy.model.jumpState == EntityModel.EntityJumpState.Jumping)
                            {
                                float tmp = 0;
                                enemy.UpdateJumpState(EntityModel.EntityJumpState.InFlight, enemy.model, ref tmp);
                            }

                            GameSceneData.player_controller.UpdateJumpState(EntityModel.EntityJumpState.Grounded, model, ref GameSceneData.player_controller.jumpTime);
                            GameSceneData.player_controller.UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump, model, ref GameSceneData.player_controller.jumpTime);
                        }
                    }
    }
}
public class IsDeadZoneHit : IsHitStrategy  //엔티티가 낙사
{
    public override void Hit(EntityModel model, EntityController con, IsColliderHit _col_src)
    {
        if (!IsHit(model, _col_src, COLTYPE.TRIGGER, ColHitState.Enter, LayerMask.NameToLayer("Dead Zone")))
            return;

        if (model.state != EntityModel.EntityState.HURT || model.state != EntityModel.EntityState.DIE)
            con.SetHp(model.hp, model.hp_max, DAMAGETYPE.DAMAGE);
    }
}

public class EntityIsHitStrategyList : StrategyList<IEntityIsHitStrategy>  //옵저버 패턴으로 움직임 관리
{
    public virtual void Hit(EntityModel m, EntityController con, IsColliderHit _col_src)
    {
        if (IsNotNull())
            foreach (IEntityIsHitStrategy item in strategy)
                item.Hit(m, con, _col_src);
    }
    
}
public class PlayerIsHit : EntityIsHitStrategyList
{
    public PlayerIsHit()
    {
        strategy.Add(new IsJumpPoleHit());
        strategy.Add(new IsEnemyHit());
        strategy.Add(new IsDeadZoneHit());
    }
}

public class EnemyIsHit : EntityIsHitStrategyList
{
    public EnemyIsHit()
    {
        strategy.Add(new IsDeadZoneHit());
    }
}