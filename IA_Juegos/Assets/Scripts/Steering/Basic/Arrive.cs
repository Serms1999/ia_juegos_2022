using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Arrive : SteeringBehaviour
{

    // Declara las variables que necesites para este SteeringBehaviour
    private float TimeToTarget = 0.1f;

    void Start()
    {
        this.nameSteering = SteeringNames.Arrive;
    }


    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();

        Vector3 direction;
        float distance;
        float targetSpeed;
        Vector3 targetVelocity;

        // Conseguimos la distancia al objetivo
        direction = Target.Position - agent.Position;
        distance = direction.magnitude;

        // Si estamos lo sufiente cerca, frenamos
        if (distance < agent.InteriorRadius)
        {
            steer.linear = (direction - agent.Velocity) / TimeToTarget;
            return steer;
        }

        // Si estamos muy lejos, vamos a maxima velocidad
        if (distance > agent.ArrivalRadius)
        {
            targetSpeed = agent.MaxSpeed;
        }
        // En otro caso, calculamos una aceleracion ajustada 
        else
        {
            targetSpeed = agent.MaxSpeed * (distance / agent.ArrivalRadius);
        }
        
        // Combinamos la direccion y la velocidad
        targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        steer.linear = targetVelocity - agent.Velocity;
        steer.linear /= TimeToTarget;

        // Si la aceleracion es muy alta ponemos a la maxima
        if (steer.linear.magnitude > agent.MaxAcceleration)
        {
            steer.linear.Normalize();
            steer.linear *= agent.MaxAcceleration;
        }

        return steer;
    }
}