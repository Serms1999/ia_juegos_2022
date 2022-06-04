using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DistanceMethods
{
    Euclidea,
    Chebychev,
    Manhattan
}

public interface Distance
{
    public float GetDistance(Node n1, Node n2);
}
