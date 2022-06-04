using System.Collections;
using System.Collections.Generic;
using UniBT;
using UnityEngine;

public class FarFromBase : Conditional
{
    private AgentNPC _agent;

    protected override void OnAwake()
    {
        _agent = gameObject.GetComponent<AgentNPC>();
    }

    protected override bool IsUpdatable()
    {
        if (_agent == null)
        {
            return false;
        }

        return _agent.FarFromBase();
    }
}
