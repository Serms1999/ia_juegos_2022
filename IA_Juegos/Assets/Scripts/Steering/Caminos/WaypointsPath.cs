using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * @class WaypointsPath
 * @brief Camino de waypoints.
 */
public class WaypointsPath : MonoBehaviour
{
    private List<Waypoint> _waypoints;      // Lista de waypoints.
    [SerializeField] private float radius;  // Radio de cada waypoint.
    [SerializeField] private bool circular; // Switch de camino circular.

    private void Awake()
    {
        _waypoints = new List<Waypoint>(GetComponentsInChildren<Waypoint>());

        foreach (Waypoint waypoint in _waypoints)
        {
            waypoint.Radius = radius;
        }
    }

    /**
     * @return Número de waypoints en el camino.
     */
    public int NumNodes
    {
        get { return _waypoints.Count; }
    }

    /**
     * @return Waypoint en el índice ``node``.
     */
    public Waypoint GetWaypointAt(int node)
    {
        return _waypoints.ElementAt(node);
    }

    private void OnDrawGizmos()
    {
        // Recorremos todos los hijos
        foreach (Transform node in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(node.position, radius);
        }
        
        Gizmos.color = Color.black;
        // Dibujamos las aristas
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position,
                transform.GetChild(i + 1).position);
        }
        
        // Enlazamos ultimo y primero
        if (circular)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position,
                transform.GetChild(0).position);
        }
    }
}
