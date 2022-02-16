using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum COLTYPE
{
    COLLISION,
    TRIGGER,

    _MAX
}

public enum ColHitState
{
    None,
    Enter,
    Stay,
    Exit,

    _MAX
}

public class IsColliderHit : MonoBehaviour
{
    public COLTYPE type;
    public ColHitState state = ColHitState.None;
    public Collision2D other_col_COLLISION;
    public Collider2D other_col_TRIGGER;

    private void OnCollisionEnter2D(Collision2D other)
    {
        type = COLTYPE.COLLISION;
        state = ColHitState.Enter;
        other_col_COLLISION = other;
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        type = COLTYPE.COLLISION;
        state = ColHitState.Stay;
        other_col_COLLISION = other;
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        type = COLTYPE.COLLISION;
        state = ColHitState.Exit;
        other_col_COLLISION = other;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        type = COLTYPE.TRIGGER;
        state = ColHitState.Enter;
        other_col_TRIGGER = other;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        type = COLTYPE.TRIGGER;
        state = ColHitState.Stay;
        other_col_TRIGGER = other;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        type = COLTYPE.TRIGGER;
        state = ColHitState.Exit;
        other_col_TRIGGER = other;
    }
}
