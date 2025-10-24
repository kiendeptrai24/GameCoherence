using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;

[RequireComponent(typeof(CoherenceSync),typeof(AnimationSync))]
public class PlayerController : MonoBehaviour
{
    public IStateMachine m_playerSM;
    public IMovement m_movement;
    public Animator anim;
    protected void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        m_movement = GetComponent<IMovement>();
        m_playerSM = new PlayerStateMachine(this, GetComponent<CoherenceSync>());
    }
    protected void Start()
    {
        m_playerSM.Init<IdleState_Player>();
    }
    private void Update()
    {
        if (m_playerSM != null)
        {
            m_playerSM.Update();
        }
    }
}