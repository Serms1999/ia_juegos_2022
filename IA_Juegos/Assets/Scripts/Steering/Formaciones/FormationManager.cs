using System.Collections.Generic;
using UnityEngine;

/**
 * @class FormationManager
 * @brief Controlador de las formaciones.
 */
public class FormationManager
{
    private List<SlotAssignment> _slotAssignments;  // Lista de casillas en la formación.
    private AgentNPC _leader;                       // Líder de la formación.
    private Formation _pattern;                     // Tipo de formación
    private bool _formationDone;                    // Comprueba si la formación está hecha.
    private Vector3 _newPoint;                      // Posición auxiliar para el movimiento.
    
    /**
     * @return Líder de la formación.
     */
    public AgentNPC Leader
    {
        get { return _leader; }
        set { _leader = value; }
    }

    /**
     * @return Tipo de formación.
     */
    public Formation Pattern
    {
        get { return _pattern; }
        set { _pattern = value; }
    }

    /**
     * @return Comprueba si la formación está hecha.
     */
    public bool FormationDone
    {
        get { return _formationDone; }
        set { _formationDone = value; }
    }
    
    /**
     * @return Posición auxiliar para el movimiento.
     */
    public Vector3 NewPoint
    {
        get { return _newPoint; }
        set { _newPoint = value; }
    }

    /**
     * @brief Constructor por defecto.
     */
    public FormationManager()
    {
        _slotAssignments = new List<SlotAssignment>();
        _newPoint = Vector3.down;
    }

    // Actualizar las posiciones.
    private void UpdateSlotAssignments()
    {
        for (int i = 0; i < _slotAssignments.Count; i++)
        {
            _slotAssignments[i].SlotNumber = i;
        }
    }

    /**
     * @brief Añadir personaje a la formación.
     * @param[in] agent Agente a añadir.
     * @return true si se ha podido añadir.
     */
    public bool AddCharacter(AgentNPC agent)
    {
        int occupiedSlots = _slotAssignments.Count;

        if (_pattern.SupportsSlots(occupiedSlots + 1))
        {
            if (occupiedSlots == 0)
            {
                _leader = agent;
            }
            
            SlotAssignment slotAssignment = new SlotAssignment();
            slotAssignment.Agent = agent;
            slotAssignment.SlotNumber = occupiedSlots;
            _slotAssignments.Add(slotAssignment);
            
            UpdateSlotAssignments();
            agent.Formation = this;
            return true;
        }

        return false;
    }

    /**
     * @brief Actualizar los slots de la formación.
     */
    public void UpdateSlots()
    {
        Vector3 anchor = _leader.Position;
        float orientation = _leader.Orientation;

        for (int i = 1; i < _slotAssignments.Count; i++)
        {
            Steering relativeLoc = _pattern.GetSlotLocation(_slotAssignments[i].SlotNumber);
            
            GameObject auxiliar = new GameObject("DetectPlayer");
            Agent target = auxiliar.AddComponent<Agent>();
            auxiliar.tag = "Auxiliar";
            
            target.Position = Quaternion.AngleAxis(orientation, Vector3.up) * relativeLoc.linear + anchor;
            target.Orientation = orientation + relativeLoc.angular;

            AgentNPC agent = _slotAssignments[i].Agent;
            agent.RemoveAllSteeringsExcept(new List<string>()
            {
                SteeringNames.LookingWhereYoureGoing,
                SteeringNames.WallAvoidance
            });
            agent.AddSteering(SteeringNames.Seek, target);
            agent.AddSteering(SteeringNames.Align, target);
        }

        _formationDone = true;
    }

    /**
     * @brief Comprueba si un agente es el líder
     * @param[in] agent Agente a comprobar
     * @return true si el agente es el líder.
     */
    public bool AmILeader(AgentNPC agent)
    {
        return _leader.Equals(agent);
    }

    /**
     * @brief Comprueba si el agente ha llegado a la posición con un margen de error.
     * @return true si ha llegado.
     */
    public bool LeaderAtNewPoint()
    {
        return Vector3.Distance(_newPoint, Leader.Position) < 0.01f;
    }

    /**
     * @brief Ordena a los personajes dejar de seguir al líder.
     */
    public void UnfollowLeader()
    {
        foreach (SlotAssignment slot in _slotAssignments)
        {
            slot.Agent.RemoveSteering(SteeringNames.LeaderFollowing);
        }
        _newPoint = Vector3.down;
    }
}
