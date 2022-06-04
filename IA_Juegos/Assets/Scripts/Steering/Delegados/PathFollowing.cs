using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum PathOption
{
    Final,      // Se queda en el último nodo
    Back,       // Una vez que llega al final, da la vuelta y continua
    Circular    // Une el camino, formando un ciclo que se recorre para siempre
}

public class PathFollowing: Seek 
{
    [FormerlySerializedAs("path")] [SerializeField] protected WaypointsPath waypointsPath;
    [SerializeField] protected PathOption pathOption;

    protected int _pathDir;
    protected int _currentNode;

    protected void Start()
    {
        this.nameSteering = SteeringNames.PathFollowing;
        _currentNode = 0;
        _pathDir = 1;
    }
    public override Steering GetSteering(Agent agent)
    {   
        Steering steer = new Steering();
        Waypoint currentWaypoint;
        
        if(waypointsPath == null) return steer;

        // Conseguimos el nodo actual
        currentWaypoint = waypointsPath.GetWaypointAt(_currentNode);
        
        // Si ya he llegado, paso al siguiente
        if (Vector3.Distance(currentWaypoint.Position, agent.Position) <= currentWaypoint.Radius)
        {
            _currentNode += _pathDir;
        }

        // Decidimos que hacer en los extremos del camino
        switch (pathOption)
        {
            case PathOption.Final:
            {
                if (_currentNode == 0 && _pathDir < 0)
                {
                    _pathDir *= -1;
                }
                else if (_currentNode >= waypointsPath.NumNodes)
                {
                    _currentNode = waypointsPath.NumNodes - 1;
                }
                break;
            }
            case PathOption.Back:
            {
                if (_currentNode >= waypointsPath.NumNodes || _currentNode < 0)
                {
                    _pathDir *= -1;
                    _currentNode += _pathDir;
                }
                break;
            }
            case PathOption.Circular:
            {
                if (_currentNode >= waypointsPath.NumNodes)
                {
                    _currentNode = 0;
                }
                break;
            }
        }
        
        // Conseguimos el siguiente nodo
        currentWaypoint = waypointsPath.GetWaypointAt(_currentNode);
        
        // Creamos un agente auxiliar
        GameObject auxiliar = new GameObject("DetectPlayer");
        Target = auxiliar.AddComponent<Agent>();
        auxiliar.tag = "Auxiliar";
        
        // Asignamos la posicion al agente
        Target.Position = currentWaypoint.Position;
        
        // Delegamos en Seek
        steer = base.GetSteering(agent);
        
        // Borramos el agente auxiliar
        Destroy(auxiliar);

        return steer;
    }
}