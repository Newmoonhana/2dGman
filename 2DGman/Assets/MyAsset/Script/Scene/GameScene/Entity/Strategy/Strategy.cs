using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyList<T>
{
    public List<T> strategy = new List<T>();

    public virtual bool IsNotNull()
    {
        if (strategy != null)
            return true;
        return false;
    }
}
