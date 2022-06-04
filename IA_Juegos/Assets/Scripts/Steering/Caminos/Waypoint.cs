using UnityEngine;

/**
 * @class Waypoint
 * @brief Punto por el que pasará un agente.
 */
public class Waypoint : MonoBehaviour
{
    private float _radius;  // Radio

    /**
     * @return Radio.
     */
    public float Radius
    {
        get { return _radius; }
        set { _radius = value; }
    }

    /**
     * @return Posición del punto en el mundo.
     */
    public Vector3 Position
    {
        get { return new Vector3(transform.position.x, 0, transform.position.z); }
        set { transform.position = value; }
    }
}
