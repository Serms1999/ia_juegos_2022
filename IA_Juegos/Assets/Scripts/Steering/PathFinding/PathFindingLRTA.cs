using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingLRTA : Arrive
{
    [SerializeField] private GameObject initial;
    [SerializeField] private GameObject final;
    [SerializeField] private GridController gridController;
    [SerializeField] private DistanceMethods distanceMethods;
    private Grid _grid;
    private Node _start;
    private Node _currentNode;
    private Node _end;
    private Heuristic _heuristic;

    private void Start()
    {
        _grid = gridController.Grid;
        _start = _grid.WorldPointToNode(initial.transform.position);
        _end = _grid.WorldPointToNode(final.transform.position);
        this.nameSteering = SteeringNames.PathFindingLrta;
        _currentNode = _start;
        _heuristic = new Heuristic(_end, distanceMethods);
        InitializeCost();
        Agent agent = transform.parent.GetComponent<Agent>();
        GenerateLocalSpace(agent);
        ValueUpdateStep();
    }

    public override Steering GetSteering(Agent agent)
    {
        Steering steer = new Steering();
        

        if (!_currentNode.Equals(_end) && AtNode(agent))
        {
            _currentNode.HCost += 10f;
            _currentNode = _grid.GetNeighbours(_currentNode).OrderBy(n => n.HCost + 1f).First();
            _grid.Path.Add(_currentNode);
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
        
        // Si hemos salido del subespacio generamos uno nuevo
        if (!_grid.LocalSpace.Contains(_currentNode))
        {
            GenerateLocalSpace(agent);
            ValueUpdateStep();
        }
        
        // Retornamos el steering
        return steer;
    }

    private bool AtNode(Agent agent)
    {
       return _grid.WorldPointToNode(agent.Position).Equals(_currentNode);
    }
    
    private void GenerateLocalSpace(Agent agent)
    {
        A.AStar(_grid, _currentNode, _end, _heuristic, agent);
    }

    private void ValueUpdateStep()
    {
        List<Node> localSpace = new List<Node>(_grid.LocalSpace);
        float[] temps = new float[localSpace.Count];

        for (int i = 0; i < localSpace.Count; i++)
        {
            temps[i] = localSpace[i].HCost;
            localSpace[i].HCost = Mathf.Infinity;
        }
        
        Node v;

        while (localSpace.Count > 0)
        {
            v = GetNextNeighbour(_grid, localSpace, temps);

            if (float.IsInfinity(v.HCost))
            {
                return;
            }

            localSpace.Remove(v);
        }
    }

    private Node GetNextNeighbour(Grid grid, List<Node> localSpace, float[] temps)
    {
        Dictionary<Node, float> values = new Dictionary<Node, float>();
        List<Node> neighbours;
        Node minValue;
        foreach (Node node in localSpace)
        {
            neighbours = grid.GetNeighbours(node);
            minValue = neighbours.OrderBy(n => n.HCost).First();
            values.Add(node, Mathf.Max(temps[localSpace.IndexOf(node)], minValue.HCost + 1f));
        }

        Node v = values.OrderBy(v => v.Value).First().Key;
        v.HCost = values[v];

        return v;
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
}
