using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationTriangulo : Formation
{
    [SerializeField] private float distance = 1f;
    [SerializeField] private float angle = 30f;
    
    public FormationTriangulo()
    {
        _numSlots = 7;
        _formationPattern = FormationPattern.Triangulo;
    }

    public override Steering GetSlotLocation(int slotNumber)
    {
        Steering location = new Steering();

        if (slotNumber == 0)
        {
            return location;
        }

        int vertical = (int) Mathf.Ceil(slotNumber / 2f);
        int horizontal = (int) (((slotNumber - 1) % 2 - 0.5f) * 2);

        location.linear = new Vector3(horizontal * vertical, 0f, -vertical);
        location.linear *= distance;

        location.angular = horizontal * vertical * angle;
        return location;
    }
}
