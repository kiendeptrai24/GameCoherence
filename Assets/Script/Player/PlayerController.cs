using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;


public class PlayerController : Charactor
{

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine(this, GetComponent<CoherenceSync>());
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Init<IdleState_Player>();
    }
    private void Update()
    {
        if (stateMachine != null)
        {
            stateMachine.Update();
        }
    }

}