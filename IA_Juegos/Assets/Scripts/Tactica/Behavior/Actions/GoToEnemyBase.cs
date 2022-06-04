using System.Collections;
using System.Collections.Generic;
using UniBT;
using UnityEngine;

public class GoToEnemyBase : Action
{
    private AgentNPC _agent;

    public override void Awake()
    {
        _agent = gameObject.GetComponent<AgentNPC>();
    }

    protected override Status OnUpdate()
    {
        if (_agent.GoingToEnemyBase())
        {
            return Status.Success;
        }
        _agent.GoToEnemyBase();
        return Status.Success;
    }
}
