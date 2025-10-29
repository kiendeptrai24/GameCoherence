using System;
using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;

public class RobotController : Charactor
{

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new RobotStateMachine(this, GetComponent<CoherenceSync>());
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Init<IdleState_Robot>();
    }
    private void Update()
    {
        if (stateMachine != null)
        {
            stateMachine.Update();
        }
    }
}