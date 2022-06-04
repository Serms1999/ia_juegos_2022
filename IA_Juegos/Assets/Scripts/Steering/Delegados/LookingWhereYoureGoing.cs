using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingWhereYoureGoing : Align
{
    // Declara las variables que necesites para este SteeringBehavior
    
    
    void Start()
    {
        this.nameSteering = SteeringNames.LookingWhereYoureGoing;
    }

    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        
        // Si el personaje no se mueve, no hacemos nada
        if (agent.Velocity.magnitude == 0)
        {
            return steer;
        }
        
        // En otro caso nos creamos un agente auxiliar
        // Creamos un personaje auxiliar
        GameObject auxiliar = new GameObject("DetectPlayer");
        Target = auxiliar.AddComponent<Agent>();
        auxiliar.tag = "Auxiliar";

        // Le asignamos la rotacion necesaria
        Target.Orientation = Bodi.PositionToAngle(agent.Velocity);
        
        // Delegamos a Align
        steer = base.GetSteering(agent);
        
        // Borramos al personaje auxiliar y volvemos a asignar el antiguo
        Destroy(auxiliar);
        
        // Retornamos el resultado final
        return steer;
    }
}
