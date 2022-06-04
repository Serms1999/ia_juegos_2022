/**
 * @class Formation
 * @brief Representación de una formación.
 */
public class Formation
{
    protected int _numSlots;    // Número de agentes para la formación.

    /**
     * @brief Devuelve una posición y una horientación para un personaje en la formación.
     * @param[in] slotNumber Índice de un personaje en la formación.
     * @return Posición y horientación para el personaje.
     */
    public virtual Steering GetSlotLocation(int slotNumber)
    {
        return null;
    }

    /**
     * @brief Comprueba si caben una cantidad de personajes en la formación.
     * @param[in] slotCount Número de personajes.
     * @return true si caben ``slotCount`` personajes.
     */
    public bool SupportsSlots(int slotCount)
    {
        return slotCount <= _numSlots;
    }
}
