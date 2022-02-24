using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityMovableStrategy
{
    void Move(EntityModel model, int _layerMask);
    void UpdateJumpState(EntityModel.EntityJumpState _state, EntityModel model, ref float jumpTime);
    void Jump(EntityModel model, ref float jumpTime);
}

public class DefaultMoveable : IEntityMovableStrategy
{
    //tmp
    Vector2 vec2;
    RaycastHit2D[] hit;
    float f;
    int i;

    protected bool IsForwardHit(EntityModel model, int _layerMask)
    {
        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
            return false;

        f = model.entity_tns.localScale.x * 0.75f;  //레이캐스트 히트 최대 거리
        vec2 = model.entity_tns.position;
        vec2.y += model.entity_tns.localScale.y / 2;
        if (model.movedir == EntityModel.MOVEDIRTYPE.LEFT)
            i = -1;
        else
            i = 1;
        Debug.DrawRay(vec2, Vector2.right * i * f, Color.red);
        hit = Physics2D.RaycastAll(vec2, Vector2.right * i, f, _layerMask);

        if (hit != null)
        {
            foreach (RaycastHit2D hitItem in hit)
                if (hitItem.collider != model.entity_col)
                {
                    if (model.movedir == EntityModel.MOVEDIRTYPE.LEFT)
                    {
                        if (hitItem.point.x < model.entity_tns.position.x)
                            return true;
                    }
                    else
                    {
                        if (hitItem.point.x > model.entity_tns.position.x)
                            return true;
                    }
                }
        }
        return false;
    }   //바라보는 방향에 충돌하는 물체 확인

    public virtual void Move(EntityModel model, int _layerMask)
    {
        //if (jumpState == EntityJumpState.Grounded)    //점프 후 방향 전환 안되는 점프 원하면 이 코드 사용하면 됨(이 프로젝트에선 필요 없어서 주석 처리)
        //{
        //model.movedir = EntityModel.MOVEDIRTYPE.CENTER;
        //if (_horizontal < 0)
        //{
        //    model.movedir = EntityModel.MOVEDIRTYPE.LEFT;
        //}
        //else if (_horizontal > 0)
        //{
        //    model.movedir = EntityModel.MOVEDIRTYPE.RIGHT;
        //}
        //}
        if (IsForwardHit(model, _layerMask))
            return;

        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
        {
            model.entity_ani.SetFloat("_velocityX", 0);
            return;
        }

        Vector3 movement = Vector3.zero; //이동 좌표 계산
        float speed = 1;
        //점프 상태에 따른 속도값 조절(마찰력처럼)인데 그냥 점프가 나을 듯해서 주석 처리.
        //switch (model.jumpState)
        //{
        //    case EntityModel.EntityJumpState.Jumping:
        //        speed = 0.6f;
        //        break;
        //    case EntityModel.EntityJumpState.InFlight:
        //        speed = 0.4f;
        //        break;
        //}
        if (model.movedir == EntityModel.MOVEDIRTYPE.LEFT)
        {
            movement = Vector3.left;
            speed = -speed;
            model.entity_spr.flipX = true;
        }
        else if (model.movedir == EntityModel.MOVEDIRTYPE.RIGHT)
        {
            movement = Vector3.right;
            model.entity_spr.flipX = false;
        }
        movement.x = speed;

        model.entity_obj.transform.position += movement * GameSceneData.GetSpeed(model.speed) * Time.deltaTime;
        model.entity_ani.SetFloat("_velocityX", Mathf.Abs((int)model.movedir - 1));
    }

    public virtual void Jump(EntityModel model, ref float jumpTime)
    {
        switch (model.jumpState)
        {
            case EntityModel.EntityJumpState.PrepareToJump:
                if (!model.IsGrounded)
                    break;
                model.rigid.velocity = Vector3.zero;  //점프 Tag를 밟으면 해당 코드 필요
                UpdateJumpState(EntityModel.EntityJumpState.Jumping, model, ref jumpTime);
                return;
            case EntityModel.EntityJumpState.Jumping:
                if (model.stopJump || jumpTime >= model.jumpTimeLimit)  //하락 체크
                    return; //어차피 Update에서 공중에 있으면 떨어지게 체크되어 있어서 InFlight로 바꿀 필요 없음(있으면 점프 중에 플랫폼 닿으면 바로 떨어지는 버그 생김)

                model.rigid.velocity = Vector3.zero;
                Vector2 addForce = Vector2.up;
                addForce.y *= model.jumpPower * (jumpTime * 0.1f + 1f);
                model.rigid.AddForce(addForce, ForceMode2D.Impulse);
                jumpTime += Time.fixedDeltaTime;
                model.entity_ani.SetFloat("_velocityY", Mathf.Abs(jumpTime));
                return;
        }
    }

    public virtual void UpdateJumpState(EntityModel.EntityJumpState _state, EntityModel model, ref float jumpTime)
    {
        model.jumpState = _state;

        switch (model.jumpState)
        {
            case EntityModel.EntityJumpState.Grounded:
                model.IsGrounded = true;
                model.entity_ani.SetBool("_grounded", true);
                model.stopJump = false;
                break;
            case EntityModel.EntityJumpState.PrepareToJump:
                model.entity_ani.SetBool("_grounded", false);
                jumpTime = 0;
                model.stopJump = false;
                model.footcol.enabled = false;
                break;
            case EntityModel.EntityJumpState.InFlight:
                model.IsGrounded = false;
                model.entity_ani.SetBool("_grounded", false);
                model.entity_ani.SetFloat("_velocityY", -1);
                model.footcol.enabled = true;
                break;
            case EntityModel.EntityJumpState.Landed:
                UpdateJumpState(EntityModel.EntityJumpState.Grounded, model, ref jumpTime);
                break;
        }
    }
}

public class IsMove : DefaultMoveable
{
    public override void Move(EntityModel model, int _layerMask)
    {
        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
            return;

        if (base.IsForwardHit(model, _layerMask))   //물체에 충돌 시 방향 전환
            model.movedir = model.movedir == EntityModel.MOVEDIRTYPE.LEFT ? EntityModel.MOVEDIRTYPE.RIGHT : EntityModel.MOVEDIRTYPE.LEFT;

        base.Move(model, _layerMask);
    }
    public override void UpdateJumpState(EntityModel.EntityJumpState s, EntityModel m, ref float j) { }
    public override void Jump(EntityModel m, ref float j) { }
}

public class IsFollowPlayer : DefaultMoveable
{
    //플레이어의 현재 방향 체킹.
    void UpdateDirType(EntityModel model)
    {
        model.movedir = EntityModel.MOVEDIRTYPE.CENTER;
        if (PlayerController.player_model.player_tns.position.x < model.entity_tns.transform.position.x)   //플레이어가 자신보다 왼쪽에 있을 때
        {
            model.movedir = EntityModel.MOVEDIRTYPE.LEFT;
        }
        else if (PlayerController.player_model.player_tns.position.x > model.entity_tns.transform.position.x)   //플레이어가 자신보다 오른쪽에 있을 때
        {
            model.movedir = EntityModel.MOVEDIRTYPE.RIGHT;
        }
    }

    public override void Move(EntityModel model, int _layerMask)
    {
        UpdateDirType(model);
        if (model.movedir == EntityModel.MOVEDIRTYPE.CENTER)
            return;

        base.Move(model, _layerMask);
    }
    public override void UpdateJumpState(EntityModel.EntityJumpState s, EntityModel m, ref float j) { }
    public override void Jump(EntityModel m, ref float j) { }
}

public class IsJump : DefaultMoveable
{
    public override void Move(EntityModel m, int l) { }
}

public class IsInputPlayer : DefaultMoveable
{
    public override void Move(EntityModel model, int _layerMask)
    {
        if (model.movedir != EntityModel.MOVEDIRTYPE.CENTER)
            if (model.jumpState == EntityModel.EntityJumpState.Grounded)
                AudioManager.Instance.Play("Walk01");
        base.Move(model, _layerMask);
    }

    public override void UpdateJumpState(EntityModel.EntityJumpState _state, EntityModel model, ref float jumpTime)
    {
        switch (_state)
        {
            case EntityModel.EntityJumpState.PrepareToJump:
                AudioManager.Instance.Play("Jump");
                break;
            case EntityModel.EntityJumpState.Landed:
                AudioManager.Instance.Play("LandOnGround");
                break;
        }
        base.UpdateJumpState(_state, model, ref jumpTime);
    }
}

public class EntityMovableStrategyList  //옵저버 패턴으로 움직임 관리
{
    public List<IEntityMovableStrategy> strategy = new List<IEntityMovableStrategy>();

    public virtual void Move(EntityModel m, int _layoutMask)
    {
        if (m.movableStrategy != null)
            foreach (IEntityMovableStrategy item in m.movableStrategy.strategy)
                item.Move(m, _layoutMask);
    }
    public void Jump(EntityModel m, ref float j)
    {
        if (m.movableStrategy != null)
            foreach (IEntityMovableStrategy item in m.movableStrategy.strategy)
                item.Jump(m, ref j);
    }
}
public class Move : EntityMovableStrategyList
{
    public Move()
    {
        strategy.Add(new IsMove());
    }
}
public class FollowPlayer : EntityMovableStrategyList
{
    public FollowPlayer()
    {
        strategy.Add(new IsFollowPlayer());
    }
}
public class Jump : EntityMovableStrategyList
{
    public Jump()
    {
        strategy.Add(new IsJump());
    }
}
public class MoveAndJump : EntityMovableStrategyList
{
    public MoveAndJump()
    {
        strategy.Add(new IsMove());
        strategy.Add(new IsJump());
    }
}
public class InputPlayer : EntityMovableStrategyList
{
    public InputPlayer()
    {
        strategy.Add(new IsInputPlayer());
    }
}