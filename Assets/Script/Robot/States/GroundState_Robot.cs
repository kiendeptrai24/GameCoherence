using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;

public class GroundState_Robot : RobotState
{
    public GroundState_Robot(RobotController robot, CoherenceSync _sync, IStateMachine stateMachine, string anim) : base (robot, _sync, stateMachine, anim)
    {
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
        if (Input.GetKeyDown(KeyCode.Space))
            m_machine.ChangeState<FlailState_Robot>();

    }
    public override void Exit()
    {
        base.Exit();
    }

}