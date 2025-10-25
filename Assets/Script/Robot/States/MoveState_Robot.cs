using System;
using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;

public class MoveState_Robot : GroundState_Robot
{

    public MoveState_Robot(RobotController robot, CoherenceSync _sync, IStateMachine stateMachine, string anim) : base(robot, _sync, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Move");

    }
    public override void Excute()
    {
        base.Excute();

        if (Mathf.Abs(m_robot.m_movement.GetVelocity().magnitude) == 0)
            m_machine.ChangeState<IdleState_Robot>();
        
    }
    public override void Exit()
    {
        base.Exit();
    }
}