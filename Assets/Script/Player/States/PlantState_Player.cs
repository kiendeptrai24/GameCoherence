using System;
using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlantState_Player : PlayerState
{
    public PlantState_Player(PlayerController player, CoherenceSync _sync, IStateMachine stateMachine, string anim) : base(player, _sync, stateMachine, anim)
    {
    
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Excute()
    {
        base.Excute();
        if (Input.GetKeyDown(KeyCode.F))
            m_machine.ChangeState<IdleState_Player>();
    }
    public override void Exit()
    {
        base.Exit();
    }
}