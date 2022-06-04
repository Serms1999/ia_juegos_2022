using UnityEngine;

/**
 * @class Collision
 * @brief Representación de una colisión de un bigote.
 */
public class Collision
{
    private Vector3 _position;  // Posición de la colisión.
    private Vector3 _normal;    // Vector normal de la colisión.

    /**
     * @return Posición de la colisión.
     */
    public Vector3 Position
    {
        get { return _position; }
        set { _position = value; }
    }
    
    /**
     * @return Vector normal de la colisión.
     */
    public Vector3 Normal
    {
        get { return _normal; }
        set { _normal = value; }
    }

    /**
     * @brief Constructor por defecto.
     */
    public Collision()
    {
        _position = Vector3.zero;
        _normal = Vector3.zero;
    }
}
