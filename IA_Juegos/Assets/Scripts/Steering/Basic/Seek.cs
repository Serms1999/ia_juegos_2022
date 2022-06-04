using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{

    // Declara las variables que necesites para este SteeringBehaviour
    private float TimeToTarget = 0.1f;
    
    void Start()
    {
        this.nameSteering = SteeringNames.Seek;
    }


    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        Vector3 direction;

        // Calcula el steering.
        direction = Target.Position - agent.Position;

        // Si estamos lo sufientemente cerca, frenamos
        if (direction.magnitude < agent.InteriorRadius)
        {
            steer.linear = (direction - agent.Velocity) / TimeToTarget;
            return steer;
        }

        // Si no, usamos maxima aceleracion
        steer.linear = direction;
        steer.linear.Normalize();
        steer.linear *= agent.MaxAcceleration;

        // Retornamos el resultado final.
        return steer;
    }
}