using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [Controller]
public class PlayerController : EntityController
{
    public static PlayerModel player_model = new DefaultPC();
    public PlayerView player_view;
    //tmp
    Color color_tmp;

    public int Life { get { return player_model.lifePoint; } set { player_model.lifePoint = value; OnLifeChanged(); } }
    // 화면에 라이프 표시
    public void OnLifeChanged()
    {
        player_view.lifeText.text = $"Life x {Life}";
    }

    public bool SetHp(float _hp, DAMAGETYPE _type) //hp가 0 이하일 경우 false
    {
        if (_type == DAMAGETYPE.DAMAGE)
        {
            player_model.hp -= _hp;
            UpdateState(EntityModel.EntityState.HURT);
            if (player_model.hp <= 0)
                UpdateState(EntityModel.EntityState.DIE);
            return true;
        }

        else if (_type == DAMAGETYPE.HEAL)
        {
            player_model.hp += _hp;
            return true;
        }

        return false;
    }

    protected override void UpdateState(EntityModel.EntityState _state)
    {
        base.UpdateState(_state);

        switch (model.state)
        {
            case EntityModel.EntityState.DEFAULT:
                color_tmp = model.entity_spr.color;
                color_tmp.a = 1;
                model.entity_spr.color = color_tmp;
                break;
            case EntityModel.EntityState.HURT:
                AudioManager.Instance.Play("Hurt");
                break;
            case EntityModel.EntityState.UNBEAT:
                model.entity_col.enabled = true;
                color_tmp = model.entity_spr.color;
                color_tmp.a = 0.5f;
                model.entity_spr.color = color_tmp;
                break;
            case EntityModel.EntityState.DIE:
                AudioManager.Instance.Play("Death");
                break;
        }
    }
    public void IsHurtEnd()
    {
        if (player_model.hp > 0)
        {
            UpdateState(EntityModel.EntityState.UNBEAT);
            Invoke("IsUnBeatEnd", 3f);
        } 
    }
    public void IsDeadEnd()
    {
        
    }
    void IsUnBeatEnd()
    {
        UpdateState(EntityModel.EntityState.DEFAULT);
    }

    protected override void Start()
    {
        Life = Life;
        player_model.player_tns = GameObject.Find("Player").transform;
        player_model.playerCon_src = GameObject.Find("Player Controller").GetComponent<PlayerController>();
        model.SetSubValue(player_model.speed, player_model.jumpPower, player_model.jumpTimeLimit);
        model.movableStrategy = new IsInputPlayer();

        base.Start();
    }

    void Update()
    {
        //대시 체크
        if (model.jumpState == EntityModel.EntityJumpState.Grounded)    //점프 중엔 영향 끼치지 않음
        {
            if (UserInputManager.Instance.model.IsButtonInput("Dash", UserInputModel.inputItem.TYPE.BUTTON))
                model.speed = player_model.speed_dash;
            else
                model.speed = player_model.speed;
        } 

        //플랫폼에서 하단 점프
        if (UserInputManager.Instance.model.IsButtonInput("Down", UserInputModel.inputItem.TYPE.BUTTONDOWN))
            if (model.foot_col_src.type == COLTYPE.COLLISION)   //중복 하강 방지 코드
                if (model.foot_col_src.state == ColHitState.Stay)
                    if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Platform"))
                    {
                        AudioManager.Instance.Play("Jump");
                    }

        if (model.jumpState == EntityModel.EntityJumpState.Grounded)
        {
            if (UserInputManager.Instance.model.IsButtonInput("Jump", UserInputModel.inputItem.TYPE.BUTTONDOWN))
                UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump, model, ref jumpTime);
        }

        if (UserInputManager.Instance.model.IsButtonInput("Jump", UserInputModel.inputItem.TYPE.BUTTONUP))
        {
            if (model.jumpState == EntityModel.EntityJumpState.Jumping || model.jumpState == EntityModel.EntityJumpState.InFlight)
            {
                model.stopJump = true;
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (model.state != EntityModel.EntityState.HURT)
            if (model.state != EntityModel.EntityState.DIE)
            {
                Move(Input.GetAxisRaw("Horizontal"), model);
            }

        bool isEnemyjump = false;
        if (model.foot_col_src.type == COLTYPE.COLLISION)
        {
            if (model.foot_col_src.state == ColHitState.Enter)
            {
                //자동 점프 판정 충돌 판정
                if (model.foot_col_src.other_col_COLLISION.gameObject.tag == "jump pole")
                {
                    if (model.jumpState == EntityModel.EntityJumpState.Grounded || model.jumpState == EntityModel.EntityJumpState.InFlight)
                    {
                        //몬스터 충돌 판정(몬스터 밟고 점프)
                        if (model.state != EntityModel.EntityState.HURT)
                            if (model.state != EntityModel.EntityState.DIE)
                                if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                                {
                                    EnemyController enemy = model.foot_col_src.other_col_COLLISION.gameObject.GetComponent<EnemyController>();

                                    if (enemy.model.state == EntityModel.EntityState.DEFAULT)
                                    {
                                        enemy.model.rigid.velocity = Vector2.zero;
                                        enemy.SetHp(1, DAMAGETYPE.DAMAGE);

                                        if (enemy.model.jumpState == EntityModel.EntityJumpState.Jumping)
                                        {
                                            float tmp = 0;
                                            enemy.UpdateJumpState(EntityModel.EntityJumpState.InFlight, enemy.model, ref tmp);
                                        }

                                        UpdateJumpState(EntityModel.EntityJumpState.Grounded, model, ref jumpTime);
                                        UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump, model, ref jumpTime);
                                        isEnemyjump = true;
                                    }
                                }
                    }
                }
            }
        }

        if (!isEnemyjump)
            if (model.entity_col_src.type == COLTYPE.COLLISION)
            {
                if (model.entity_col_src.state == ColHitState.Stay)
                {
                    //몬스터 충돌 판정
                    if (model.state == EntityModel.EntityState.DEFAULT)
                        if (model.entity_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                            if (model.entity_col_src.other_col_COLLISION.gameObject.GetComponent<EnemyController>().model.state == EntityModel.EntityState.DEFAULT)
                            {
                                SetHp(1, DAMAGETYPE.DAMAGE);
                            }
                }
            }

        base.FixedUpdate();
    }
}
