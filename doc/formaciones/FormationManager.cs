using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FormationManager
{
    private List<SlotAssignment> _slotAssignments;
    private Steering _driftOffset;
    private AgentNPC _leader;
    private Formation _pattern;
    private bool _formationDone;
    private Vector3 _newPoint;

    public FormationManager()
    {
        _slotAssignments = new List<SlotAssignment>();
        _newPoint = Vector3.down;
    }

    public AgentNPC Leader
    {
        get { return _leader; }
        set { _leader = value; }
    }

    public Formation Pattern
    {
        get { return _pattern; }
        set { _pattern = value; }
    }

    public bool FormationDone
    {
        get { return _formationDone; }
        set { _formationDone = value; }
    }
    
    public Vector3 NewPoint
    {
        get { return _newPoint; }
        set { _newPoint = value; }
    }

    private void UpdateSlotAssignments()
    {
        for (int i = 0; i < _slotAssignments.Count; i++)
        {
            _slotAssignments[i].SlotNumber = i;
        }

        _driftOffset = _pattern.GetDriftOffset(_slotAssignments);
    }

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

    public void RemoveCharacter(AgentNPC agent)
    {
        int slot = _slotAssignments.Find(assignment => assignment.Agent.Equals(agent)).SlotNumber;

        if (slot >= 0 && slot <= _slotAssignments.Count)
        {
            _slotAssignments.RemoveAt(slot);
            UpdateSlotAssignments();
        }
    }

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
            //agent.RemoveAllSteerings();
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

    public bool AmILeader(AgentNPC agent)
    {
        return _leader.Equals(agent);
    }

    public bool LeaderAtNewPoint()
    {
        return Vector3.Distance(_newPoint, Leader.Position) < 0.01f;
    }

    public void UnfollowLeader()
    {
        foreach (SlotAssignment slot in _slotAssignments)
        {
            slot.Agent.RemoveSteering(SteeringNames.LeaderFollowing);
        }
        _newPoint = Vector3.down;
    }
}
