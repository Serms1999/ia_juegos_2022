using UnityEngine;

/**
 * @class Árbitro
 * @brief Ábrito que modifica los steerings a aplicar.
 */
public class Arbitro : MonoBehaviour
{
    /**
     * @brief Devuelve el steering modificado.
     * @param[in] behaviour Comportamiento.
     * @param[in] steering Steering a modificar.
     * @return Steering modificado.
     */
    public virtual Steering GetSteering(SteeringBehaviour behaviour, Steering steering)
    {
        return null;
    }
}
