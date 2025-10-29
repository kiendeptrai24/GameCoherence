using System;
using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;

public class MoveState_Player : GroundState_Player
{

    public MoveState_Player(PlayerController player, CoherenceSync _sync, IStateMachine stateMachine, string anim) : base(player, _sync, stateMachine, anim)
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
        if(m_player.m_movement == null || m_player.m_movement.GetVelocity() == null)
            return;
        if (Mathf.Abs(m_player.m_movement.GetVelocity().magnitude) == 0)
            m_machine.ChangeState<IdleState_Player>();
        
    }
    public override void Exit()
    {
        base.Exit();
    }
}