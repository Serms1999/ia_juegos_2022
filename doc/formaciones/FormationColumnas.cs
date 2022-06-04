using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationColumnas: Formation
{
    [SerializeField] private float distance = 3f;
    
    [SerializeField] private float angle = 30f;

    public FormationColumnas()
    {
        _numSlots = 8;
        _formationPattern = FormationPattern.Columnas;
    }

    public override Steering GetSlotLocation(int slotNumber)
    {
        Steering location = new Steering();
        if (slotNumber == 0)
        {
            return location;
        }
        
        int vertical = (int) Mathf.Ceil(slotNumber / 2f);
        int horizontal = 0;
        if (slotNumber < _numSlots - 1)
        {
            switch (slotNumber % 2)
            {
                case 0:
                    horizontal = -1;
                    break;
                case 1:
                    horizontal = 1;
                    break;


            }
        }


        location.linear = new Vector3(horizontal, 0f, -vertical);
        location.linear *= distance;

        location.angular = horizontal * vertical * angle;
        return location;
        
    }
}
