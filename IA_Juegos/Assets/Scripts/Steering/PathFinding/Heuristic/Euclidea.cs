using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Euclidea : Distance
{
    public float GetDistance(Node n1, Node n2)
    {
        Vector3 pos1 = n1.WorldPosition;
        Vector3 pos2 = n2.WorldPosition;

        return Vector3.Distance(pos1, pos2);
    }
}
