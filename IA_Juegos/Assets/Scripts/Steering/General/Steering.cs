using UnityEngine;

/**
 * @class Steering
 * @brief Representa un par aceleración lineal y aceleración angular.
 */
[System.Serializable]
public class Steering
{
    public float angular;   // Aceleración angular.
    public Vector3 linear;  // Aceleración lineal.

    /**
     * @brief Constructor por defecto.
     */
    public Steering()
    {
        angular = 0.0f;
        linear = Vector3.zero;
    }

    /**
     * @brief Suma de Steerings
     */
    public static Steering operator +(Steering steer1, Steering steer2)
    {
        Steering steer = new Steering();

        steer.linear = steer1.linear + steer2.linear;
        steer.angular = steer1.angular + steer2.angular;

        return steer;
    }
}
