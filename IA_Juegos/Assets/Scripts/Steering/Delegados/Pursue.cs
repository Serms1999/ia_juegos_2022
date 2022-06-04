using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : Seek
{
    // Declara las variables que necesites para este SteeringBehavior
    //Tiempo maximo de prediccion
    public float maxPrediction = 0f;
    float prediction;

    void Start()
    {
        this.nameSteering = SteeringNames.Pursue;
    }

    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        Agent originalTarget = Target;
        
        // Conseguimos la direccion hacia el objetivo
        Vector3 direction = Target.Position - agent.Position;
        float distance = direction.magnitude;
        float speed = agent.Velocity.magnitude;
        
        // Comprobamos si la velocidad es muy pequeña respecto a la distancia
        if (speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        // Creamos un personaje auxiliar
        GameObject auxiliar = new GameObject("DetectPlayer");
        Target = auxiliar.AddComponent<Agent>();
        auxiliar.tag = "Auxiliar";
        
        // Le asignamos la posicion necesaria
        target.Position = originalTarget.Position;
        target.Position += originalTarget.Velocity * prediction;

        // Delegamos en Seek
        steer = base.GetSteering(agent);
        
        // Borramos al personaje auxiliar y volvemos a asignar el antiguo
        Destroy(auxiliar);
        Target = originalTarget;
        
        return steer;
    }
}