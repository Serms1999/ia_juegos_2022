using System.Collections;
using System.Collections.Generic;
using UniBT;
using UnityEngine;

public class Defend : Action
{
    private AgentNPC _agent;

    public override void Awake()
    {
        _agent = gameObject.GetComponent<AgentNPC>();
    }

    protected override Status OnUpdate()
    {
        if (_agent.GoingToTeamBase())
        {
            return Status.Success;
        }

        _agent.Path = null;
        _agent.Defend();
        return Status.Success;
    }
}
