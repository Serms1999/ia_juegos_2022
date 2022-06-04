using UnityEngine;

/**
 * @class FormationColumnas
 * @brief Formación en columnas.
 */
public class FormationColumnas: Formation
{
    [SerializeField] private float distance = 3f;   // Distancia entre personajes.
    [SerializeField] private float angle = 30f;     // Cambio de ángulo entre personajes.
    /**
     * @brief Constructor por defecto.
     */
    public FormationColumnas()
    {
        _numSlots = 8;
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
        int horizontal = 0;
        if (slotNumber < _numSlots - 1)
        {
            switch (slotNumber % 2)
            {
                case 0:
                    horizontal = -1;
                    break;
                case 1:
                    horizontal = 1;
                    break;


            }
        }


        location.linear = new Vector3(horizontal, 0f, -vertical);
        location.linear *= distance;

        location.angular = horizontal * vertical * angle;
        return location;
        
    }
}
