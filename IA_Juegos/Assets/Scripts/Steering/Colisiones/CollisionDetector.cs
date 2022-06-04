using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/**
 * @class CollisionDetector
 * @brief Controlador de las colisiones.
 */
public class CollisionDetector
{
    /**
     * @brief Comprueba las colisiones futuras del agente ``agent``.
     * @param[in] agent Agente a comprobar.
     * @return Primera colisión futura del agente o null si no se va a dar ninguna.
     */
    [CanBeNull]
    public Collision GetCollision(Agent agent)
    {
        Vector3 origin = agent.Position;
        List<Vector3> rays = agent.Rays;
        Collision collision;
        foreach (Vector3 ray in rays)
        {
            if ((collision = CheckRay(origin, ray)) != null)
            {
                return collision;
            }
        }

        return null;
    }

    // Comprobamos cada rayo.
    private Collision CheckRay(Vector3 origin, Vector3 moveAmount)
    {
        Collision collision = new Collision();
        RaycastHit hit;
        
        if (Physics.Raycast(origin, moveAmount, out hit) && hit.collider.CompareTag("Wall"))
        {
            collision.Position = hit.point;
            collision.Normal = hit.normal;
            return collision;
        }

        return null;
    }
}
