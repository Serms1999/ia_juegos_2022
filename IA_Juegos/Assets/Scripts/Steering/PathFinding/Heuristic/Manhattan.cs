using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manhattan : Distance
{
    public float GetDistance(Node n1, Node n2)
    {
        int dx = Mathf.Abs(n1.GridPosition.x - n2.GridPosition.x);
        int dy = Mathf.Abs(n1.GridPosition.y - n2.GridPosition.y);

        return dx + dy;
    }
}
