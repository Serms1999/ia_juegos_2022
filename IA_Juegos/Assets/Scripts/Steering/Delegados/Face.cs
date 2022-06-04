using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align
{
    // Declara las variables que necesites para este SteeringBehavior


    void Start()
    {
        this.nameSteering = SteeringNames.Face;
    }


    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        Agent originalTarget = Target;
        

        // Conseguimos la direccion hacia el objetivo
        Vector3 direction = Target.Position - agent.Position;

        // Si estamos juntos no hacemos nada
        if (direction.magnitude == 0)
        {
            return steer;
        }

        // Creamos un personaje auxiliar
        GameObject auxiliar = new GameObject("DetectPlayer");
        Target = auxiliar.AddComponent<Agent>();
        auxiliar.tag = "Auxiliar";

        // Le asignamos la rotacion necesaria
        Target.Orientation = Bodi.PositionToAngle(direction);
        
        // Delegamos a Align
        steer = base.GetSteering(agent);
        
        // Borramos al personaje auxiliar y volvemos a asignar el antiguo
        Destroy(auxiliar);
        Target = originalTarget;

        // Retornamos el resultado final.
        return steer;
    }
}
