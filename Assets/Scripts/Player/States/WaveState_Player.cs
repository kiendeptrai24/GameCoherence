using System;
using Coherence;
using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WaveState_Player : PlayerState, IAnimationTrigger
{
    private NavMeshAgent navMeshAgent;
    public WaveState_Player(PlayerController player, CoherenceSync _sync, IStateMachine stateMachine, string anim) : base(player, _sync, stateMachine, anim)
    {

        navMeshAgent = m_player.GetComponent<NavMeshAgent>();
    }
    public void AnimationTrigger()
    {
        m_machine.ChangeState<IdleState_Player>();
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Excute()
    {
        base.Excute();
        if(m_player.m_movement == null || m_player.m_movement.GetVelocity() == null)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
            m_machine.ChangeState<IdleState_Player>();

        if (Mathf.Abs(m_player.m_movement.GetVelocity().magnitude) > 0)
        {
            m_anim.SetFloat("MovemntVelocity", 1);
            m_sync.SendCommand<AnimationSync>(nameof(AnimationSync.SetFloat), MessageTarget.Other, "MovemntVelocity", 1);
        }
        if (Mathf.Abs(m_player.m_movement.GetVelocity().magnitude) == 0)
        {
            m_anim.SetFloat("MovemntVelocity", 0);
            m_sync.SendCommand<AnimationSync>(nameof(AnimationSync.SetFloat), MessageTarget.Other, "MovemntVelocity", 0);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}