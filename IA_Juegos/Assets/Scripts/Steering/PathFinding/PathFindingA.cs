using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindingA : Arrive
{
    [SerializeField] private GameObject initial;
    [SerializeField] private GameObject final;
    [SerializeField] private DistanceMethods distanceMethods;
    private GridController _gridController;
    private Grid _grid;
    private Node _start;
    private Node _currentNode;
    private Node _end;
    private Heuristic _heuristic;
    private List<Node> _path;

    public Node StartNode
    {
        get { return _start; }
        set
        {
            _start = value;
            _currentNode = _start;
        }
    }

    public Node EndNode
    {
        get { return _end; }
        set { _end = value; }
    }

    public DistanceMethods DistanceMethod
    {
        get { return distanceMethods; }
        set { distanceMethods = value; }
    }

    public void Awake()
    {
        GameObject gameController = GameObject.Find("GameController");
        _gridController = gameController.GetComponent<GridController>();
    }


    private void Start()
    {
        _grid = _gridController.Grid;
        if (_start == null && _end == null)
        {
            _start = _grid.WorldPointToNode(initial.transform.position);
            _end = _grid.WorldPointToNode(final.transform.position);
            _currentNode = _start;
        }

        this.nameSteering = SteeringNames.PathFindingA;
        _heuristic = new Heuristic(_end, distanceMethods);
        InitializeCost();
    }

    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        _path = agent.Path;

        if (_path == null)
        {
            GeneratePath(agent);
            _path = agent.Path;
        }
        
        if (!_currentNode.Equals(_end) && AtNode(agent))
        {
            _currentNode = _path.ElementAt(_path.IndexOf(_currentNode) + 1);
        }

        // Creamos un personaje auxiliar
        GameObject auxiliar = new GameObject("DetectPlayer");
        Target = auxiliar.AddComponent<Agent>();
        auxiliar.tag = "Auxiliar";
        
        // Le asignamos la posicion necesaria
        Target.Position = _currentNode.WorldPosition;
        
        // Delegamos en Seek
        steer = base.GetSteering(agent);
        
        // Borramos al personaje auxiliar y volvemos a asignar el antiguo
        Destroy(auxiliar);

        // Retornamos el steering
        return steer;
    }

    private bool AtNode(Agent agent)
    {
        return _grid.WorldPointToNode(agent.Position).Equals(_currentNode);
    }

    private void InitializeCost()
    {
        Node n;
        for (int i = 0; i < _grid.NumCellsX; i++)
        {
            for (int j = 0; j < _grid.NumCellsY; j++)
            {
                n = _grid.GetNode(i, j);
                n.GCost = Mathf.Infinity;
                if (!n.Passable)
                {
                    n.HCost = Mathf.Infinity;
                }
            }
        }
        _start.GCost = 0f;
        _start.HCost = _heuristic.EstimateCost(_start);
        _end.HCost = 0f;
    }

    private void GeneratePath(Agent agent)
    {
        InitializeCost();
        A.AStar(_grid, _start, _end, _heuristic, agent);
    }
}
