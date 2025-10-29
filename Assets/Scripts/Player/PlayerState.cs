

using Coherence;
using Coherence.Toolkit;
using UnityEngine;

public abstract class  PlayerState : IState
{
    protected PlayerController m_player;
    protected CoherenceSync m_sync;
    protected IStateMachine m_machine;
    protected Animator m_anim;
    protected string m_animName;
    protected float m_stateTimer;
    public PlayerState(PlayerController player, CoherenceSync _sync, IStateMachine stateMachine, string animName)
    {
        m_player = player;
        m_anim = player.anim;
        m_sync = _sync;
        m_machine = stateMachine;
        m_animName = animName;
    }
 
    public virtual void Enter()
    {
        m_stateTimer = 0;
        m_anim.SetBool(m_animName,true);
        m_sync.SendCommand<AnimationSync>(nameof(AnimationSync.SetBool), MessageTarget.Other, m_animName, true);
    }

    public virtual void Excute()
    {   
        m_stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        m_anim.SetBool(m_animName,false);
        m_sync.SendCommand<AnimationSync>(nameof(AnimationSync.SetBool), MessageTarget.Other, m_animName, false);
    }
}