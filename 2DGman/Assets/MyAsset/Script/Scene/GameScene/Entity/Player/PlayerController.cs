using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[View]
[System.Serializable]
public class PlayerView
{
    public Text lifeText;   //라이프 표시
    public Text coinText;   //먹은 코인 개수 표시
}

// [Controller]
public class PlayerController : EntityController
{
    public static PlayerModel player_model = new DefaultPC();
    [SerializeField] PlayerView player_view = new PlayerView();
    //tmp


    public int Life { get { return player_model.lifePoint; } set { player_model.lifePoint = value; OnLifeChanged(); } }
    // 화면에 라이프 표시
    public void OnLifeChanged()
    {
        player_view.lifeText.text = $"Life x {Life}";
    }

    protected override void UpdateState(EntityModel.EntityState _state)
    {
        base.UpdateState(_state);

        switch (model.state)
        {
            case EntityModel.EntityState.HURT:
                AudioManager.Instance.Play("Hurt");
                break;
            case EntityModel.EntityState.DIE:
                AudioManager.Instance.Play("Death");
                break;
        }
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
        if (model.state == EntityModel.EntityState.DEFAULT)
        {
            Move(Input.GetAxisRaw("Horizontal"), model);
        }

        if (model.foot_col_src.type == COLTYPE.COLLISION)
        {
            if (model.foot_col_src.state == ColHitState.Enter)
            {
                //자동 점프 판정 충돌 판정
                if (model.foot_col_src.other_col_COLLISION.gameObject.tag == "jump pole")
                {
                    if (model.jumpState == EntityModel.EntityJumpState.Grounded || model.jumpState == EntityModel.EntityJumpState.InFlight)
                    {
                        //몬스터 충돌 판정
                        if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        {
                            EnemyController enemy = model.foot_col_src.other_col_COLLISION.gameObject.GetComponent<EnemyController>();

                            if (enemy.model.state == EntityModel.EntityState.DEFAULT)
                            {
                                enemy.SetHp(1, DAMAGETYPE.DAMAGE);
                                UpdateJumpState(EntityModel.EntityJumpState.Grounded, model, ref jumpTime);
                                UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump, model, ref jumpTime);
                            }
                        }
                    }
                }
            }
        }

        base.FixedUpdate();
    }
}
