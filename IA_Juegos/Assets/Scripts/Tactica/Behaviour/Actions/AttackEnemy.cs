using System.Collections;
using System.Collections.Generic;
using UniBT;
using UnityEngine;

public class AttackEnemy : Action
{
    private AgentNPC _agent;

    public override void Awake()
    {
        _agent = gameObject.GetComponent<AgentNPC>();
    }

    protected override Status OnUpdate()
    {
        AgentNPC enemy = _agent.GetNearestEnemy();
        _agent.Path = null;
        _agent.AttackEnemy(enemy);
        return Status.Success;
    }
}
