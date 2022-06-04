using System.Collections.Generic;
using UnityEngine;

public class Alignment : Align
{    
    private List<Agent> neighbours = new List<Agent>();
    int _numNeighbours = 0;

    [SerializeField] private float maxCohesion;

    public override Steering GetSteering(Agent agent)
    {
        float averageHeading = 0;
        Steering steer = new Steering();
        if (neighbours.Count < 1 )
        {
            return steer;
        }
        foreach (Agent n in neighbours)
        {
            // Compruebo que no soy yo
            if (!n.Equals(agent))
            {
                Vector3 direction = agent.Position - n.Position;
                float distance = direction.magnitude;
                if (distance < maxCohesion)
                {
                    averageHeading += n.Orientation;
                    _numNeighbours++;
                }
            }

        }
        // Dividimos para tener el heading promedio
        averageHeading /= _numNeighbours;
        //steering vector is the difference between the average and our character’s current velocity
        
        averageHeading -= agent.Orientation;
        // Mapeamos al intervalo (-pi,pi)
        averageHeading = Bodi.MapToRange(averageHeading,
            new Range(-180, 180));
        steer.angular += averageHeading;
        

        
        return base.GetSteering(agent);
    }
}
