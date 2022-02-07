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
    public Collision2D other_col;

    private void OnCollisionEnter2D(Collision2D other)
    {
        type = COLTYPE.COLLISION;
        state = ColHitState.Enter;
        other_col = other;
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        type = COLTYPE.COLLISION;
        state = ColHitState.Stay;
        other_col = other;
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        type = COLTYPE.COLLISION;
        state = ColHitState.Exit;
        other_col = other;
    }
}
