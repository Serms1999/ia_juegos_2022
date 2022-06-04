using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class A
{
    public static void AStar(Grid grid, Node start, Node end, Heuristic h, Agent agent)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openList.Add(start);

        while (openList.Count > 0)
        {
            Node current = SmallestElement(openList);


            if (current.Equals(end))
            {
                ReversePath(grid, start, end, agent);
                return;
            }
            
            openList.Remove(current);
            closedSet.Add(current);

            List<Node> neighbours = grid.GetNeighbours(current);
            foreach (Node neighbour in neighbours)
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newCost = current.GCost + 1;

                newCost = current.GCost + agent.GetTerrainCost(current.TerrainType);
                
                /*
                float influenceValue = current.InfluenceValue * 5f;
                switch (agent.Team)
                {
                    case Teams.TeamA:
                    {
                        newCost -= influenceValue;
                        break;
                    }
                    case Teams.TeamB:
                    {
                        newCost += influenceValue;
                        break;
                    }
                }
                */

                if (newCost < neighbour.GCost)
                {
                    neighbour.GCost = newCost;
                    neighbour.HCost = h.EstimateCost(neighbour);
                    neighbour.Parent = current;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                                                
                }
            }
        }
    }
    
    private static Node SmallestElement(List<Node> list)
    {
        return list.OrderBy(n => n.FCost).First();
    }

    private static void ReversePath(Grid grid, Node start, Node end, Agent agent)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (!currentNode.Equals(start)) {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        
        path.Reverse();
        
        //grid.LocalSpace = path;
        agent.Path = path;
    }
}
