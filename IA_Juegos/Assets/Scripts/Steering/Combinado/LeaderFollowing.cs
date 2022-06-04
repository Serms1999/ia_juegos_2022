using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderFollowing : SteeringBehaviour
{
    // Declara las variables que necesites para este SteeringBehavior
    private float _arriveWeight;
    private float _separationWeight;
    private float _fleeWeight;
    private float _auxLeaderDistance;
    private GameController _gameController;

    private Agent _leader;

    public Agent Leader
    {
        get { return _leader; }
        set { _leader = value; }
    }

    public float ArriveWeight
    {
        get { return _arriveWeight; }
        set { _arriveWeight = value; }
    }

    public float SeparationWeight
    {
        get { return _separationWeight; }
        set { _separationWeight = value;
    }
    }

    public float FleeWeight
    {
        get { return _fleeWeight; }
        set { _fleeWeight = value; }
    }

    public float AuxLeaderDistance
    {
        get { return _auxLeaderDistance; }
        set { _auxLeaderDistance = value; }
    }

    public GameController GameController
    {
        get { return _gameController; }
        set { _gameController = value; }
    }

    void Start()
    {
        this.nameSteering = SteeringNames.LeaderFollowing;
    }


    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        SteeringBehaviour auxSteering;
        Steering auxSteer;

        // Creamos un lider auxiliar
        Target = CreateAuxLeader(Leader);

        // Obtenemos la ponderacion del Arrive
        auxSteering = agent.gameObject.AddComponent<Arrive>();
        auxSteering.Target = Target;
        auxSteer = auxSteering.GetSteering(agent);

        steer.linear += auxSteer.linear * _arriveWeight;
        steer.angular += auxSteer.angular * _arriveWeight;
        
        // Borramos el steering
        Destroy(auxSteering);

        // Obtenemos la ponderacion del Flee
        auxSteering = agent.gameObject.AddComponent<Flee>();
        auxSteering.Target = Leader;
        auxSteer = auxSteering.GetSteering(agent);

        steer.linear += auxSteer.linear * _fleeWeight;
        steer.angular += auxSteer.angular * _fleeWeight;
        
        // Borramos el steering
        Destroy(auxSteering);
        
        // Obtenemos la ponderacion del Separation
        auxSteering = agent.gameObject.AddComponent<Separation>();
        auxSteering.Target = Target;
        ((Separation) auxSteering).GameController = _gameController;
        auxSteer = auxSteering.GetSteering(agent);

        steer.linear += auxSteer.linear * _separationWeight;
        steer.angular += auxSteer.angular * _separationWeight;
        
        // Borramos el steering
        Destroy(auxSteering);
        
        // Borramos el lider auxiliar
        Destroy(Target.gameObject);

        return steer;
    }

    private Agent CreateAuxLeader(Agent leader)
    {
        Agent auxAgent;
        Vector3 leaderVelocity = leader.Velocity;
        
        // Creamos un personaje auxiliar
        GameObject auxiliar = new GameObject("DetectPlayer");
        auxAgent = auxiliar.AddComponent<Agent>();
        auxiliar.tag = "Auxiliar";

        auxAgent.Position = -leaderVelocity.normalized * _auxLeaderDistance + leader.Position;
        auxAgent.InteriorRadius = leader.InteriorRadius;
        auxAgent.ArrivalRadius = leader.ArrivalRadius;

        return auxAgent;

    }
}
