using Coherence.Toolkit;
using Unity.VisualScripting;
using UnityEngine;

public class GroundState_Player : PlayerState
{
    public GroundState_Player(PlayerController player, CoherenceSync _sync, IStateMachine stateMachine, string anim) : base (player, _sync, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Excute()
    {
        base.Excute();
        if (Input.GetKeyDown(KeyCode.Space))
            m_machine.ChangeState<WaveState_Player>();
        // Debug.Log(Mathf.Abs(m_player.m_movement.GetVelocity().magnitude));
    }
    public override void Exit()
    {
        base.Exit();
    }

}