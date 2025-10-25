using System;
using System.Collections.Generic;
using Coherence.Toolkit;

public class RobotStateFactory : IStateFactory
{
    private readonly RobotController _robot;
    private readonly CoherenceSync _sync;
    private readonly IStateMachine _machine;
    public RobotStateFactory(RobotController robot, CoherenceSync sync, IStateMachine machine)
    {
        _robot = robot;
        _sync = sync;
        _machine = machine;
    }

    public Dictionary<Type, IState> CreateState()
    {
        var Playerdictionary = new Dictionary<Type, IState>
        {
            {typeof(IdleState_Robot), new IdleState_Robot(_robot, _sync, _machine, "Idle")},
            {typeof(MoveState_Robot), new MoveState_Robot(_robot, _sync, _machine, "Move")},
            {typeof(FlailState_Robot), new FlailState_Robot(_robot, _sync, _machine, "Flail")},

        };
        return Playerdictionary;
    }

}