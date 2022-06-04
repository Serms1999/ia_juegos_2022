using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{

    // Declara las variables que necesites para este SteeringBehaviour
    private float TimeToTarget = 0.1f;

    
    void Start()
    {
        this.nameSteering = SteeringNames.VelocityMatching;
    }


    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();

        // Intentamos igualar la velocidad
        steer.linear = target.Velocity - agent.Velocity;
        steer.linear /= TimeToTarget;
        
        //Si es demasiado rápido
        if (steer.linear.magnitude > agent.MaxAcceleration)
        {
            steer.linear.Normalize();
            steer.linear *= agent.MaxAcceleration;
        }
        
        // Retornamos el resultado final.
        return steer;
    }
}