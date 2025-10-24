using System;
using System.Collections.Generic;
using Coherence.Toolkit;

public class PlayerStateFactory : IStateFactory
{
    private readonly PlayerController _player;
    private readonly CoherenceSync _sync;
    private readonly IStateMachine _machine;
    public PlayerStateFactory(PlayerController player, CoherenceSync sync, IStateMachine machine)
    {
        _player = player;
        _sync = sync;
        _machine = machine;
    }

    public Dictionary<Type, IState> CreateState()
    {
        var Playerdictionary = new Dictionary<Type, IState>
        {
            {typeof(IdleState_Player), new IdleState_Player(_player, _sync, _machine, "Idle")},
            {typeof(MoveState_Player), new MoveState_Player(_player, _sync, _machine, "Move")},
            {typeof(WaveState_Player), new WaveState_Player(_player, _sync, _machine, "Wave")},
            {typeof(CrateState_Player), new CrateState_Player(_player, _sync, _machine, "Crate")},
            {typeof(PlantState_Player), new PlantState_Player(_player, _sync, _machine, "Plant")},
        };
        return Playerdictionary;
    }

}