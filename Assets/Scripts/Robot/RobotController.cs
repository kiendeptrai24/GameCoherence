using System;
using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;

public class RobotController : Charactor
{
    public PickUp pickUp;
    private GameManager gameManager;
    protected override void Awake()
    {
        base.Awake();
        pickUp = GetComponent<PickUp>();

        gameManager = GameManager.Instance;
        gameManager.OnGameEnd += () =>
        {
            if (!_sync.HasInputAuthority) return;
            Destroy(gameObject);

        };

        pickUp.animAction += (active) =>
        {
            if (active)
                stateMachine.ChangeState<CrateState_Robot>();
            else
                stateMachine.ChangeState<IdleState_Robot>();
        };
        stateMachine = new RobotStateMachine(this, GetComponent<CoherenceSync>());
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Init<IdleState_Robot>();
    }
    private void Update()
    {
        if (stateMachine != null)
        {
            stateMachine.Update();
        }
    }
}