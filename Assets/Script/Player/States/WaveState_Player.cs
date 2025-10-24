using System;
using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WaveState_Player : PlayerState , AnimationTrigger
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
        // navMeshAgent.isStopped = true;
        // navMeshAgent.SetDestination(m_player.transform.position);
    }
    public override void Excute()
    {
        base.Excute();
        if (Input.GetKeyDown(KeyCode.Space))
            m_machine.ChangeState<IdleState_Player>();
    }
    public override void Exit()
    {
        base.Exit();
        // navMeshAgent.isStopped = false;
    }
}