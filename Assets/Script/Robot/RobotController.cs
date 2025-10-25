using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;

[RequireComponent(typeof(CoherenceSync),typeof(AnimationSync))]
public class RobotController : MonoBehaviour
{
    public IStateMachine m_robotSM;
    public IMovement m_movement;
    public Animator anim;
    protected void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        m_movement = GetComponent<IMovement>();
        m_robotSM = new RobotStateMachine(this, GetComponent<CoherenceSync>());
    }
    protected void Start()
    {
        m_robotSM.Init<IdleState_Robot>();
    }
    private void Update()
    {
        if (m_robotSM != null)
        {
            m_robotSM.Update();
        }
    }
}