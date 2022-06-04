using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehaviour
{

    // Declara las variables que necesites para este SteeringBehavior
    private float TimeToTarget = 0.1f;


    void Start()
    {
        this.nameSteering = SteeringNames.Align;
    }


    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        float rotation, rotationSize, targetRotation, angularAcceleration;

        // Calcula el steering.
        // Calculamos la diferencia entre donde miran los agentes
        rotation = Target.Orientation - agent.Orientation;
        
        // Mapeamos el resultado al intervalo (-pi, pi)
        rotation = Bodi.MapToRange(rotation, new Range(-180, 180));
        rotationSize = Mathf.Abs(rotation);

        // Si ya estamos alineados, no hacemos nada
        if (rotationSize < Target.InteriorAngle)
        {
            steer.angular = (rotation - agent.Rotation) / TimeToTarget;
            return steer;
        }
        
        // Si estamos suficientemente desalineados, usamos maxima rotacion
        if (rotationSize > Target.ExteriorAngle)
        {
            targetRotation = agent.MaxRotation;
        }
        // Si no, calculamos un movimiento escalado
        else
        {
            targetRotation = agent.MaxRotation * (rotationSize / agent.ExteriorAngle);
        }
        
        // Obtenemos la direccion del movimiento
        targetRotation *= rotation / rotationSize;
        
        // Calculamos la aceleracion
        steer.angular = targetRotation - agent.Rotation;
        steer.angular /= TimeToTarget;
        
        // Comprobamos si la aceleracion es demasiado grande
        angularAcceleration = Mathf.Abs(steer.angular);
        if (angularAcceleration > agent.MaxRotation)
        {
            steer.angular /= angularAcceleration;
            steer.angular *= agent.MaxRotation;
        }

        // Retornamos el resultado final.
        return steer;
    }
}