using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [Model]
public enum PLAYERTYPE
{
    DEFAULTPC,
    HUMAN
}

public abstract class PlayerModel
{
    [SerializeField] protected PLAYERTYPE type;
    [SerializeField] protected string this_name;

    public Transform player_tns;
    public PlayerController playerCon_src;
    public int lifePoint = 3;

    public SPEEDTYPE speed;
    public float jumpPower;
    public float jumpTimeLimit;
}

public class DefaultPC : PlayerModel
{
    public DefaultPC()
    {
        type = PLAYERTYPE.DEFAULTPC;
        speed = SPEEDTYPE.NORMAL;
        jumpPower = 6f;
        jumpTimeLimit = 0.4f;
    }
}