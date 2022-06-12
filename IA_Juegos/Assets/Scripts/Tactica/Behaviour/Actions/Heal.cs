using System.Collections;
using System.Collections.Generic;
using UniBT;
using UnityEngine;

public class Heal : Action
{
    private AgentNPC _agent;

    public override void Awake()
    {
        _agent = gameObject.GetComponent<AgentNPC>();
    }

    protected override Status OnUpdate()
    {
        _agent.Path = null;
        _agent.Heal();
        return Status.Success;
    }
}
