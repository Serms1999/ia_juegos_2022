using System.Collections;
using System.Collections.Generic;
using UniBT;
using UnityEngine;

public class CaptureEnemyBase : Action
{
    private AgentNPC _agent;

    public override void Awake()
    {
        _agent = gameObject.GetComponent<AgentNPC>();
    }

    protected override Status OnUpdate()
    {
        _agent.CaptureEnemyBase();
        return Status.Success;
    }
}
