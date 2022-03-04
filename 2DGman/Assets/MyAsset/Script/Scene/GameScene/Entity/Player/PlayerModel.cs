using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [Model]
[System.Serializable]
public abstract class PlayerModel
{
    public enum PLAYERTYPE
    {
        DEFAULTPC
    }

    [SerializeField] protected PLAYERTYPE type;
    [SerializeField] protected string this_name;

    public Transform player_tns;
    public PlayerController playerCon_src;
    public float hp = 3;
    public int lifePoint = 3;

    public SPEEDTYPE speed;
    public SPEEDTYPE speed_dash;
    public float jumpPower;
    public float jumpTimeLimit;

    public static int coinCount = 0;
}

public class DefaultPC : PlayerModel
{
    public DefaultPC()
    {
        type = PLAYERTYPE.DEFAULTPC;
        speed = SPEEDTYPE.NORMAL;
        speed_dash = SPEEDTYPE.FAST;
        jumpPower = 6f;
        jumpTimeLimit = 0.4f;
    }
}