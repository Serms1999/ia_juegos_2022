using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Wander : Face
{
    // Declara las variables que necesites para este SteeringBehavior
    [Tooltip("Distancia del personaje al círculo del wander")]
    [SerializeField] private float wanderOffset = 15f;

    [Tooltip("Radio del círculo del wander")]
    [SerializeField] private float wanderRadius = 8f;

    [Tooltip("Máxima rotación del wander")]
    [SerializeField] private float wanderRate = 60f;
    
    private float _wanderOrientation;

    private const int FPSLimit = 5;
    private int fpsCount = 0;

    private float RandomBinomial()
    {
        Random random = new Random();
        return (float) (random.NextDouble() - random.NextDouble());
    }

    void Start()
    {
        this.nameSteering = SteeringNames.Wander;
    }


    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        
        // Lo ejecutamos solo cada FPSLimit frames para mejorar el movimiento
        if (fpsCount++ % FPSLimit == 0)
        {
            return steer;
        }

        // Calcula el steering.
        // Creamos un objetivo auxiliar
        GameObject auxiliar = new GameObject("DetectPlayer");
        Target = auxiliar.AddComponent<Agent>();
        auxiliar.tag = "Auxiliar";
        
        // Calculamos la orientacion del objetivo
        _wanderOrientation += RandomBinomial() * wanderRate;
        Target.Orientation = _wanderOrientation + agent.Orientation;
        
        // Calculamos el centro del circulo del wander
        Target.Position = agent.Position + wanderOffset * agent.OrientationToVector();
        
        // Calculamos la posicion del objetivo
        Target.Position += wanderRadius * Target.OrientationToVector();
        
        // Delegamos en Face
        steer = base.GetSteering(agent);
        
        // Usamos maxima aceleacion lineal
        steer.linear = agent.MaxAcceleration * agent.OrientationToVector();
        
        // Borramos el objetivo auxiliar
        Destroy(auxiliar);
        
        // Retornamos el resultado final.
        return steer;
    }
}
