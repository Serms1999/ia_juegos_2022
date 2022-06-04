using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviour
{

    // Declara las variables que necesites para este SteeringBehaviour

    void Start()
    {
        this.nameSteering = SteeringNames.Flee;
    }


    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        Vector3 direction;

        // Calcula el steering.
        direction = agent.Position - Target.Position;
        
        steer.linear = direction;
        steer.linear.Normalize();
        steer.linear *= agent.MaxAcceleration;

        // Retornamos el resultado final.
        return steer;
    }
}