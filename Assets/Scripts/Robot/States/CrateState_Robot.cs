using System;
using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;

public class CrateState_Robot : RobotState, IAnimationTrigger
{

    public CrateState_Robot(RobotController robot, CoherenceSync _sync, IStateMachine stateMachine, string anim) : base(robot, _sync, stateMachine, anim)
    {
    }

    public void AnimationTrigger()
    {
        m_machine.ChangeState<IdleState_Robot>();
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Excute()
    {
        base.Excute();
          if(m_robot.m_movement == null || m_robot.m_movement.GetVelocity() == null)
            return;
        if (Input.GetKeyDown(KeyCode.F))
            m_machine.ChangeState<IdleState_Robot>();
    }
    public override void Exit()
    {
        base.Exit();
    }
}