using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[View]
[System.Serializable]
public class PlayerView
{
    public Text lifeText;   //라이프 표시
}

// [Controller]
public class PlayerController : EntityController
{
    public static PlayerModel player_model = new DefaultPC();
    [SerializeField] PlayerView player_view = new PlayerView();
    //tmp
    Vector2 _vec2;
    public bool IsJump = false, IsDown = false;

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

    protected override void UpdateJumpState(EntityModel.EntityJumpState _state)
    {
        switch (_state)
        {
            case EntityModel.EntityJumpState.Grounded:
                IsJump = false;
                IsDown = false;
                break;
            case EntityModel.EntityJumpState.PrepareToJump:
                AudioManager.Instance.Play("jump");
                IsJump = true;
                break;
            case EntityModel.EntityJumpState.Landed:
                AudioManager.Instance.Play("LandOnGround");
                break;
        }
        base.UpdateJumpState(_state);
    }

    protected override void Start()
    {
        player_model.player_tns = GameObject.Find("Player").transform;
        player_model.playerCon_src = GameObject.Find("Player Controller").GetComponent<PlayerController>();
        model.SetSubValue(player_model.speed, player_model.jumpPower, player_model.jumpTimeLimit);

        base.Start();
    }

    void Update()
    {
        if (Input.GetButtonDown("Down"))
            if (model.foot_col_src.type == COLTYPE.COLLISION)   //중복 하강 방지 코드
                if (model.foot_col_src.state == ColHitState.Stay)
                    if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Platform"))
                    {
                        IsDown = true;
                        _vec2 = Vector2.down;
                        _vec2.y *= model.jumpPower / 10 * (jumpTime * 0.1f + 1f);
                        model.rigid.AddForce(_vec2, ForceMode2D.Impulse);
                        AudioManager.Instance.Play("jump");
                    }

        if (model.jumpState == EntityModel.EntityJumpState.Grounded && Input.GetButtonDown("Jump"))
        {
            UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump);
        }
        else if (Input.GetButtonUp("Jump"))
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
            Move(Input.GetAxisRaw("Horizontal"));
        }

        if (model.foot_col_src.type == COLTYPE.COLLISION)
        {
            if (model.foot_col_src.state == ColHitState.Enter)
            {
                //몬스터 충돌 판정
                if (model.foot_col_src.other_col_COLLISION.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    model.foot_col_src.other_col_COLLISION.gameObject.SetActive(false);
                    UpdateJumpState(EntityModel.EntityJumpState.Grounded);
                    UpdateJumpState(EntityModel.EntityJumpState.PrepareToJump);
                }
            }
        }

        base.FixedUpdate();
    }

    public override void Move(float _horizontal)
    {
        if (_horizontal != 0)
            if (model.jumpState == EntityModel.EntityJumpState.Grounded)
                AudioManager.Instance.Play("Walk01");
        base.Move(_horizontal);
    }
}
