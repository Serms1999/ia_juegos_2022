using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FormationPattern
{
    Triangulo,
    Columnas
}

public class Formation
{
    protected int _numSlots;
    protected FormationPattern _formationPattern;

    public int NumSlots
    {
        get { return _numSlots; }
    }

    public FormationPattern FormationPattern
    {
        get { return _formationPattern; }
    }

    public virtual Steering GetDriftOffset(List<SlotAssignment> slotAssignments)
    {
        return null;
    }

    public virtual Steering GetSlotLocation(int slotNumber)
    {
        return null;
    }

    public bool SupportsSlots(int slotCount)
    {
        return slotCount <= _numSlots;
    }
}
