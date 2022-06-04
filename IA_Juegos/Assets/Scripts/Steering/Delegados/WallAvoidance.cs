using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WallAvoidance : Seek
{
    // Declara las variables que necesites para este SteeringBehavior
    [SerializeField]
    private float avoidDistance = 1.5f;
    [SerializeField]
    private GameController controller;

    private CollisionDetector _collisionDetector;
    
    void Start()
    {
        this.nameSteering = SteeringNames.WallAvoidance;
        _collisionDetector = controller.CollisionDetector;
    }

    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        Collision collision;
        
        // Comprobamos la colision
        collision = _collisionDetector.GetCollision(agent);
        
        // Si no hay colision, no hacemos nada
        if (collision == null)
        {
            return steer;
        }
        
        // Si la hay creamos un objetivo auxiliar
        GameObject auxiliar = new GameObject("DetectPlayer");
        Target = auxiliar.AddComponent<Agent>();
        auxiliar.tag = "Auxiliar";
        
        // Le asignamos la posicion
        Target.Position = collision.Position + collision.Normal * avoidDistance;
        
        // Delegamos en Seek
        steer = base.GetSteering(agent);
        
        // Borramos el objetivo auxiliar
        Destroy(auxiliar);

        return steer;
    }
}
