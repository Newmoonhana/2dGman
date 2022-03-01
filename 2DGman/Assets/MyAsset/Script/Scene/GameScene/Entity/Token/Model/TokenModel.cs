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

    [SerializeField] protected TOKENTYPE type;
    [SerializeField] protected string this_name;

    public TokenController controller;
}

public abstract class CoinModel : TokenModel
{
    public CoinModel()
    {
        type = TOKENTYPE.COIN;
    }

    protected virtual void Awake()
    {
        controller = GetComponent<TokenController>();
    }
}