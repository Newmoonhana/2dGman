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

    public int Life { get { return player_model.lifePoint; } set { player_model.lifePoint = value; player_view.OnLifeChanged(value); } }
    public int Coin { get { return PlayerModel.coinCount; } set { SetCoin(value); } }

    public override bool SetHp(float _hp, float _hpMax, DAMAGETYPE _type) //hp가 0 이하일 경우 false
    {
        float value = model.hp;
        if (_type == DAMAGETYPE.EQUAL)
        {
            value = _hp;
        }
        else if (_type == DAMAGETYPE.DAMAGE)
        {
            value -= _hp;
        }
        else if (_type == DAMAGETYPE.HEAL)
        {
            value += _hp;
        }
        player_view.OnHpChanged(value, _hpMax, true);
        return base.SetHp(_hp, _hpMax, _type);
    }
    public void SetCoin(int _coin)
    {
        bool isPlus = false;
        if (PlayerModel.coinCount < _coin)
            isPlus = true;
        PlayerModel.coinCount = _coin;

        if (isPlus) //코인 추가
        {
            AudioManager.Instance.Play("Item3");
            if (PlayerModel.coinCount >= 100)   //코인 100개 모을 시 라이프 상승
            {
                PlayerModel.coinCount -= 100;
                Life += 1;
                AudioManager.Instance.Play("Item2");
            }
        }
        
        player_view.OnCoinChanged(PlayerModel.coinCount);
    }

    public override void UpdateState(EntityModel.EntityState _state)
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
                CancelInvoke("IsUnBeadEnd");
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
                if (Life <= 0)
                    GameSceneData.clearCanvas_obj.SetActive(true);
                break;
            case EntityModel.EntityState.VICTORY:
                if (model.entity_ani != null)
                    model.entity_ani.SetTrigger("_victory");
                GameSceneData.clearCanvas_obj.SetActive(true);
                GameSceneData.clearCanvas_obj.transform.GetChild(0).GetComponent<TextMesh>().text = "\nClear!";
                break;
        }
    }
    public void IsHurtEnd()
    {
        if (model.hp > 0)
        {
            UpdateState(EntityModel.EntityState.UNBEAT);
            Invoke("IsUnBeatEnd", 3f);
        }
    }
    public void IsDeadEnd()
    {
        Invoke("Resurrect", 3f);
    }
    void IsUnBeatEnd()
    {
        UpdateState(EntityModel.EntityState.DEFAULT);
    }

    void Resurrect()
    {
        if (Life <= 0)
            return;
        Life -= 1;
        SetHp(player_model.hp, player_model.hp, DAMAGETYPE.EQUAL);
        model.entity_tns.SetParent(GameSceneData.startpoint_tns);
        model.entity_tns.localPosition = Vector3.zero;
        UpdateState(EntityModel.EntityState.DEFAULT);
        UpdateJumpState(EntityModel.EntityJumpState.Grounded, model, ref jumpTime);
    }

    protected override void Start()
    {
        player_view.onInit(Life, model.hp, player_model.hp);
        player_model.player_tns = GameObject.Find("Player").transform;
        player_model.playerCon_src = GameSceneData.player_controller;
        model.SetSubValue(player_model.speed, player_model.jumpPower, player_model.jumpTimeLimit);
        EntityMovableStrategyFactory ef = new LandFactory();
        model.movableStrategy = ef.CreateMovableStrategy("input_player");
        EntityIsHitStrategyFactory ef_ih = new IsHitFactory();
        model.ishitStrategy = ef_ih.CreateIsHitStrategy("player");

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
            Down(model);

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
                if (Input.GetAxisRaw("Horizontal") < 0)
                    model.movedir = EntityModel.MOVEDIRTYPE.LEFT;
                else if (Input.GetAxisRaw("Horizontal") > 0)
                    model.movedir = EntityModel.MOVEDIRTYPE.RIGHT;
                else
                    model.movedir = EntityModel.MOVEDIRTYPE.CENTER;
                Move(model, (1 << LayerMask.NameToLayer("Land")));
            }

        base.FixedUpdate();
    }
}
