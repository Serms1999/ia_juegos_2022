using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Cohesion : Seek
{
    [SerializeField]
    private float maxCohesion;
    private readonly List<Agent> _neighbours = new List<Agent>();
    private int _numberOfNeighbours = 0;

    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        Vector3 centerOfMass = new Vector3(0, 0, 0);
        foreach (Agent n  in _neighbours)
        {
            if (!n.Equals(agent))
            {
                Vector3 direction = agent.Position - n.Position;
                float distance = direction.magnitude;
                if (distance < maxCohesion)
                {   
                    // Sumamos la posicion de nuestros vecinos
                    centerOfMass += n.Position;
                    _numberOfNeighbours++;
                }
            }
        }

        if (_numberOfNeighbours == 0)
            return steer;
        // Get the average position of ourself and our neighbours
        centerOfMass /= _numberOfNeighbours;
        Target.Position = centerOfMass;

        return base.GetSteering(agent);
    }
}
