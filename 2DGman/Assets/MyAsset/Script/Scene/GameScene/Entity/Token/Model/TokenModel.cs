using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [Model]
[System.Serializable]
public abstract class TokenModel : MonoBehaviour
{
    public enum TOKENTYPE
    {
        COIN
    }

    [SerializeField] public TOKENTYPE type;
    [SerializeField] protected string this_name;

    public TokenController controller;

    public abstract void GetToken();
}

public abstract class CoinModel : TokenModel
{
    public int coinCount = 1;
    public CoinModel()
    {
        type = TOKENTYPE.COIN;
    }

    protected virtual void Awake()
    {
        controller = GetComponent<TokenController>();
    }

    public override void GetToken()
    {
        if (type != TOKENTYPE.COIN)
            return;

        GameSceneData.player_controller.Coin += coinCount;
        controller.model.entity_obj.SetActive(false);
    }
}