/**
 * @class SlotAssignment
 * @brief Asocia a un agente y una posición en la formación.
 */
public class SlotAssignment
{
    private AgentNPC _agent;    // Agente.
    private int _slotNumber;    // Posición en la formación.

    /**
     * @return Agente.
     */
    public AgentNPC Agent
    {
        get { return _agent; }
        set { _agent = value; }
    }

    /**
     * @return Posición en la formación.
     */
    public int SlotNumber
    {
        get { return _slotNumber; }
        set { _slotNumber = value; }
    }
}
