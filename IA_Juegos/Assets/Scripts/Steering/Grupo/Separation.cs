using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringBehaviour
{
    [SerializeField] private float minSeparation = 2f;

    [SerializeField] private float maxForce = 2f;

    [SerializeField] private GameController gameController;

    public GameController GameController
    {
        set { gameController = value; }
    }

    private List<Agent> _neighbours;
    // Start is called before the first frame update
    void Start()
    {
        this.nameSteering = SteeringNames.Separation;
    }

    public override Steering GetSteering(Agent agent)
    {
        Vector3 direction;
        float distance;
        float repulsiveForce;
        
        Steering steer = new Steering();
        _neighbours = gameController.GetNearAgents(agent, minSeparation);
        if (_neighbours.Count < 1 )
        {
            return steer;
        }
        
        // Calculamos la distancia para cada vecino
        foreach (Agent neighbour in _neighbours)
        {
            // Compruebo que no sea el propio agente
            if (neighbour.Equals(agent))
            {
                continue;
            }

            direction = agent.Position - neighbour.Position;
            distance = direction.magnitude;
            
            if (distance < minSeparation)
            {
                // Calculamos la fuerza de repulsión
                repulsiveForce = Mathf.Min(maxForce / Mathf.Pow(distance, 2), agent.MaxAcceleration);
                direction.Normalize();
                steer.linear += repulsiveForce * direction;
            }
        }
        
        // Devolvemos el steering
        return steer;
    }
}
