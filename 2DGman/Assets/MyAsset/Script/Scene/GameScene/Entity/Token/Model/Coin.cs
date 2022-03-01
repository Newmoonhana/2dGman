using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : CoinModel
{
    public Coin()
    {
        this_name = "Coin";
    }

    protected override void Awake()
    {
        base.Awake();

        controller.Entity_obj = gameObject;
        controller.token_model = this;
    }
}
