using System;
using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;

public class FlailState_Robot : RobotState, IAnimationTrigger
{

    public FlailState_Robot(RobotController robot, CoherenceSync _sync, IStateMachine stateMachine, string anim) : base(robot, _sync, stateMachine, anim)
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
    }
    public override void Exit()
    {
        base.Exit();
    }
}