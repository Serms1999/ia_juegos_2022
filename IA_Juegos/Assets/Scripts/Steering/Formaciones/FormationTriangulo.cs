using UnityEngine;

/**
 * @class FormationTriangulo
 * @brief Formación en triángulo
 */
public class FormationTriangulo : Formation
{
    [SerializeField] private float distance = 1f;   // Distancia entre personajes.
    [SerializeField] private float angle = 30f;     // Cambio de ángulo entre personajes.
    
    /**
     * @brief Constructor por defecto.
     */
    public FormationTriangulo()
    {
        _numSlots = 7;
    }

    /**
     * @brief Devuelve una posición y una horientación para un personaje en la formación.
     * @param[in] slotNumber Índice de un personaje en la formación.
     * @return Posición y horientación para el personaje.
     */
    public override Steering GetSlotLocation(int slotNumber)
    {
        Steering location = new Steering();

        if (slotNumber == 0)
        {
            return location;
        }

        int vertical = (int) Mathf.Ceil(slotNumber / 2f);
        int horizontal = (int) (((slotNumber - 1) % 2 - 0.5f) * 2);

        location.linear = new Vector3(horizontal * vertical, 0f, -vertical);
        location.linear *= distance;

        location.angular = horizontal * vertical * angle;
        return location;
    }
}
