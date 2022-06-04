using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek_IanMillington : SteeringBehaviour
{


    public virtual void Awake()
    {
        nameSteering = "Seek Ian Millingt.";
    }

    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();

        // Determinar el vector de velocidad como el vector diferencia de las posiciones
        // Vector3 direction = 
        Vector3 direction = Vector3.zero;

        // Aplica máxima aceleración.
        // Vector3 newAcceleration = 
        Vector3 newAcceleration = Vector3.zero;


        // Guardamos en la variable de salida
        steer.linear = newAcceleration;
        steer.angular = 0; // NO genera acleración angular


        return steer;
    }
}
